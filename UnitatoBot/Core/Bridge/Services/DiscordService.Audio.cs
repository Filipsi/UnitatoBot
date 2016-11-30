using System.Linq;
using Discord;
using Discord.Audio;
using Discord.WebSocket;

namespace BotCore.Bridge.Services {

    public partial class DiscordService {

        // IAudioCapability

        /*
        public string[] GetAudioChannels(string origin) {
            ITextChannel channel = _client.GetChannel(ulong.Parse(origin)) as ITextChannel;
            return channel?.Guild.GetVoiceChannelsAsync().GetAwaiter().GetResult().Select(x => x.Name).ToArray() ?? new string[0];
        }

        public string GetUserAudioChannel(string origin, string user) {
            ITextChannel channel = _client.GetChannel(ulong.Parse(origin)) as ITextChannel;
            IGuildUser gUser = channel?.Guild.GetUserAsync(ulong.Parse(user)).GetAwaiter().GetResult();
            return gUser?.VoiceChannel.Name;
        }

        public bool PlayAudio(string origin, string channel, string file) {
            // Not implemented yet due to discord.net 1.0 
            return false;
        }
        */

    }

}
