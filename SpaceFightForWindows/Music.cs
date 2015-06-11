using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using SpaceShooter.ElementsClass;
using SpaceShooter.Tools;

namespace SpaceShooter
{
    class Music
    {
        // zmienne przechowujące dźwięki i muzyke
        private static Dictionary<string, SoundEffect> musicObject = new Dictionary<string, SoundEffect>();
        public static Dictionary<string, SoundEffectInstance> musicIstances = new Dictionary<string, SoundEffectInstance>();
        private static Dictionary<string, SoundEffect> soundsObject = new Dictionary<string, SoundEffect>();

        /// <summary>
        /// Ładuje tła muzyczne
        /// </summary>
        /// <param name="Content"></param>
        public static void LoadMusic(ContentManager Content)
        {
            musicObject.Add("menuMusic", Content.Load<SoundEffect>("music/menuMusic"));
            musicObject.Add("gameMusic", Content.Load<SoundEffect>("music/gameMusic"));

            musicIstances.Add("menuMusic", musicObject["menuMusic"].CreateInstance());
            musicIstances.Add("gameMusic", musicObject["gameMusic"].CreateInstance());

            musicIstances["menuMusic"].IsLooped = true;
            musicIstances["menuMusic"].Volume = 1.0f;
            musicIstances["gameMusic"].IsLooped = true;
            musicIstances["gameMusic"].Volume = 1.0f;
        }

        /// <summary>
        /// Ładuje dźwięki
        /// </summary>
        /// <param name="Content"></param>
        public static void LoadSounds(ContentManager Content)
        {
            soundsObject.Add("explosion", Content.Load<SoundEffect>("sounds/explosion"));
            soundsObject.Add("shoot", Content.Load<SoundEffect>("sounds/shoot"));
        }

        /// <summary>
        /// Odgrywa tło muzyczne
        /// </summary>
        /// <param name="play">Okresla, czy muzyka nie jest wyciszona</param>
        /// <param name="actuallyScreen">Aktualny ekran</param>
        public static void PlayMusic(bool play, int actuallyScreen)
        {
            if (play)
            {
                if (actuallyScreen == Constants.GAME || actuallyScreen == Constants.TUTORIAL)
                {
                    if (musicIstances["gameMusic"].State == SoundState.Stopped)
                    {
                        musicIstances["menuMusic"].Stop();
                        musicIstances["gameMusic"].Pan = 0.0f;
                        musicIstances["gameMusic"].Play();
                    }
                }
                else
                {
                    if (musicIstances["menuMusic"].State == SoundState.Stopped)
                    {
                        musicIstances["gameMusic"].Stop();
                        musicIstances["menuMusic"].Pan = 0.0f;
                        musicIstances["menuMusic"].Play();
                    }
                }
            }
            else
            {
                musicIstances["menuMusic"].Stop();
                musicIstances["gameMusic"].Stop();
            }
        }

        /// <summary>
        /// Odtwarza dźwięki
        /// </summary>
        /// <param name="play">Określa czy dźwięki nie są wyciszone</param>
        /// <param name="soundName">Nazwa dźwięku do odgrania</param>
        /// <param name="actuallyScreen">Aktualny ekran</param>
        public static void PlaySound(bool play, string soundName, int actuallyScreen)
        {
            if (play)
            {
                if (actuallyScreen == Constants.GAME)
                {
                    soundsObject[soundName].Play(1.0f, 0.0f, 0.0f);
                }
                else
                {
                    soundsObject[soundName].Play(1.0f, 0.0f, 0.0f);
                }
            }
        }
    }
}
