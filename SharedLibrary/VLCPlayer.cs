using System;
using System.IO;
using System.Linq;
using System.Text;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;

namespace SharedLibrary
{
    public class VLCPlayer : IAudioPlayer
    {
        private Vlc.DotNet.Wpf.VlcControl vlcob;
        private string currentPath;

        public void Init()
        {
            //VlcContext.LibVlcDllsPath = Environment.CurrentDirectory;
            vlcob = new VlcControl();
            vlcob.PositionChanged += vlcob_PositionChanged;
        }
        void vlcob_PositionChanged(VlcControl sender, VlcEventArgs<float> e)
        {
            if (PositionChanged != null)
                PositionChanged(e.Data, sender.Duration);
        }

        public void SetSong(string filePath)
        {
            currentPath = filePath;
            PathMedia pm = new PathMedia(currentPath);
            vlcob.Media = pm;
            vlcob.Media.AddOption(
                ":sout=#transcode{vcodec=none,acodec=mp3,ab=128,channels=2,samplerate=44100}:http{mux=raw,dst=:8080/} :sout-keep");
        }

        public void Play()
        {
            vlcob.Play();
        }

        public void Stop()
        {
            vlcob.Stop();
        }

        public void Dispose()
        {
            vlcob.Dispose();
        }

        public TimeSpan Position
        {
            get
            {
                int time = (int)(vlcob.Position * vlcob.Duration.TotalMilliseconds);
                return TimeSpan.FromMilliseconds(time);
            }
        }

        public event PositionChangedEventHandler PositionChanged;
        public void DisposeMediaIfAny()
        {
            if (vlcob != null && vlcob.Media != null)
                vlcob.Media.Dispose();
        }

        public bool IsPlaying { get; set; }
    }
}