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

namespace SpaceShooter.ElementsClass
{
    /// <summary>
    /// Klasa reprezentująca rakiety
    /// </summary>
    class Missile
    {
        // współrzędne rakiety
        public Vector2 vector;

        // ID statku, który wystrzelił rakiete
        public int shipID;

        // aktualny kąt lotu
        public float angle;
        public float mAngle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = (float)(value * Math.PI / 180);
            }
        }

        // siła wystrzału - odpowiada za dlugość lotu i wyrywanie się z pola grawitacji
        public int power;

        // zmienna odpowiadająca za rysowanie
        public int toDraw = Constants.DRAW;

        // licznik pixeli do przesunięcia
        public double pixelCounter = 0;

        /// <summary>
        /// Konstrutkor klasy Missle
        /// </summary>
        /// <param name="_x">oś x</param>
        /// <param name="_y">oś y</param>
        /// <param name="_radius">kąt nachylenia lotu</param>
        /// <param name="_power">siła wystrzału</param>
        public Missile(float _x, float _y, float _radius, int _power, int _shipID)
        {
            vector = new Vector2(_x, _y);
            mAngle = _radius;
            power = _power;
            shipID = _shipID;
        }
    }
}
