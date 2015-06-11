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
    // źródło cząsteczek
    class ParticleSource
    {
        // umieszczenie źródła cząsteczek
        public Vector2 vector;

        // kierunek rozrzutu
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

        // odchylenie rozrzutu (maksymalne wychylenie)
        public float angleInclination;
        public float mAngleInclination
        {
            get
            {
                return angleInclination;
            }
            set
            {
                angleInclination = (float)(value * Math.PI / 180);
            }
        }

        // kolor cząsteczki
        public List<Color> colorList;

        // minimalna prędkość cząsteczek
        public double minSpeed;
        // maksymalna prędkość czasteczek
        public double maxSpeed;

        // uśpienie źródła
        public bool sleeping;

        // unikale ID generatora
        public int ID;

        // typ generatora
        public int type;

        // ilość wysyłanych cząsteczek
        public int power;

        // życie cząsteczki (bez dodatku losowego)
        public int life;

        // ilość aktualnie utowrzonych cząsteczek
        public int actuallyCreated;

        // określa, czy na cząsteczke ma wpływać pole grawitacyjne
        public bool gravityAreaEffect;

        /// <summary>
        /// Źródło tworzenia cząsteczek 
        /// Wersja z tworzeniem wektora
        /// </summary>
        /// <param name="x">Oś x</param>
        /// <param name="y">Oś y</param>
        /// <param name="radius">Kąt wyrzucania cząstek</param>
        /// <param name="radiusInclination">Maksymalny odchył</param>
        /// <param name="_power">Ilość wysyłanych cząsteczek</param>
        /// <param name="_life">Życie cząsteczki bez dodatku losowego</param>
        /// <param name="_colorList">Lista kolorów dla cząsteczek</param>
        /// <param name="_minSpeed">Minimalna prędkość cząsteczki</param>
        /// <param name="_maxSpeed">Maksymalna prędkość cząsteczki</param>
        /// <param name="_sleeping">Uśpienie źródła</param>
        /// <param name="_gravityAreaEffect">Ustala, czy na cząsteczki wpływa pole grawitacyjne</param>
        /// <param name="_ID">ID generatora (poniżej 1000 zmienny, powyżej stały)</param>
        public ParticleSource(int _ID, int _type, int x, int y, float radius, float radiusInclination, int _power, int _life, List<Color> _colorList, bool _sleeping = false, bool _gravityAreaEffect = false, double _minSpeed = 0.1, double _maxSpeed = 1.0)
        {
            ID = _ID;
            type = _type;
            vector = new Vector2(x, y);
            mAngle = radius;
            mAngleInclination = radiusInclination;
            power = _power;
            life = _life;
            colorList = _colorList;
            minSpeed = _minSpeed;
            maxSpeed = _maxSpeed;
            sleeping = _sleeping;
            gravityAreaEffect = _gravityAreaEffect;
        }

        /// <summary>
        /// Źródło towrzenia cząsteczek
        /// Wersja z gotowym wektorem
        /// </summary>
        /// <param name="_vector">Wektor poruszającego się źródła</param>
        /// <param name="radius">Kąt wyrzucania cząstek</param>
        /// <param name="radiusInclination">Maksymalny odchył</param>
        /// <param name="_power">Ilość wysyłanych czasteczek</param>
        /// <param name="_life">Życie cząsteczki bez dodatku losowego</param>
        /// <param name="_colorList">Lista kolorów dla cząsteczek</param>
        /// <param name="_minSpeed">Minimalna prędkoścć cząsteczki</param>
        /// <param name="_maxSpeed">Maksymalna prędkość cząsteczki</param>
        /// <param name="_sleeping">Uśpienie źródła</param>
        /// <param name="_gravityAreaEffect">Ustala, czy na cząsteczki wpływa pole grawitacyjne</param>
        /// <param name="_ID">ID generatora (poniżej 1000 zmienny, powyżej stały)</param>
        public ParticleSource(int _ID, int _type, Vector2 _vector, float radius, float radiusInclination,  int _power, int _life, List<Color> _colorList, bool _sleeping = false, bool _gravityAreaEffect = false, double _minSpeed = 0.1, double _maxSpeed = 1.0)
        {
            ID = _ID;
            type = _type;
            vector = _vector;
            mAngle = radius;
            mAngleInclination = radiusInclination;
            power = _power;
            life = _life;
            colorList = _colorList;
            minSpeed = _minSpeed;
            maxSpeed = _maxSpeed;
            sleeping = _sleeping;
            gravityAreaEffect = _gravityAreaEffect;
        }

        /// <summary>
        /// Usypianie źródła
        /// </summary>
        public void Sleep() {
            sleeping = true;
        }

        /// <summary>
        /// Wznowienie tworzenia cząsteczek
        /// </summary>
        public void Wake()
        {
            sleeping = false;
        }

    }
}