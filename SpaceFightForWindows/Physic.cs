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
    /// Klasa odpowiadająca za fizykę, głównie grawitacje
    /// </summary>
    class Physic
    {
        /// <summary>
        /// Przelicza radiany na stopnie
        /// </summary>
        /// <param name="radian">Kąt w radianach</param>
        /// <returns></returns>
        static public float ConvertRadianToDegrees(float radian)
        {
            return (float)(radian * 180 / Math.PI);
        }

        /// <summary>
        /// Przelicza stopnie na radiany
        /// </summary>
        /// <param name="degrees">Kąt w stopniach</param>
        /// <returns></returns>
        static public float ConvertDegreesToRadian(float degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        /// <summary>
        /// Obliczanie siły pocisku/statku na starcie
        /// </summary>
        /// <param name="xDifference">Rożnica między kliknięciami na osi X</param>
        /// <param name="yDifference">Różnica między kliknięciami na osi Y</param>
        /// <returns></returns>
        static public double CountPower(int xDifference, int yDifference) 
        {
            // do dopracowania
            double d = Math.Sqrt(Math.Pow(xDifference, 2) + Math.Pow(yDifference, 2));
            d = d > 100 ? 100 : d;
            return d;
        }

        /// <summary>
        /// Oblicza odległość od celu
        /// </summary>
        /// <param name="a">Cel</param>
        /// <param name="b">Start</param>
        /// <returns></returns>
        static public double CountLength(Vector2 a, Vector2 b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        /// <summary>
        /// Sprawdza, czy obiekt jest w polu widzenia gracza, jeśli nie wykrywać muszą go sensory statku
        /// </summary>
        /// <param name="observator">Położenie obserwatora</param>
        /// <param name="target">Położenie celu</param>
        /// <param name="viewAngle">Kąt pola widzenia</param>
        /// <param name="observatorAngle">Kierunek w który odwrócony jest obserwator</param>
        /// <returns></returns>
        static public bool InPurview(Vector2 observator, Vector2 target, float viewAngle, float observatorAngle)
        {
            float xDifference = observator.X - target.X;
            float yDifference = observator.Y - target.Y;

            float angle = Physic.ConvertRadianToDegrees((float)Math.Atan2(yDifference, xDifference)) - 90;
            observatorAngle = Physic.ConvertRadianToDegrees(observatorAngle);

            if (angle > observatorAngle - viewAngle / 2 && angle < observatorAngle + viewAngle / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sprawdza, czy sensory wyłapują jeszcze obiekt
        /// </summary>
        /// <param name="a">Położenie obiektu</param>
        /// <param name="gravityArea">Pole grawitacyjne</param>
        /// <returns></returns>
        static public bool InGravityArea(Vector2 a, GravityArea gravityArea)
        {
            Vector2 gravityAreaGraphic = Controler.TransferGraphicInfo(gravityArea.name);
            Vector2 gravityAreaVect = new Vector2(gravityArea.vector.X + gravityAreaGraphic.X / 2, gravityArea.vector.Y + gravityAreaGraphic.Y / 2);

            double d = CountLength(a, gravityAreaVect);
            double gravityPower = gravityArea.force / d;

            if (gravityPower > 0.4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Ustala dylatacje czasu
        /// </summary>
        /// <param name="vect">Wektor obiektu, którego dotyczy dylatacja</param>
        /// <param name="gravityAreas">Lista pól grawitacyjnych</param>
        /// <returns>Siła wpływu dylatacji czasu</returns>
        static public double TimeDilationEffect(Vector2 vect, List<GravityArea> gravityAreas)
        {
            double allGravityForce = 0.0;
            double d = 0.0;
            Vector2 gravityAreaGraphic, gravityAreaVect;
            foreach (GravityArea gravityArea in gravityAreas)
            {
                gravityAreaGraphic = Controler.TransferGraphicInfo(gravityArea.name);
                gravityAreaVect = new Vector2(gravityArea.vector.X + gravityAreaGraphic.X / 2, gravityArea.vector.Y + gravityAreaGraphic.Y / 2);
                d = CountLength(vect, gravityAreaVect);

                allGravityForce += gravityArea.force / d;
            }

            return allGravityForce;
        }

        /// <summary>
        /// Porównuje zmienne dotyczące dylatacji czasu
        /// </summary>
        /// <param name="playerDilation">Dylatacja gracza</param>
        /// <param name="antoherDilation">Dylatacja innego obiektu</param>
        /// <returns>tab[0] - prefix; tab[1] - wartość</returns>
        static public double[] CompareDilationEffect(double playerDilation, double antoherDilation)
        {
            double[] returnTab = new double[2];
            if (antoherDilation > playerDilation)
            {
                returnTab[0] = Constants.SLOWDOWN;
                returnTab[1] = antoherDilation - playerDilation;
            }
            else if (antoherDilation < playerDilation)
            {
                returnTab[0] = Constants.SPEEDUP;
                returnTab[1] = playerDilation -  antoherDilation;
            }
            else
            {
                returnTab[0] = returnTab[1] = 0.0;
            }

            return returnTab;
        }

        /// <summary>
        /// Sprawdza tablice z danymi o dylatacji i zmienia szybkość
        /// </summary>
        /// <param name="speed">Standardowa szybkość statku</param>
        /// <param name="dilationTab">tablica z danymi o dylatacji</param>
        /// <returns>Zmieniona szybkość</returns>
        static public double CheckDilation(double speed, double[] dilationTab)
        {
            if (dilationTab != null)
            {
                switch (Convert.ToInt32(dilationTab[0]))
                {
                    case Constants.SLOWDOWN:
                        speed -= dilationTab[1] * 3;
                        if (speed < 0.3)
                        {
                            speed = 0.1;
                        }
                        break;
                    case Constants.SPEEDUP:
                        speed += dilationTab[1] * 3;
                        if (speed > 10)
                        {
                            speed = 10;
                        }
                        break;
                }
            }

            return speed;
        }

        /// <summary>
        /// Zmienia wektor pod wpływem pola grawiatcyjnego
        /// </summary>
        /// <param name="vector">Wektor obiektu</param>
        /// <param name="gravityArea">Obiekt GravityArea</param>
        /// <param name="type">Typ obiektu</param>
        static public void InGravityAreaChangeVector(ref Vector2 vector, GravityArea gravityArea, string type, double[] dilationTab = null)
        {
            Vector2 gravityAreaGraphic = Controler.TransferGraphicInfo(gravityArea.name);
            Vector2 gravityAreaVect = new Vector2(gravityArea.vector.X + gravityAreaGraphic.X / 2, gravityArea.vector.Y + gravityAreaGraphic.Y / 2);

            double speed = 1;
            double lengthToGravityArea = CountLength(vector, gravityAreaVect);

            float xDifference = vector.X - gravityAreaVect.X;
            float yDifference = vector.Y - gravityAreaVect.Y;

            double gravityPower = 0;
            switch (type)
            {
                case "ship":
                    gravityPower = gravityArea.force / (lengthToGravityArea * 0.7);
                    break;
                case "missile":
                    gravityPower = gravityArea.force / (lengthToGravityArea * 0.9);
                    break;
                case "particle":
                    gravityPower = gravityArea.force / lengthToGravityArea / 2;
                    break;
            }

            float new_angle = (float)Math.Atan2(yDifference, xDifference) - 90;

            speed = CheckDilation(speed, dilationTab);

            vector.X += (float)(gravityPower * Math.Cos(new_angle) * speed);
            vector.Y += (float)(gravityPower * Math.Sin(new_angle) * speed);
        }

        /// <summary>
        /// Zmienia wektor o podane dane. Warunki zmiany zawarte w GameLogic.
        /// </summary>
        /// <param name="missile">Obiekt typu Missile</param>
        static public void ChangeVector(Missile missile, double[] dilationTab = null)
        {
            double speed = 1.0;

            double sin = Math.Sin(missile.mAngle);
            double cos = Math.Cos(missile.mAngle);

            double newX = 0 * cos - 1 * sin;
            double newY = 0 * sin + 1 * cos;

            speed = CheckDilation(speed, dilationTab);

            missile.vector.X -= (float)(newX * speed);
            missile.vector.Y -= (float)(newY * speed);
        }

        /// <summary>
        /// Zmienia wektor o podane dane. Warunki zmiany zawarte w GameLogic.
        /// </summary>
        /// <param name="ship">Obiekt typu Ship</param>
        /// <param name="notRoadLength">Ustala, czy z obiektu statku odejmowana jest przybyta droga</param>
        /// <param name="echo">Okresla, czy ruch statku jest efektem hamowania</param>
        static public void ChangeVector(Ship ship, double[] dilationTab = null, bool notRoadLength = false, bool echo = false, bool player = false)
        {
            
            double sin = Math.Sin(ship.mAngle);
            double cos = Math.Cos(ship.mAngle);

            double newX = 0 * cos - 1 * sin;
            double newY = 0 * sin + 1 * cos;

            double speed = 0;

            if (ship.turboMode)
            {
                speed = ship.property.speed + 2;
            }
            else
            {
                speed = ship.property.speed;
            }

            if (echo)
            {
                speed /= 2;
            }

            speed = CheckDilation(speed, dilationTab);

            ship.vector.X -= (float)(newX * speed);
            ship.vector.Y -= (float)(newY * speed);

            if (!notRoadLength)
            {
                ship.roadLength -= 1 * speed;
            }
        }

        /// <summary>
        /// Zmienia wektor o podane dane. Warunki zmiany zawarte w GameLogic.
        /// </summary>
        /// <param name="particle">Obiekt typu Particle</param>
        static public void ChangeVector(Particle particle, double[] dilationTab = null)
        {
            double sin = Math.Sin(particle.mAngle);
            double cos = Math.Cos(particle.mAngle);

            double speed = 1;

            double newX = 0 * cos - 1 * sin;
            double newY = 0 * sin + 1 * cos;

            speed = CheckDilation(speed, dilationTab);

            particle.vector.X -= (float)(newX * particle.speed * speed);
            particle.vector.Y -= (float)(newY * particle.speed * speed);
            particle.life -= speed;
        }

        /// <summary>
        /// Zmienia wektor o podane dane, służy do zmiany położenia pocisków względem statku.
        /// </summary>
        /// <param name="vector">Referencja wektoru</param>
        /// <param name="angle">Kąt w stopniach</param>
        /// <param name="iteration">Ilość iteracji</param>
        static public void SetNewCorrectVector(ref Vector2 vector, float angle, int iteration = 1) 
        {
            angle = ConvertDegreesToRadian(angle);

            for (int i = 0; i < iteration; i++)
            {
                double sin = Math.Sin(angle);
                double cos = Math.Cos(angle);

                double newX = 0 * cos - 1 * sin;
                double newY = 0 * sin + 1 * cos;

                vector.X -= (float)(newX);
                vector.Y -= (float)(newY);
            }
        }

        /// <summary>
        /// Ustala długość drogi i jej kąt
        /// </summary>
        /// <param name="ship">Obiekt Ship</param>
        /// <param name="x">oś x kliknięcia myszki</param>
        /// <param name="y">oś y klienięcia myszki</param>
        static public void PrepareShipMove(Ship ship, double x, double y)
        {
            double xDifference = ship.vector.X - x;
            double yDifference = ship.vector.Y - y;

            double newRoadLenght = Math.Sqrt(Math.Pow(xDifference, 2) + Math.Pow(yDifference, 2));
            if (newRoadLenght > 6)
            {
                ship.roadLength = newRoadLenght;
                float new_angle = (float)Math.Atan2(yDifference, xDifference);
                new_angle = ConvertRadianToDegrees(new_angle);
                ship.mAngle = new_angle - 90;
            }
        }
    }
}
