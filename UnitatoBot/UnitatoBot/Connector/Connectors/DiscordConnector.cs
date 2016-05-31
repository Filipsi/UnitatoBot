using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Connector.Connectors {

    internal class DiscordConnector : IConnector {

        public CommandManager CommandManager { private set; get; }
        public Channel Channel { private set; get; }

        private DiscordClient Client;
        private ulong ChannelId;

        public DiscordConnector(string email, string password, ulong channel) {  
            this.CommandManager = new CommandManager(this);
            this.Client = new DiscordClient();
            this.ChannelId = channel;

            // Create a task that will trigger after Client fires Ready event
            TaskCompletionSource<bool> taskClientReady = new TaskCompletionSource<bool>();

            // Bind trigger for the task to Ready event
            Client.Ready += (sender, args) => {
                Console.WriteLine("Discord client is ready.");
                taskClientReady.SetResult(true);
            };

            // Try to connect, handle errors if any
            try {
                Client.Connect(email, password);
                Console.WriteLine("Discord connection sucessfully enstablished!");
            } catch(Exception e) {
                Console.WriteLine("Something went wrong during Discord connection attempt!\n" + e.Message);
            }

            // Wait until Client is ready
            while(!taskClientReady.Task.IsCompleted) { /* NO-OP */ }

            // Retrives the channel where it should perform tasks
            this.Channel = Client.GetChannel(channel);
        }

        public void Begin() {
            InitEventhandlers();
            Console.WriteLine("DiscordConnector event handlers inicilized.");
            CommandManager.Initialize();
            Console.WriteLine("CommandManager was inicilized.");
        }

        private void InitEventhandlers() {
            Client.MessageReceived += (sender, args) => {
                if(args.Channel.Id.Equals(this.Channel.Id) && !args.User.Id.Equals(Client.CurrentUser.Id)) {
                    OnMessageReceived(this, new ConnectionMessageEventArgs(new ConnectionMessage(this, args.Message)));
                }
            };
        }

        // IConnector

        public event EventHandler<ConnectionMessageEventArgs> OnMessageReceived;

        public void SendMessage(string text) {
            this.Channel.SendMessage(text);
        }

    }

}
