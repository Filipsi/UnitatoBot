using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitatoBot.Component.Audio;
using UnitatoBot.Connector;

namespace UnitatoBot.Command.Execution.Executors {

    class SoundExecutor : IInitializable, IExecutionHandler {

        private List<Sound> Sounds;

        // IInitializable

        public void Initialize(CommandManager manager) {
            if(!Directory.Exists("sounds"))
                Directory.CreateDirectory("sounds");

            Sounds = LoadSounds();
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Play sound effect in #general voice chat. Use with 'list' as first argument to get list of available sounds. Use with name of the sound as first argument to play it.";
        }

        public ExecutionResult CanExecute(CommandContext context) {
            return context.SourceMessage.ConnectionProvider is IAudioCapability && context.HasArguments && (context.Args[0].Equals("list") || HasSound(context.Args[0])) ? ExecutionResult.Success : ExecutionResult.Denied;
        }

        public ExecutionResult Execute(CommandContext context) {
            if(context.Args[0].Equals("list")) {
                ResponseBuilder builder = context.ResponseBuilder
                    .Username()
                    .With("there {0}", Sounds.Count > 1 ? "are" : "is")
                    .Block(Sounds.Count)
                    .With("sounds that you can play.");

                builder
                    .TableStart(25, "Name", "Alias");

                foreach(Sound sound in Sounds) {
                    builder
                        .TableRow(sound.Name, string.Join(",", sound.Alias));
                }

                builder
                    .TableEnd()
                    .BuildAndSend();
            } else {
                return PlaySound((IAudioCapability)context.SourceMessage.ConnectionProvider, context.SourceMessage, context.Args[0]) ? ExecutionResult.Success : ExecutionResult.Denied;
            }

            return ExecutionResult.Success;
        }

        // Logic

        private bool PlaySound(IAudioCapability player, ConnectionMessage requestMessage, string soundName) {
            return Sounds.Find(s => s.Name.Equals(soundName) || s.Alias.Contains(soundName)).Play(player, requestMessage);
        }

        private bool HasSound(string name) {
            return Sounds.Exists(s => s.Name.Equals(name) || s.Alias.Contains(name));
        }

        private List<Sound> LoadSounds() {
            Logger.SectionStart();

            List<Sound> list = new List<Sound>();
            foreach(FileInfo soundFile in new DirectoryInfo("sounds").GetFiles("*.mp3", SearchOption.TopDirectoryOnly)) {
                FileInfo metaFile = new FileInfo(Path.Combine("sounds", Path.GetFileNameWithoutExtension(soundFile.Name) + ".meta"));

                Sound sound;
                if(metaFile.Exists) {
                    sound = Sound.LoadFrom(metaFile);
                } else {
                    Logger.Warn("Metadata file for {0} not found, generating default ...", soundFile);
                    sound = new Sound(soundFile);
                    sound.Save();
                }

                list.Add(sound);
            }

            Logger.SectionEnd();
            Logger.Info("Loaded {0} file{1}", list.Count, list.Count == 1 ? string.Empty : "s");

            return list;
        }

    }

}