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
    /// Klasa reprezentująca statki
    /// </summary>
    class Ship
    {
        // ID
        public int ID;

        // współrzedne statku
        public Vector2 vector;

        // nazwa klasy statku
        public string name;

        // kąt ustawienia
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

        // dlugość drogi do przebycia
        public double roadLength = 0;

        // odpowiada za przyspieszenie
        public bool turboMode = false;
        public double turboPower = Constants.maxTurboPower;

        // zmienna odpowiadająca za rysowanie
        public int toDraw = Constants.DRAW;

        // zmienna dotycząca AI
        public int action = Constants.PURSIUT_ENEMY;      
  
        // zmienna dotycząca AI - specjalny statek
        public bool extras;
        public int blink;

        // zmienna dodające specyfikacje statku
        public Property property;

        /// <summary>
        /// Konstruktor klasy Ship
        /// </summary>
        /// <param name="_ID">ID statku</param>
        /// <param name="_x">oś x</param>
        /// <param name="_y">oś y</param>
        /// <param name="_name">nazwa statku wg. dodanych grafik</param>
        /// <param name="radius">Kąt nachylenia</param>
        /// <param name="_property">Specyfikajca statku</param>
        /// <param name="_extras">Definiuje statek jako specjalny, bonusowy</param>
        public Ship(int _ID, float _x, float _y, string _name, float radius, Property _property, bool _extras = false)
        {
            // przypisanie początkowych danych
            ID = _ID;
            vector = new Vector2(_x, _y);
            name = _name;
            mAngle = radius;
            property = (Property)_property.CloneProperty();
            extras = _extras;
            blink = 0;
        }
    }
}