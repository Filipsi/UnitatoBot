using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot {

    internal class DiscordConnector : IConnector {

        public CommandManager CommandManager { private set; get; }
        public Channel Channel { private set; get; }

        private ulong ChannelId;
        private DiscordClient Client;

        public DiscordConnector(string email, string password, ulong channel) {
            this.ChannelId = channel;
            this.CommandManager = new CommandManager(this);
            this.Client = new DiscordClient();

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
            Console.WriteLine("DiscordConnector event handlers inicilized.");
            InitEventhandlers();
            CommandManager.Initialize();
        }

        private void InitEventhandlers() {
            Client.MessageReceived += (sender, args) => {
                Console.WriteLine("Msg {0} from {1} at {2}", args.Message.Text, args.User, args.Channel.Name);
                if(args.Channel.Equals(this.Channel)) OnMessageReceived(this, args);
            };
        }

        // IConnector

        public event EventHandler<MessageEventArgs> OnMessageReceived;

        public void Send(string message) {
            this.Channel.SendMessage(message);
        }

    }

}
