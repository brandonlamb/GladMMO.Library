﻿using System;
using Dissonance.Audio.Codecs;
using Dissonance.Audio.Codecs.Identity;
using Dissonance.Audio.Codecs.Opus;
using Dissonance.Config;

namespace Dissonance
{
    internal class CodecSettingsLoader
    {
        #region fields and properties
        private static readonly Log Log = Logs.Create(LogCategory.Core, typeof(CodecSettingsLoader).Name);

        private bool _started;

        private bool _settingsReady;
        private readonly object _settingsWriteLock = new object();

        private CodecSettings _config;

        private AudioQuality _encoderQuality;
        private FrameSize _encoderFrameSize;
        private Codec _codec = Codec.Opus;
        private bool _encodeFec;

        public CodecSettings Config
        {
            get
            {
                Generate();
                return _config;
            }
        }
        #endregion

        public void Start(Codec codec = Codec.Opus)
        {
            //Save encoder settings to ensure we use the same settings every time it is restarted
            _codec = codec;
            _encoderQuality = VoiceSettings.Instance.Quality;
            _encoderFrameSize = VoiceSettings.Instance.FrameSize;
            _encodeFec = VoiceSettings.Instance.ForwardErrorCorrection;
            _started = true;
        }

        private void Generate()
        {
            if (!_started)
                throw Log.CreatePossibleBugException("Attempted to use codec settings before codec settings loaded", "9D4F1C1E-9C09-424A-86F7-B633E71EF100");

            if (!_settingsReady)
            {
                lock (_settingsWriteLock)
                {
                    if (!_settingsReady)
                    {
                        //Create and destroy an encoder to determine the decoder settings to use
                        var encoder = CreateEncoder(_encoderQuality, _encoderFrameSize, _encodeFec);
                        _config = new CodecSettings(_codec, (uint)encoder.FrameSize, encoder.SampleRate);
                        encoder.Dispose();

                        _settingsReady = true;
                    }
                }
            }
        }

        [NotNull] private IVoiceEncoder CreateEncoder(AudioQuality quality, FrameSize frameSize, bool fec)
        {
            switch (_codec)
            {
                case Codec.Identity:
                    throw new NotSupportedException("We do not support anything but Opus.");
                    //return new IdentityEncoder(44100, 441);

                //ncrunch: no coverage start (Justification: We don't want to load the opus binaries into a testing context)
                case Codec.Opus:
                    return new OpusEncoder(quality, frameSize, fec);
                //ncrunch: no coverage end

                default:
                    throw Log.CreatePossibleBugException(string.Format("Unknown Codec {0}", _codec), "6232F4FA-6993-49F9-AA79-2DBCF982FD8C");
            }
        }

        [NotNull] public IVoiceEncoder CreateEncoder()
        {
            if (!_started)
                throw Log.CreatePossibleBugException("Attempted to use codec settings before codec settings loaded", "0BF71972-B96C-400B-B7D9-3E2AEE160470");

            return CreateEncoder(_encoderQuality, _encoderFrameSize, _encodeFec);
        }

        public override string ToString()
        {
            return Config.ToString();
        }
    }
}
