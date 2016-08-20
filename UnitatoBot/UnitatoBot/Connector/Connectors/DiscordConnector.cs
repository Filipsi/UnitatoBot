using Discord;
using Discord.Audio;
using NAudio.Wave;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UnitatoBot.Connector.Connectors {

    internal class DiscordConnector : IConnector, IAudioCapability {

        private Server          Server;
        private DiscordClient   Client;
        private AudioService    Audio; 
        private ulong           ServerId;

        public DiscordConnector(string email, string password, ulong serverId) {  
            this.Client = new DiscordClient();
            this.ServerId = serverId;

            // Create a task that will trigger after Client fires Ready event
            TaskCompletionSource<bool> taskClientReady = new TaskCompletionSource<bool>();

            // Bind trigger for the task to Ready event
            Client.Ready += (sender, args) => {
                Logger.Log("{0} client is ready.", this.GetType().Name);
                taskClientReady.SetResult(true);
            };

            // Try to connect, handle errors if any
            try {
                Client.Connect(email, password);
                Logger.Log("{0} connection sucessfully enstablished!", this.GetType().Name);
            } catch(Exception e) {
                Logger.Error("Something went wrong during {0} connection attempt!\n" + e.Message, this.GetType().Name);
            }

            // Wait until Client is ready
            while(!taskClientReady.Task.IsCompleted) { /* NO-OP */ }

            // Retrives server where it should perform tasks
            Server = Client.GetServer(serverId);

            // Audio service setup
            Client.AddService(new AudioService(new AudioServiceConfigBuilder() {
                Mode = AudioMode.Outgoing,
                EnableEncryption = false,
                Bitrate = 128
            }));

            // Retrives audio service from client
            Audio = Client.GetService<AudioService>();

            // Initializes event handlers 
            InitEventHandlers();
            Logger.Log("{0} event handlers inicilized.", this.GetType().Name);
        }

        private void InitEventHandlers() {
            Client.MessageReceived += (sender, args) => {
                if(args.Server.Id.Equals(this.Server.Id) && !args.User.Id.Equals(Client.CurrentUser.Id)) {
                    OnMessageReceived(this, new ConnectionMessageEventArgs(new ConnectionMessage(this, args.Message)));
                }
            };
        }

        // Util

        private async Task<Message> SendText(Channel channel, string text) {
            return channel != null ? await channel.SendMessage(text) : null;
        }

        private async void PlaySoundFile(Channel channel, string file) {
            // https://github.com/RogueException/Discord.Net/blob/master/src/Discord.Net.Audio/opus.dll

            IAudioClient ac = await Audio.Join(channel);

            System.Threading.Thread.Sleep(250);

            var OutFormat = new WaveFormat(48000, 16, Audio.Config.Channels);
            using(var MP3Reader = new Mp3FileReader(file))
            using(var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) {
                resampler.ResamplerQuality = 60;
                int blockSize = OutFormat.AverageBytesPerSecond / 50;
                byte[] buffer = new byte[blockSize];
                int byteCount;

                while((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) {
                    if(byteCount < blockSize) {
                        for(int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }
                    ac.Send(buffer, 0, blockSize);
                }
            }

            System.Threading.Thread.Sleep(1000);

            await ac.Disconnect();
        }

        // IConnector

        public string GetIdentificator() {
            return Server.Id.ToString();
        }

        public event EventHandler<ConnectionMessageEventArgs> OnMessageReceived;

        public ConnectionMessage SendMessage(string destination, string text) {
            Channel channel = Server.TextChannels.First(c => c.Id.ToString().Equals(destination));

            if(channel == null) {
                Logger.Warn("Text channel {0} not found while sending message!", destination);
                return null;
            }

            Message msg = SendText(channel, text).Result;

            if(msg == null)
                return null;

            // Oh god, this is wrong.
            while(msg.Id == 0) {
                /* NO-OP */
            }

            return new ConnectionMessage(this, msg);
        }

        public ConnectionMessage FindMessage(string destination, string id) {
            Channel channel = Server.TextChannels.FirstOrDefault(c => c.Id.ToString().Equals(destination));

            if(channel == null) {
                Logger.Warn("Text channel {0} not found while searching for message {1}", destination, id);
                return null;
            }

            // Message has no data (https://github.com/RogueException/Discord.Net/blob/master/src/Discord.Net/Models/Channel.cs#L284)
            Message msg = channel.GetMessage(Convert.ToUInt64(id));
            
            return msg == null ? null : new ConnectionMessage(this, msg);
        }


        // IAudioCapability

        public void SendAudio(string destination, string file) {
            Channel channel = Server.VoiceChannels.FirstOrDefault(c => c.Name.ToString().Equals(destination));

            if(channel == null) {
                Logger.Warn("Audio channel {0} not found!", destination);
                return;
            }

            PlaySoundFile(channel, file);
        }

    }

}
