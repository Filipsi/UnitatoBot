using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Command;

namespace UnitatoBot.Connector.Connectors {

    internal class DiscordConnector : IConnector {

        public Channel Channel { private set; get; }

        private DiscordClient Client;
        private ulong ChannelId;

        public DiscordConnector(string email, string password, ulong channel) {  
            this.Client = new DiscordClient();
            this.ChannelId = channel;

            // Create a task that will trigger after Client fires Ready event
            TaskCompletionSource<bool> taskClientReady = new TaskCompletionSource<bool>();

            // Bind trigger for the task to Ready event
            Client.Ready += (sender, args) => {
                Console.WriteLine("{0} client is ready.", this.GetType().Name);
                taskClientReady.SetResult(true);
            };

            // Try to connect, handle errors if any
            try {
                Client.Connect(email, password);
                Console.WriteLine("{0} connection sucessfully enstablished!", this.GetType().Name);
            } catch(Exception e) {
                Console.WriteLine("Something went wrong during {0} connection attempt!\n" + e.Message, this.GetType().Name);
            }

            // Wait until Client is ready
            while(!taskClientReady.Task.IsCompleted) { /* NO-OP */ }

            // Retrives the channel where it should perform tasks
            this.Channel = Client.GetChannel(channel);

            // Initializes event handlers 
            InitEventHandlers();
            Console.WriteLine("{0} event handlers inicilized.", this.GetType().Name);
        }

        private void InitEventHandlers() {
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
