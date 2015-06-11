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
    /// Klasa opisująca cząsteczki
    /// </summary>
    class Particle
    {
        // wektor położenia cząsteczki
        public Vector2 vector;

        // prędkość poruszania się
        public double speed;

        // wielkość cząsteczki
        public double size;

        // maksymalna długość życia
        public int maxLife;

        // długość życia cząsteczki
        // 0 == koniec
        public double life;

        // kolor cząsteczki
        public List<Color> colorList;

        // ID generatora
        public int idSource;

        // typ czasteczki
        public int type;

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

        // wpływ pola grawitacyjnego
        public bool gravityAreaEffect;

        /// <summary>
        /// Konstruktor pojedynczej cząsteczki
        /// </summary>
        /// <param name="_idSource">ID generatora</param>
        /// <param name="_vector">Wektor rozpoczącia</param>
        /// <param name="_colorList">Lista kolorów</param>
        /// <param name="_life">Życie cząsteczki</param>
        /// <param name="radius">Kąt nachylenia</param>
        /// <param name="_speed">Prędkość</param>
        public Particle(int _idSource, int _type, Vector2 _vector, List<Color> _colorList, double _life, float radius, double _speed = 1.0, double _size = 1, bool _gravityAreaEffect = false)
        {
            idSource = _idSource;
            type = _type;
            vector = _vector;
            angle = radius;
            colorList = _colorList;
            maxLife = (int)_life;
            life = _life;
            speed = _speed;
            
            gravityAreaEffect = _gravityAreaEffect;
            size = SetRandomSize(_size);
        }

        private double SetRandomSize(double preferSize)
        {
            Random rand = GameLogic.rand;
            return rand.NextDouble() + preferSize;
        }
    }
}
