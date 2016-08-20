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
            return context.SourceMessage.ConnectionProvider is IAudioCapability && context.HasArguments && (context.Args[0].Equals("list") || HasSound(context.Args[0])) ? ExecutionResult.Success : ExecutionResult.Fail;
        }

        public ExecutionResult Execute(CommandContext context) {
            if(context.Args[0].Equals("list")) {
                context.ResponseBuilder
                        .Block()
                            .Username()
                        .Block()
                        .With("here is a list of sounds I can play:")
                        .NewLine();

                foreach(Sound sound in Sounds) {
                    context.ResponseBuilder
                        .With(sound.Name + (sound.Alias.Length > 0 ? string.Format(" (alias: {0})", string.Join(", ", sound.Alias)) : string.Empty))
                        .NewLine();
                }

                context.ResponseBuilder.BuildAndSend();
            } else {
                PlaySound((IAudioCapability)context.SourceMessage.ConnectionProvider, context.Args[0], context.ResponseBuilder);
            }

            return ExecutionResult.Success;
        }

        // Logic

        private void PlaySound(IAudioCapability player, string name, ResponseBuilder builder = null) {
            Sounds.Find(s => s.Name.Equals(name) || s.Alias.Contains(name)).Play(player, builder);
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
                    Logger.Warn("Metadata file for {0} was not found, generating default ...", soundFile);
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