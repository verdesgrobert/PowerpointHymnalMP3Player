﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pptAddIn
{
    public interface IAudioPlayer
    {
        void Init();
        void SetSong(String filePath);
        void Play();
        void Stop();
        void Dispose();
        /// <summary>
        /// Player Position
        /// </summary>
        TimeSpan Position { get; set; }
        event PositionChangedEventHandler PositionChanged;
        void DisposeMediaIfAny();
        bool IsPlaying { get; set; }
    }
    public delegate void PositionChangedEventHandler(double position, TimeSpan duration);


}
