using System;
using System.IO;
using System.Timers;
using BotCore.Bridge;
using BotCore.Util.Symbol;
using NAudio.Wave;
using Newtonsoft.Json;

namespace UnitatoBot.Component.Audio {

    [JsonObject(MemberSerialization.OptIn)]
    internal class Sound {

        public string   Name        { get; }
        public FileInfo SourceFile  { get; }
        public FileInfo MetaFile    { get; }

        [JsonProperty]
        public string[] Alias { private set; get; }

        private TimeSpan _length = TimeSpan.Zero;
        public TimeSpan Length {
            get {
                if(_length == TimeSpan.Zero)
                    using(Mp3FileReader mp3Reader = new Mp3FileReader(SourceFile.FullName))
                        _length = mp3Reader.TotalTime;

                return _length;
            }
        }

        public Sound(string name, string path) {
            Name = name;
            SourceFile = new FileInfo(Path.Combine(path, Name + ".mp3"));
            MetaFile = new FileInfo(Path.Combine(path, Name + ".meta"));
            Alias = new string[0];
        }

        public Sound(FileInfo source) {
            SourceFile = source;
            Name = Path.GetFileNameWithoutExtension(SourceFile.Name);
            MetaFile = new FileInfo(Path.Combine(source.DirectoryName, Name + ".meta"));
            Alias = new string[0];
        }

        public bool Play(IAudioCapability player, ServiceMessage requestMessage, string channel) {
            if(channel == null || !player.PlayAudio(requestMessage.Origin, channel, SourceFile.FullName))
                return false;

            ResponseBuilder builder = new ResponseBuilder(requestMessage);

            ServiceMessage msg = builder
                .Text(Emoji.Note)
                .Username()
                .Text("is playing sound")
                .Block(Name)
                .Text("with length")
                .Block(Length.ToString("mm':'ss"))
                .Text("on channel")
                .Block(channel)
                .Send();

            Timer t = new Timer();
            t.Interval = Length.TotalMilliseconds + 1000;
            t.Elapsed += (sender, args) => {
                builder
                    .Clear()
                    .Text(Emoji.Note)
                    .Username()
                    .Text("played sound")
                    .Block(Name)
                    .Text("on channel")
                    .Block(channel);

                msg.Edit(builder.Build());
                t.Dispose();
            };
            t.Start();

            return true;
        }

        public void Save() {
            using(StreamWriter writer = MetaFile.CreateText())
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Sound LoadFrom(FileInfo metaFile) {
            Sound sound = new Sound(Path.GetFileNameWithoutExtension(metaFile.Name), metaFile.DirectoryName);
            using(StreamReader reader = metaFile.OpenText())
                JsonConvert.PopulateObject(reader.ReadToEnd(), sound);
            
            return sound;
        }

        public override string ToString() {
            return Name;
        }

    }

}
