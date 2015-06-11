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
    class GameMode
    {
        // maksymalna ilość przeciwników
        public int enemiesMax;

        // szansa na specjalnego przeciwnika
        public int extraEnemyChance;

        // minimalna siła pola grawitacyjnego
        public int gravityAreaMinPower;

        // maksymalna siła pola grawitacyjnego
        public int gravityAreaMaxPower;

        // dylatacja czasu
        public bool timeDilation;

        // highscore
        public int highscore;

        /// <summary>
        /// Konstruktor klasy GameMode
        /// </summary>
        /// <param name="_enemiesMax">maksymalna ilość przeciwników</param>
        /// <param name="_extraEnemyChance">szansa na specjalnego przeciwnika</param>
        /// <param name="_gravityAreaMaxPower">maksymalna siła pola grawitacyjnego</param>
        /// <param name="_timeDilation">dylatacja czasu</param>
        public GameMode(int _enemiesMax, int _extraEnemyChance, int _gravityAreaMinPower, int _gravityAreaMaxPower, bool _timeDilation)
        {
            enemiesMax = _enemiesMax;
            extraEnemyChance = _extraEnemyChance;
            gravityAreaMinPower = _gravityAreaMinPower;
            gravityAreaMaxPower = _gravityAreaMaxPower;
            timeDilation = _timeDilation;
        }
    }
}
