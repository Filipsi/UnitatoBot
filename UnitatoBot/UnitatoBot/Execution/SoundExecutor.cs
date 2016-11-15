using System.Collections.Generic;
using System.IO;
using System.Linq;
using BotCore.Bridge;
using BotCore.Execution;
using BotCore.Util;
using UnitatoBot.Component.Audio;

namespace UnitatoBot.Execution {

    class SoundExecutor : IInitializable, IConditionalExecutonHandler {

        private List<Sound> _sounds;

        // IInitializable

        public void Initialize(ExecutionManager manager) {
            if(!Directory.Exists("sounds"))
                Directory.CreateDirectory("sounds");

            _sounds = LoadSounds();
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Play sound effect in #general voice chat. Use with 'list' as first argument to get list of available sounds. Use with name of the sound as first argument to play it. You can specifiy channel name as second argument (this is optional).";
        }

        public bool CanExecute(ExecutionContext context) {
            return context.Message.Service is IAudioCapability && context.HasArguments && (context.Args[0].Equals("list") || HasSound(context.Args[0]));
        }

        public bool Execute(ExecutionContext context) {
            if(context.Args[0].Equals("list")) {
                ResponseBuilder builder = context.ResponseBuilder
                    .Username()
                    .Text("there {0}", _sounds.Count > 1 ? "are" : "is")
                    .Block(_sounds.Count)
                    .Text("sounds that you can play.");

                builder.TableStart(25, "Name", "Alias", "Length");
                foreach(Sound sound in _sounds) {
                    if(builder.Length > 1900) {
                        builder.MultilineBlock()
                            .Send();
                        builder.Clear()
                            .KeepSourceMessage()
                            .MultilineBlock();
                    }
                    builder.TableRow(sound.Name, string.Join(",", sound.Alias), sound.Length.ToString("mm':'ss"));
                }
                builder
                    .TableEnd()
                    .Send();

            } else {
                return PlaySound((IAudioCapability)context.Message.Service, context, context.Args[0]);
            }

            return true;
        }

        // Logic

        private bool PlaySound(IAudioCapability player, ExecutionContext context, string soundName) {
            string channel = context.Args.Length == 2 ? context.Args[1] : player.GetUserAudioChannel(context.Message.Origin, context.Message.Sender);
            return _sounds.Find(s => s.Name.Equals(soundName) || s.Alias.Contains(soundName)).Play(player, context.Message, channel);
        }

        private bool HasSound(string name) {
            return _sounds.Exists(s => s.Name.Equals(name) || s.Alias.Contains(name));
        }

        private List<Sound> LoadSounds() {
            Logger.SectionStart();

            List<Sound> list = new List<Sound>();
            foreach(FileInfo soundFile in new DirectoryInfo("sounds").GetFiles("*.mp3", SearchOption.TopDirectoryOnly)) {
                FileInfo metaFile = new FileInfo(Path.Combine("sounds", Path.GetFileNameWithoutExtension(soundFile.Name) + ".meta"));
                list.Add(metaFile.Exists ? Sound.LoadFrom(metaFile) : new Sound(soundFile));
            }

            Logger.SectionEnd();
            Logger.Info("Loaded {0} file{1}", list.Count, list.Count == 1 ? string.Empty : "s");

            return list;
        }

    }

}