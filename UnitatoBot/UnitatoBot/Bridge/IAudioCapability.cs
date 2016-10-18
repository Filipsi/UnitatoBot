namespace UnitatoBot.Bridge {

    internal interface IAudioCapability {

        string[] GetAudioChannels(string origin);

        string GetUserAudioChannel(string origin, string user);

        bool PlayAudio(string origin, string channel, string file);

    }

}
