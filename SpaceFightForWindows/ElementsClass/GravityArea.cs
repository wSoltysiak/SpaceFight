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
    /// Klasa reprezuntująca pole grawiacyjne
    /// </summary>
    class GravityArea
    {
        // współrzędne
        public Vector2 vector;

        // siła grawitacji w centrum pola
        public double force;

        // nazwa pliku grafki
        public string name;

        /// <summary>
        /// Konstruktor klasy GravityArea
        /// </summary>
        /// <param name="_x">oś x</param>
        /// <param name="_y">oś y</param>
        /// <param name="_force">Siła grawitacji</param>
        /// <param name="_name">Nazwa grafiki</param>
        public GravityArea(int _x, int _y, double _force, string _name)
        {
            vector = new Vector2(_x, _y);
            force = _force;
            name = _name;
        }
    }
}
