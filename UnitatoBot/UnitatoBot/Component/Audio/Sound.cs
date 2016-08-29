using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Timers;
using UnitatoBot.Command;
using UnitatoBot.Connector;

namespace UnitatoBot.Component.Audio {

    [JsonObject(MemberSerialization.OptIn)]
    internal class Sound {

        private static JsonSerializer SERIALIZER;

        static Sound() {
            SERIALIZER = new JsonSerializer();
            SERIALIZER.Formatting = Formatting.Indented;
        }

        public string   Name    { private set; get; }
        public string   Source  { private set; get; }
        public TimeSpan Length  { private set; get; }

        [JsonProperty]
        public string[] Alias { private set; get; }

        public Sound(FileInfo soundFile) : base() {
            Name = Path.GetFileNameWithoutExtension(soundFile.Name);
            Source = soundFile.FullName;
            Alias = new string[0];

            using(var MP3Reader = new Mp3FileReader(Source)) {
                Length = MP3Reader.TotalTime;
            }
        }

        public bool Play(IAudioCapability player, ConnectionMessage requestMessage) {
            bool playing = player.SendAudio(requestMessage.Origin, Source);

            if(playing) {
                ResponseBuilder builder = new ResponseBuilder(requestMessage);

                ConnectionMessage msg = builder
                    .With(SymbolFactory.Emoji.Note)
                    .Username()
                    .With("is playing sound")
                    .Block(Name)
                    .With("with length")
                    .Block(Length.ToString("mm':'ss"))
                    .Send();

                Timer t = new Timer();
                t.Interval = Length.TotalMilliseconds + 1000;
                t.Elapsed += (sender, args) => {
                    builder
                        .Clear()
                        .With(SymbolFactory.Emoji.Note)
                        .Username()
                        .With("played sound")
                        .Block(Name);

                    msg.Edit(builder.Build());
                    t.Dispose();
                };
                t.Start();
            }

            return playing;
        }

        public void Save() {
            using(StreamWriter writer = GetMetadataFile(Source).CreateText()) {
                SERIALIZER.Serialize(writer, this);
            }
        }

        public static Sound LoadFrom(FileInfo metaFile) {
            string data = "{}";
            using(StreamReader reader = metaFile.OpenText()) {
                data = reader.ReadToEnd();
            }

            Sound sound = new Sound(GetSoundFile(metaFile.FullName));
            JsonConvert.PopulateObject(data, sound);
            return sound;
        }

        public static FileInfo GetMetadataFile(string soundFile) {
            return new FileInfo(Path.Combine("sounds", Path.GetFileNameWithoutExtension(soundFile) + ".meta"));
        }

        public static FileInfo GetSoundFile(string soundFile) {
            return new FileInfo(Path.Combine("sounds", Path.GetFileNameWithoutExtension(soundFile) + ".mp3"));
        }

        public override string ToString() {
            return Name;
        }

    }

}
