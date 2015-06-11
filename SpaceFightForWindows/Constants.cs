#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace SpaceShooter
{
    /// <summary>
    /// Klasa przechowująca wszelkie stałe
    /// </summary>
    static class Constants
    {
        public const int viewAngle = 80;
        public const int maxTurboPower = 150;
        public const int maxShield = 15;
        public const int maxParticlesCount = 5000;
        public const double explosionSize = 0.7;
        public const double engineSize = 0.5;
        public const double otherSize = 0.8;

        public const int TUTORIAL_INIT = 0;

        public const int MOVE_SHIP = 1;
        public const int CREATE_MISSILE = 2;
        public const int TURBO_MODE = 3;
        public const int LEFT = 4;
        public const int RIGHT = 5;
        public const int FORWARD = 6;
        public const int RUSH = 7;
        public const int FORWARD_ECHO = 8;
        public const int NEXT_STEP = 9;

        public const int ATTACK_ENEMY = 101;
        public const int RUN = 102;
        public const int PURSIUT_ENEMY = 103;

        public const int NOT_DRAW = 201;
        public const int DRAW = 202;
        public const int RADAR_DRAW = 203;

        public const int START = 301;
        public const int CHOOSING = 302;
        public const int GAME = 303;
        public const int OPTIONS = 304;
        public const int CREDITS = 305;
        public const int TUTORIAL = 306;

        public const int CONTROL_MOUSE = 401;
        public const int CONTROL_MIXED = 402;
        public const int CONTROL_KEYBOARD = 403;

        public const int PARTICLE_DYNAMIC = 501;
        public const int PARTICLE_BACKGROUND = 502;
        public const int PARTICLE_GRAVITY_AREA = 503;
        public const int PARTICLE_EXPLOSION = 504;

        public const int EMPTY_WARNING = 601;
        public const int SHIELD_WARNING = 602;
        public const int GRAVITY_WARNING = 603;

        public const int MODE_NORMAL = 701;
        public const int MODE_HARD = 702;
        public const int MODE_TIME = 703;

        public const int SLOWDOWN = 801;
        public const int SPEEDUP = 802;
    }
}
