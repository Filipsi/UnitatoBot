using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UnitatoBot.Connector.Connectors {

    internal class DiscordConnector : IConnector {

        public Server Server { private set; get; }

        private DiscordClient Client;
        private ulong ServerId;

        public DiscordConnector(string email, string password, ulong serverId) {  
            this.Client = new DiscordClient();
            this.ServerId = serverId;

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
            this.Server = Client.GetServer(serverId);

            // Initializes event handlers 
            InitEventHandlers();
            Console.WriteLine("{0} event handlers inicilized.", this.GetType().Name);
        }

        private void InitEventHandlers() {
            Client.MessageReceived += (sender, args) => {
                if(args.Server.Id.Equals(this.Server.Id) && !args.User.Id.Equals(Client.CurrentUser.Id)) {
                    OnMessageReceived(this, new ConnectionMessageEventArgs(new ConnectionMessage(this, args.Message)));
                }
            };
        }

        public async Task<Message> Send(string destination, string text) {
            Channel channel = Server.TextChannels.First(c => c.Id.ToString().Equals(destination));
            return channel != null ? await channel.SendMessage(text) : null;
        }

        // IConnector

        public string GetIdentificator() {
            return Server.Id.ToString();
        }

        public event EventHandler<ConnectionMessageEventArgs> OnMessageReceived;

        public ConnectionMessage SendMessage(string destination, string text) {
            Message msg = Send(destination, text).Result;

            if(msg == null)
                return null;

            // Oh god, this is wrong.
            while(msg.Id == 0) {
                /* NO-OP */
            }

            return new ConnectionMessage(this, msg);
        }

        public ConnectionMessage FindMessage(string destination, string id) {
            Channel channel = Server.TextChannels.First(c => c.Id.ToString().Equals(destination));

            // Message has no data (https://github.com/RogueException/Discord.Net/blob/master/src/Discord.Net/Models/Channel.cs#L284)
            Message msg = channel.GetMessage(Convert.ToUInt64(id));
            
            return channel == null ? null : new ConnectionMessage(this, msg);
        }


    }

}
