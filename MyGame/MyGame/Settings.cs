using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    class Settings
    {



        //public KeyboardState GetPressedKeys { get; }
        //public GamePadState GetPressedPad { get; }
        public float SFVolume { get; set; }
        public float BGVolume { get; set; }
        public Controls k = new Controls();

        public Keys[] keyset { get; set; }
        public enum KEYSET
        {
            KeyUp, KeyDown, KeyLeft, KeyRight, KeyFire, KeyWepForward, KeyWepBackward, KeyEsc
        }

        public Keys ShortcutKeys { get; set; }

        public float ChangeVolume
        {
            get { return SFVolume; }
        }

        public void SetupSettings()
        {
            BGVolume = k.BackgroundMusic;
            keyset[(int)KEYSET.KeyEsc] = k.KeyEsc;
            keyset[(int)KEYSET.KeyUp] = k.KeyUp;
            keyset[(int)KEYSET.KeyDown] = k.KeyDown;
            keyset[(int)KEYSET.KeyLeft] = k.KeyLeft;
            keyset[(int)KEYSET.KeyRight] = k.KeyRight;
            keyset[(int)KEYSET.KeyFire] = k.KeyFire;
            keyset[(int)KEYSET.KeyWepForward] = k.KeyWepForward;
            keyset[(int)KEYSET.KeyWepBackward] = k.KeyWepBackward;
        }        
        public float Vol { get; set; }

        public void SaveSettings()
        {
            Controls.Default.EffectNoise = Vol;
            Controls.Default.Save();
        }
    }
}
