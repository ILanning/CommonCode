using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace CommonCode
{
    public class AudioManager : GameComponent
    {
        private static Dictionary<string, Cue> cues;
        private static Dictionary<string, SoundEffect> sounds;
        private static Dictionary<string, SoundEffectInstance> soundInstances;

        private static AudioEngine audioEngine;
        private static WaveBank waveBank;
        private static SoundBank soundBank;

        static AudioManager()
        {
            cues = new Dictionary<string, Cue>();
        }

        public AudioManager(AudioEngine engine, WaveBank waves, SoundBank sounds, Game game) : base(game)
        {
            audioEngine = engine;
            soundBank = sounds;
            waveBank = waves;
        }

        static void AddSound(string name, SoundEffect sound)
        {
            if (!sounds.ContainsKey(name))
                sounds.Add(name, sound);
        }

        static void PlaySound(string name, SoundEffect sound)
        {
            if (sounds.ContainsKey(name))
                sounds[name].Play();
        }

        public static void PlayCue(string name)
        {
            soundBank.PlayCue(name);
        }

        public static Cue GetCue(string name)
        {
            if (!cues.ContainsKey(name))
            {
                Cue cue = soundBank.GetCue(name);
                cues.Add(name, cue);
                return cue;
            }
            else
                return cues[name];
        }

        public static void PauseCue(string name)
        {
            if (cues.ContainsKey(name) && cues[name].IsPlaying && !cues[name].IsPaused)
                cues[name].Pause();
        }

        public static void StopCue(string name)
        {
            if (cues.ContainsKey(name) && cues[name].IsPlaying)
                cues[name].Stop(AudioStopOptions.Immediate);
        }

        public static void ResumeCue(string name)
        {
            if (cues[name].IsPaused)
                cues[name].Resume();
            else if(cues[name].IsPlaying)
                cues[name].Play();
        }

        public static bool IsCuePaused(string name)
        {
            if (cues.ContainsKey(name))
                return cues[name].IsPaused;
            return false;
        }

        public static bool IsCuePlaying(string name)
        {
            if (cues.ContainsKey(name))
                return cues[name].IsPlaying;
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            audioEngine.Update();
        }
    }
}
