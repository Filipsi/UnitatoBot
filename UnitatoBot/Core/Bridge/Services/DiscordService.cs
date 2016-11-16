using Discord;
using Discord.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotCore.Util;
using Discord.Rest;
using Discord.WebSocket;

namespace BotCore.Bridge.Services {

    public class DiscordService : IService {

        private readonly DiscordSocketClient  _client;

        public DiscordService(string token) {
            _client = new DiscordSocketClient();

            // Run setup task synchronously
            Setup(token).GetAwaiter().GetResult();

            Logger.Log("{0} service was inicialized with token {1}", GetServiceType(), token);
        }

        private async Task Setup(string token) {
            _client.MessageReceived += HandleReceivedMessage;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.ConnectAsync();
        }

        private Task HandleReceivedMessage(SocketMessage message) {
            if(!message.Author.Id.Equals(_client.CurrentUser.Id))
                OnMessageReceived?.Invoke(this, new ServiceMessageEventArgs(new ServiceMessage(this, message as IUserMessage)));

            return Task.CompletedTask;
        }

        // Logic

        private async Task<RestUserMessage> SendPartableMessage(ISocketMessageChannel channel, string text) {
            if(channel == null)
                return null;

            if(text.Length > DiscordConfig.MaxMessageSize) {
                string buffer = text;

                List<string> parts = new List<string>();
                do {
                    int msgLength = buffer.Length >= DiscordConfig.MaxMessageSize ? DiscordConfig.MaxMessageSize : buffer.Length;
                    int lastUsableSpace = buffer.LastIndexOf(" ", msgLength, StringComparison.Ordinal);

                    int cut = lastUsableSpace > 0 ? lastUsableSpace : msgLength;
                    parts.Add(buffer.Substring(0, cut));
                    buffer = buffer.Substring(cut);

                } while(buffer.Length > 0);

                Task<RestUserMessage> firstSent = null;
                foreach(string part in parts) {
                    if(firstSent == null) firstSent = channel.SendMessageAsync(part); else await channel.SendMessageAsync(part);
                }

                Logger.Info("While sending message, DiscordConfig.MaxMessageSize was exceeded! Message was split to {0} parts.", parts.Count);
                return await firstSent;
            }

            return await channel.SendMessageAsync(text);
        }
   
        // IService

        public event EventHandler<ServiceMessageEventArgs> OnMessageReceived;

        public string GetServiceType() {
            return "Discord";
        }

        public string GetServiceId() {
            return _client.CurrentUser.Id.ToString();
        }

        public ServiceMessage SendMessage(string destination, string text) {
            ISocketMessageChannel channel = _client.GetChannel(ulong.Parse(destination)) as ISocketMessageChannel;
            return new ServiceMessage(this, SendPartableMessage(channel, text).GetAwaiter().GetResult());
        }

        public ServiceMessage FindMessage(string destination, string id) {
            ITextChannel channel = _client.GetChannel(ulong.Parse(destination)) as ITextChannel;
            IUserMessage message = channel?.GetMessageAsync(ulong.Parse(id)).GetAwaiter().GetResult() as IUserMessage;
            return message == null ? null : new ServiceMessage(this, message);
        }
       
    }

}
