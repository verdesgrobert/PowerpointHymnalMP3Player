using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;

namespace SharedLibrary
{
    public class NAudioPlayer : IAudioPlayer
    {
        WaveOut waveOutDevice;
        AudioFileReader audioFileReader;

        public NAudioPlayer()
        {
            Thread thTimer = new Thread(PositionChanedMonitor);
            thTimer.Start();
        }

        private void PositionChanedMonitor()
        {
            while (true)
            {
                Thread.Sleep(500);
                if (waveOutDevice != null && audioFileReader != null)
                    if (_lastPosition != waveOutDevice.GetPositionTimeSpan())
                        if (PositionChanged != null)
                            PositionChanged(waveOutDevice.GetPositionTimeSpan().TotalMilliseconds / audioFileReader.TotalTime.TotalMilliseconds, audioFileReader.TotalTime);
            }
        }

        public void Init()
        {
            waveOutDevice = new WaveOut();
        }

        public void SetSong(string filePath)
        {
            audioFileReader = new AudioFileReader(filePath);
            waveOutDevice.Init(audioFileReader);
        }

        public void Play()
        {
            waveOutDevice.Play();
        }

        public void Stop()
        {
            waveOutDevice.Stop();
        }

        public void Dispose()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
            }
            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }
            if (waveOutDevice != null)
            {
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }
        }

        private TimeSpan _lastPosition;
        public TimeSpan Position
        {
            get
            {
                return _lastPosition = waveOutDevice.GetPositionTimeSpan();
            }
            set
            {

            }
        }

        public event PositionChangedEventHandler PositionChanged;

        public void DisposeMediaIfAny()
        {
            if (audioFileReader != null)
                audioFileReader.Dispose();
        }

        public bool IsPlaying
        {
            get
            {
                return waveOutDevice.PlaybackState == PlaybackState.Playing;
            }
            set
            {

            }
        }
    }
}
