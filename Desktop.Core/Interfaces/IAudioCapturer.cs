using System;

namespace URemote.Desktop.Core.Interfaces
{
    public interface IAudioCapturer
    {
        event EventHandler<byte[]> AudioSampleReady;
        void ToggleAudio(bool toggleOn);
    }
}
