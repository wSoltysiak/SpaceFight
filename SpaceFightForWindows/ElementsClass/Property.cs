using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceShooter.ElementsClass
{
    /// <summary>
    /// Klasa dotycząca dokładnych właściwości statku
    /// </summary>
    class Property
    {
        // szybkość statku
        public double speed;

        // liczba dział
        public int gunCount;

        // moc osłony
        public int shield;

        // ilość punktów za zniszczenie statku -- 0 oznacza statek gracza
        public int points;

        /// <summary>
        /// Płytka kopia obiektu property
        /// Naprawia bug z zmianą "stałych" statystyk staku
        /// </summary>
        /// <param name="property">Obiekt do skopiowania</param>
        /// <returns></returns>
        public Object CloneProperty()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Konstruktor klasy Property
        /// Ustala właściwości konkretnych statków
        /// </summary>
        /// <param name="_speed">Szybkość bazowa</param>
        /// <param name="_gunCount">Ilość dział</param>
        /// <param name="_shield">Wytrzymałość osłony</param>
        /// <param name="_points">Liczba punktów za zestrzelenie</param>
        public Property(double _speed, int _gunCount, int _shield, int _points = 0)
        {
            speed = _speed;
            gunCount = _gunCount;
            shield = _shield;
            points = _points;
        }
    }
}
