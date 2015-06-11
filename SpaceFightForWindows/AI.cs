#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using SpaceShooter.ElementsClass;
#endregion

namespace SpaceShooter
{
    /// <summary>
    /// Klasa dotycząca AI
    /// </summary>
    class AI
    {
        /// <summary>
        /// Przygotowuje AI do wykoanania odpowiednich ruchów
        /// </summary>
        /// <param name="player">Obiekt statku gracza</param>
        /// <param name="ships">Lista wszystkich statków</param>
        public static void PrepareAILogic(List<Ship> ships, Ship player)
        {
            Random rand = new Random();
            foreach (Ship ship in ships)
            {
                if (ship.ID != 1)
                {
                    // jeśli ai jest daleko od gracza
                    if (Physic.CountLength(player.vector, ship.vector) > 200)
                    {
                        if (ship.action != Constants.RUN)
                        {
                            if (ship.action != Constants.ATTACK_ENEMY)
                            {
                                PursuitEnemy(ship, player);
                            }
                            else
                            {
                                AttackEnemy(ship, player, rand);
                            }
                        }
                        else
                        {
                            if (ship.roadLength < 1)
                            {
                                PursuitEnemy(ship, player);
                            }
                        }
                    }
                    else
                    {
                        // jesli blisko
                        if (ship.action == Constants.PURSIUT_ENEMY)
                        {
                            if (rand.Next(AttackChance(ship)) == 0)
                            {
                                AttackEnemy(ship, player, rand);
                            }
                            else
                            {
                                GameLogic.AddMissile(ship, 200);
                                Run(ship, rand);
                            }
                        }
                        else if (ship.action == Constants.ATTACK_ENEMY)
                        {
                            AttackEnemy(ship, player, rand);
                        }
                        else if (ship.roadLength <= 0 || ship.action == Constants.RUN)
                        {
                            AttackEnemy(ship, player, rand);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Przygotowuje AI "gracza" do wykonania odpowiednich ruchów
        /// </summary>
        /// <param name="ships">Lista wszystkich statków</param>
        /// <param name="menuAI">Obiekt statku AI (czyli statek "gracza")</param>
        public static void PrepareMenuAILogic(List<Ship> ships, Ship menuAI)
        {
            Random rand = new Random();
            Ship nearestShip = null;
            double nearestLength = 0.0;
            double lastLength = 0.0;

            foreach (Ship ship in ships)
            {
                if (menuAI.ID != ship.ID)
                {
                    // wybiera najbliższy statek
                    lastLength = Physic.CountLength(ship.vector, menuAI.vector);
                    if (nearestLength > lastLength || nearestLength == 0.0)
                    {
                        nearestShip = ship;
                        nearestLength = lastLength;
                    }
                }
            }
            AttackEnemy(menuAI, nearestShip, rand, true);
        }
        
        /// <summary>
        /// Pościg za graczem
        /// </summary>
        /// <param name="AI">Obiekt statku AI</param>
        /// <param name="player">Obiekt statku gracza</param>
        private static void PursuitEnemy(Ship AI, Ship player)
        {
            AI.action = Constants.PURSIUT_ENEMY;
            Physic.PrepareShipMove(AI, player.vector.X, player.vector.Y);
        }

        /// <summary>
        /// Nieustanny na atak gracza
        /// </summary>
        /// <param name="AI">Obiekt statku AI</param>
        /// <param name="player">Obiekt statku gracza</param>
        /// <param name="rand">Obiekt liczb pseudolosowych</param>
        private static void AttackEnemy(Ship AI, Ship player, Random rand, bool menuMode = false)
        {
            AI.action = Constants.ATTACK_ENEMY;

            int randDigit = menuMode ? 15 : 50;
            if (rand.Next(randDigit) == 0)
            {
                GameLogic.AddMissile(AI, 200);
            }
            Physic.PrepareShipMove(AI, player.vector.X, player.vector.Y);
        }

        /// <summary>
        /// Ucieczka w losowym kierunku
        /// </summary>
        /// <param name="AI">Obiekt statku AI</param>
        /// <param name="rand">Obiekt liczb pseudolosowych</param>
        private static void Run(Ship AI, Random rand)
        {
            AI.action = Constants.RUN;

            int randomX = rand.Next(-400, 400);
            int randomY = rand.Next(-400, 400);
            Physic.PrepareShipMove(AI, AI.vector.X + randomX, AI.vector.Y + randomY);
        }

        /// <summary>
        /// Wylicza na podstawie właściwości i stanu statku możliwość ataku
        /// Czym większa zwrócona liczba tym mniejsza szansa
        /// </summary>
        /// <param name="AI"></param>
        /// <returns></returns>
        private static int AttackChance(Ship AI)
        {
            int chance = 0;
            // sprawdzanie ilości dział - czym więcej tym lepiej
            switch (AI.property.gunCount)
            {
                case 1:
                    chance += 4;
                    break;
                case 2:
                    chance += 2;
                    break;
                // dla trzech i więcej dział szansa się nie zmienia
            }

            // sprawdzanie tarcz ochronnych
            if (AI.property.shield <= 1)
            {
                chance++;
            }

            return chance;
        }
    }
}
