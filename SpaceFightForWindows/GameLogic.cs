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
using SpaceShooter.Tools;
#endregion

namespace SpaceShooter
{
    /// <summary>
    /// Klasa odpowiadająca za logikę gry, input itp.
    /// </summary>
    class GameLogic
    {
        // kolekcje trzymające obiekty
        static private List<Ship> shipsObject = new List<Ship>();
        static private List<Missile> missilesObject = new List<Missile>();
        static private List<GravityArea> gravityAreasObject = new List<GravityArea>();
        static private List<ParticleSource> particleSourcesObject = new List<ParticleSource>();
        static private Particle[] particlesObject = new Particle[Constants.maxParticlesCount];
        static private Dictionary<string, Property> propertyList = new Dictionary<string, Property>();

        // tryby gry
        static public Dictionary<int, GameMode> gameModes = new Dictionary<int, GameMode>(3);

        // ostatnie ID statków
        static private int lastID = 1;

        // ostatnie ID cząsteczek - 1000 zarezerwowane dla cząsteczek dynamicznych
        static private int lastParticleSourceID = 1000;

        // aktualny styl sterowania
        static private int controlType = Constants.CONTROL_MOUSE;
        static public int mControlType
        {
            get
            {
                return controlType;
            }

            set
            {
                controlType = value;
            }
        }

        // pokazywanie ostrzeżeń
        static private bool showingWarnings = true;
        static public bool mShowingWarnings
        {
            get
            {
                return showingWarnings;
            }

            set
            {
                showingWarnings = value;
            }
        }

        // dźwięki
        static private bool playSounds = true;
        static public bool mPlaySounds
        {
            get
            {
                return playSounds;
            }

            set
            {
                playSounds = value;
            }
        }

        static private bool playMusic = true;
        static public bool mPlayMusic
        {
            get
            {
                return playMusic;
            }

            set
            {
                playMusic = value;
            }
        }

        // tryb gry
        static private int gameMode = Constants.MODE_NORMAL;
        static public int mGameMode
        {
            get
            {
                return gameMode;
            }

            set
            {
                gameMode = value;
            }
        }

        // aktualna punktacja
        static private int actuallyPoints = 0;
        static public int mActuallyPoints
        {
            get
            {
                return actuallyPoints;
            }
            set
            {
                actuallyPoints = value;
            }
        }

        // aktualny mnożnik combosów
        static private int actuallyCombo = 1;
        static public int mActuallyCombo
        {
            get
            {
                return actuallyCombo;
            }
            set
            {
                actuallyCombo = value;
            }
        }

        // ilość strąceń do następnego combosa
        static private int toNextCombo = 0;

        // krok tutoriala
        static private int tutorialStep = Constants.TUTORIAL_INIT;
        static public int mTutorialStep
        {
            get
            {
                return tutorialStep;
            }
            set
            {
                tutorialStep = value;
            }
        }

        static private bool toNextTutorialStep = false;
        static public bool mToNextTutorialStep
        {
            get
            {
                return toNextTutorialStep;
            }
        }

        // powolne zatrzymywanie statku
        static private int forwardEcho = 0;

        // dane o highscore
        static public int mHighscore
        {
            get
            {
                return gameModes[gameMode].highscore;
            }
            set
            {
                gameModes[gameMode].highscore = value;
            }
        }

        // dane o dylatacji gracza
        static private double playerDilation;

        // zmienna losująca
        static public Random rand = new Random();

        // dane o kursorze
        static private int mouseX = 0;
        static private int mouseY = 0;
        static private int mouseXDifference = 0;
        static private int mouseYDifference = 0;

        // dane o kolorach do generatorów cząsteczek
        static private Dictionary<string, List<Color>> colors = new Dictionary<string,List<Color>>();


        // ##############################################
        // #### Metody zwracające konkretne wartości ####
        // ##############################################


        /// <summary>
        /// Zwraca całą listę statków
        /// </summary>
        /// <returns>Kolekcja statków</returns>
        static public List<Ship> GetShips
        {
            get
            {
                return shipsObject;
            }
        }

        /// <summary>
        /// Zwraca całą listę rakiet
        /// </summary>
        /// <returns>Kolekcja rakiet</returns>
        static public List<Missile> GetMissiles
        {
            get
            {
                return missilesObject;
            }
        }

        /// <summary>
        /// Zwraca całą listę pół grawitacyjnych
        /// </summary>
        static public List<GravityArea> GetGravityAreas
        {
            get
            {
                return gravityAreasObject;
            }
        }

        /// <summary>
        /// Zwraca liste cząsteczek
        /// </summary>
        static public Particle[] GetParticles
        {
            get
            {
                return particlesObject;
            }
        }

        /// <summary>
        /// Zwraca obiekt statku gracza
        /// </summary>
        /// <returns>Statek gracza</returns>
        static public Ship GetPlayerShip()
        {
            foreach (Ship ship in shipsObject)
            {
                if (ship.ID == 1)
                {
                    return ship;
                }
            }

            return null;
        }

        /// <summary>
        /// Zapisuje liste kolorów z XmlLoadera do ogólnego użytku
        /// </summary>
        static public void GetColors()
        {
            colors = XmlTool.LoadColors();
        }

        /// <summary>
        /// Zapisuje liste z XmlLoadera do ogólnego użytku
        /// </summary>
        static public void GetShipsProperty()
        {
            propertyList = XmlTool.LoadShipProperty();
        }

        /// <summary>
        /// Zapisuje ustawienia z XmlLoadera do ogólnego użytku
        /// </summary>
        static public void GetSettings()
        {
            Dictionary<string, int> settings = XmlTool.LoadSettings();
            gameModes[Constants.MODE_NORMAL].highscore = settings["highscoreNormal"];
            gameModes[Constants.MODE_HARD].highscore = settings["highscoreHard"];
            gameModes[Constants.MODE_TIME].highscore = settings["highscoreTime"];
            mControlType = settings["controlType"];
            mShowingWarnings = Convert.ToBoolean(settings["showingWarnings"]);
            mPlaySounds = Convert.ToBoolean(settings["playSounds"]);
            mPlayMusic = Convert.ToBoolean(settings["playMusic"]);
            mGameMode = settings["gameMode"];
        }

        /// <summary>
        /// Zwraca ostatnie wolne ID i powiększa licznik o jeden
        /// </summary>
        /// <returns>Wolne ID</returns>
        static private int GetLastParticleSourcesID()
        {
            lastParticleSourceID++;
            return lastParticleSourceID;
        }

        /// <summary>
        /// Zwraca ostatnie wolne ID i powiększa licznik o jeden
        /// </summary>
        /// <returns>Wolne ID</returns>
        static private int GetLastID()
        {
            if (lastID == 1000)
            {
                lastID = 1;
            }
            lastID++;
            return lastID;
        }


        // ##################################################
        // #### Metody tworzące, inicjujące i generujące ####
        // ##################################################

        /// <summary>
        /// Tworzy tryby gry
        /// </summary>
        static public void SetGameModes()
        {
            gameModes.Add(Constants.MODE_NORMAL, new GameMode(2, 5000, 40, 65, false));
            gameModes.Add(Constants.MODE_HARD, new GameMode(3, 3500, 40, 75, false));
            gameModes.Add(Constants.MODE_TIME, new GameMode(2, 6500, 30, 60, true));
        }

        /// <summary>
        /// Tworzenie statków początkowych
        /// Statek gracza zawsze jako pierwszy
        /// </summary>
        static public void CreateShips(int actuallyScreen, Ship chooseShip = null, int width = 0, int height = 0)
        {
            ClearTempVariables(actuallyScreen);
            // tworzenie statktów
            switch (actuallyScreen)
            {
                case Constants.START:
                    Ship goodAIShip = new Ship(1, GUI.mWidth / 2, GUI.mHeight / 2, "Pship3", 90f, propertyList["Pship3"]);
                    shipsObject.Add(goodAIShip);
                    particleSourcesObject.Add(new ParticleSource(goodAIShip.ID, Constants.PARTICLE_DYNAMIC, goodAIShip.vector, 240f, 40f, 40, 10, colors["engineColors"], false, false, 1.5, 1.5)); 
                    break;
                case Constants.CHOOSING:
                    Ship showShip;
                    string shipName;
                    float shipWidth, shipHeight;
                    int counter = -(height * 9 / 16);
                    int step = (height * 9 / 16) * 2 / 3;
                    for (int i = 1; i < 5; i++)
                    {
                        shipName = "Pship" + i.ToString();
                        shipWidth = (GUI.mWidth - Controler.TransferGraphicInfo(shipName).X) / 2 + counter;
                        shipHeight = (GUI.mHeight - Controler.TransferGraphicInfo(shipName).Y) / 2;
                        showShip = new Ship(i + 1, (int)shipWidth, (int)shipHeight, shipName, 90f, propertyList[shipName]);

                        shipsObject.Add(showShip);
                        particleSourcesObject.Add(new ParticleSource(i + 1, Constants.PARTICLE_DYNAMIC, showShip.vector, 240f, 40f, 40, 10, colors["engineColors"], true, false, 1.5, 1.5));

                        counter += step;
                    }
                    break;
                case Constants.GAME:
                case Constants.TUTORIAL:
                    Ship playerShip = new Ship(1, GUI.mWidth / 2, GUI.mHeight / 2, chooseShip.name, 90f, propertyList[chooseShip.name]);
                    shipsObject.Add(playerShip);
                    particleSourcesObject.Add(new ParticleSource(playerShip.ID, Constants.PARTICLE_DYNAMIC, playerShip.vector, 240f, 40f, 40, 10, colors["engineColors"], true, false, 1.5, 1.5));
                    break;
            }
        }

        /// <summary>
        /// Tworzy 3 losowo połozone pola grawitacyjne + generatory cząstek nad i pod polem
        /// </summary>
        /// <param name="actuallyScreen">Aktualny ekran (menu/gra)</param>
        /// <param name="width">Szerokość ekranu</param>
        /// <param name="height">Wysokość ekranu</param>
        static public void CreateGravityAreas(int actuallyScreen, int width, int height)
        {
            Random rand = new Random();
            GravityArea gravityArea;
            int randX, randY, randPower, x, y;
            float angle;
            int textureHeight = (int)Controler.TransferGraphicInfo("gravityArea1").Y;

            gravityAreasObject.Clear();
            for (int i = 0; i < 3; i++)
            {
                randX = rand.Next(0, width - 50);
                randY = rand.Next(0, height - 50);
                if (actuallyScreen == Constants.START)
                {
                    // niska wartość pola, żeby AI się nie zacinało
                    randPower = rand.Next(20, 40);
                }
                else
                {
                    // normalny tryb
                    randPower = rand.Next(gameModes[mGameMode].gravityAreaMinPower, gameModes[mGameMode].gravityAreaMaxPower);
                }
                gravityArea = new GravityArea(randX, randY, randPower, "gravityArea1");
                gravityAreasObject.Add(gravityArea);

                // dodawanie 24 generatorów cząsteczek na okręgu przy polu grawitacyjnym o losowej (300 - 700) jednostkach życia
                for (int j = 0; j < 24; j++)
                {
                    angle = 360f / 24 * j;
                    x = (int)(60 * Math.Cos(Physic.ConvertDegreesToRadian(angle - 90)) + randX + 64);
                    y = (int)(60 * Math.Sin(Physic.ConvertDegreesToRadian(angle - 90)) + randY + 64);
                    particleSourcesObject.Add(new ParticleSource(GetLastParticleSourcesID(), Constants.PARTICLE_GRAVITY_AREA, x, y, 0f, 360f, 4, rand.Next(300, 700), colors["gravityAreasColors"], false, true, 0.1, 0.2));
                }
            }
        }

        /// <summary>
        /// Tworzenie cząsteczek
        /// </summary>
        static public void CreateParticle()
        {
            Random rand = new Random();
            float radius = 0f;
            double speed = 0.0;
            int life = 0;
            int engineCount = 0;

            foreach (ParticleSource particleSource in particleSourcesObject)
            {
                if (!particleSource.sleeping)
                {
                    // gdy generator dotyczy eksplozji
                    if (particleSource.actuallyCreated < particleSource.power && particleSource.type == Constants.PARTICLE_EXPLOSION)
                    {
                        for (int i = 0; i < particleSource.power; i++)
                        {
                            radius = (float)(particleSource.mAngle + rand.NextDouble() * particleSource.angleInclination);
                            speed = rand.NextDouble() * particleSource.maxSpeed + particleSource.minSpeed;
                            life = particleSource.life + rand.Next(0, 5);
                            AddParticleToTable(new Particle(particleSource.ID, particleSource.type, particleSource.vector, particleSource.colorList, life, radius, speed, Constants.explosionSize, particleSource.gravityAreaEffect));
                        }
                        particleSource.actuallyCreated = particleSource.power;
                    }
                    // w każdym innym przypadku
                    else if (particleSource.actuallyCreated < particleSource.power && rand.Next(0, 1) == 0)
                    {
                        radius = (float)(particleSource.mAngle + rand.NextDouble() * particleSource.angleInclination);
                        speed = rand.NextDouble() * particleSource.maxSpeed + particleSource.minSpeed;
                        life = particleSource.life + rand.Next(0, 5);
                        engineCount = 0;
                        foreach (Ship ship in shipsObject)
                        {
                            if (ship.ID == particleSource.ID)
                            {
                                engineCount = ship.property.gunCount;
                                break;
                            }
                        }
                        if (engineCount != 0)
                        {
                            // korekcja wektorów
                            Vector2 vect1, vect2, vect3;
                            vect1 = vect2 = vect3 = particleSource.vector;
                            switch (engineCount)
                            {
                                case 1:
                                    AddParticleToTable(new Particle(particleSource.ID, particleSource.type, vect1, particleSource.colorList, life, radius, speed, Constants.engineSize, particleSource.gravityAreaEffect));
                                    break;
                                case 2:
                                    Physic.SetNewCorrectVector(ref vect1, Physic.ConvertRadianToDegrees(particleSource.angle) - 90, 10);
                                    Physic.SetNewCorrectVector(ref vect2, Physic.ConvertRadianToDegrees(particleSource.angle) + 90, 10);
                                    AddParticleToTable(new Particle(particleSource.ID, particleSource.type, vect1, particleSource.colorList, life, radius, speed, Constants.engineSize, particleSource.gravityAreaEffect));
                                    AddParticleToTable(new Particle(particleSource.ID, particleSource.type, vect2, particleSource.colorList, life, radius, speed, Constants.engineSize, particleSource.gravityAreaEffect));
                                    break;
                                case 3:
                                    Physic.SetNewCorrectVector(ref vect1, Physic.ConvertRadianToDegrees(particleSource.angle) - 90, 16);
                                    Physic.SetNewCorrectVector(ref vect3, Physic.ConvertRadianToDegrees(particleSource.angle) + 90, 16);
                                    AddParticleToTable(new Particle(particleSource.ID, particleSource.type, vect1, particleSource.colorList, life, radius, speed, Constants.engineSize, particleSource.gravityAreaEffect));
                                    AddParticleToTable(new Particle(particleSource.ID, particleSource.type, vect2, particleSource.colorList, life, radius, speed, Constants.engineSize, particleSource.gravityAreaEffect));
                                    AddParticleToTable(new Particle(particleSource.ID, particleSource.type, vect3, particleSource.colorList, life, radius, speed, Constants.engineSize, particleSource.gravityAreaEffect));
                                    break;
                            }
                        }
                        else
                        {
                            // zwykłe cząsteczki
                            AddParticleToTable(new Particle(particleSource.ID, particleSource.type, particleSource.vector, particleSource.colorList, life, radius, speed, Constants.otherSize, particleSource.gravityAreaEffect));
                        }
                        particleSource.actuallyCreated++;
                    }
                }
            }
        }

        /// <summary>
        /// Dodaje nowych przeciwników
        /// </summary>
        static public void AddNewEnemy()
        {
            Random rand = new Random();
            Property enemyProperty;
            bool add = false;
            bool next = false;
            bool extraEnemy = false;
            int counter = 0;

            // zliczanie statków
            counter = shipsObject.Count;
            foreach (Ship ship in shipsObject)
            {
                if (ship.extras)
                {
                    counter--;
                }
            }

            if (counter <= gameModes[mGameMode].enemiesMax)
            {
                add = true;
            }
            else
            {
                if (rand.Next(0, gameModes[mGameMode].extraEnemyChance) == 0 && counter == shipsObject.Count)
                {
                    extraEnemy = true;
                    add = true;
                }
            }

            if (add)
            {
                int x = 0;
                int y = 0;

                // losowanie strony z której pojawi się nowy przeciwnik
                while (!next)
                {
                    next = true;
                    switch (rand.Next(0, 4))
                    {
                        // góra ekranu
                        case 0:
                            x = rand.Next(0, GUI.mWidth);
                            y = -50;
                            break;
                        // prawa strona ekrany
                        case 1:
                            x = GUI.mWidth + 50;
                            y = rand.Next(0, GUI.mHeight);
                            break;
                        // dół ekranu
                        case 2:
                            x = rand.Next(0, GUI.mWidth);
                            y = GUI.mHeight + 50;
                            break;
                        // lewa strona ekranu
                        case 3:
                            x = -50;
                            y = rand.Next(0, GUI.mHeight);
                            break;
                    }

                    foreach (Ship ship in shipsObject)
                    {
                        // 50 to bezpieczny odstęp od innych statków
                        if ((x + 50 / 6 >= ship.vector.X && x <= ship.vector.X + 50 / 6) &&
                            (y + 50 / 6 >= ship.vector.Y && y <= ship.vector.Y + 50 / 6))
                        {
                            next = false;
                        }
                    }

                }
                // losowanie typu statku
                string name = "Eship" + rand.Next(1, 5);

                enemyProperty = propertyList[name];
                if (extraEnemy)
                {
                    enemyProperty.shield += 3;
                }
                Ship newEnemy = new Ship(GetLastID(), x, y, name, 0f, enemyProperty, extraEnemy);
                shipsObject.Add(newEnemy);
                particleSourcesObject.Add(new ParticleSource(newEnemy.ID, Constants.PARTICLE_DYNAMIC, newEnemy.vector, 240f, 40f, 40, 10, colors["engineColors"], false, false, 1.5, 1.5));      
            }
        }

        /// <summary>
        /// Dodaje efekt eksplozji
        /// </summary>
        /// <param name="vector">Miejsce eksplozji</param>
        /// <param name="ID">ID np. statku, aby ID generatora było unikalne</param>
        static private void AddExplosion(Vector2 vector, int ID)
        {
            int XCombo = mActuallyCombo > 64 ? 64 : mActuallyCombo;

            Music.PlaySound(mPlaySounds, "explosion", Controler.mActuallyScreen);
            particleSourcesObject.Add(new ParticleSource(GetLastParticleSourcesID(), Constants.PARTICLE_EXPLOSION, vector, 0f, 360f, 120, 80, colors["explosionColorsX" + XCombo], false, true, 1.8, 2.5));
        }

        /// <summary>
        /// Tworzenie nowego pocisku
        /// </summary>
        /// <param name="ship">Obiekt statku, który strzela</param>
        /// <param name="power">Siła z jaką zostaje wystrzelony pocisk</param>
        /// <param name="name">Nazwa do pliku graficznego</param>
        /// <param name="ID">ID statku, który wystrzelił</param>
        static public void AddMissile(Ship ship, int power)
        {
            Vector2 shipGraphic = Controler.TransferGraphicInfo(ship.name);
            Vector2 vect1, vect2, vect3;
            vect1 = vect2 = vect3 = ship.vector;
            switch (ship.property.gunCount)
            {
                case 1:
                    if (ship.ID == 1)
                    {
                        // dodatkowy pocisk dla gracza
                        Physic.SetNewCorrectVector(ref vect1, Physic.ConvertRadianToDegrees(ship.angle) - 180, 15);
                        missilesObject.Add(new Missile(vect1.X, vect1.Y, Physic.ConvertRadianToDegrees(ship.mAngle), power, ship.ID));
                    }
                    missilesObject.Add(new Missile(vect2.X, vect2.Y, Physic.ConvertRadianToDegrees(ship.mAngle), power, ship.ID));
                    break;
                case 2:
                    Physic.SetNewCorrectVector(ref vect1, Physic.ConvertRadianToDegrees(ship.angle) + 90, 5);
                    Physic.SetNewCorrectVector(ref vect2, Physic.ConvertRadianToDegrees(ship.angle) - 90, 5);
                    missilesObject.Add(new Missile(vect1.X, vect1.Y, Physic.ConvertRadianToDegrees(ship.mAngle), power, ship.ID));
                    missilesObject.Add(new Missile(vect2.X, vect2.Y, Physic.ConvertRadianToDegrees(ship.mAngle), power, ship.ID));
                    break;
                case 3:
                    Physic.SetNewCorrectVector(ref vect1, Physic.ConvertRadianToDegrees(ship.angle) + 90, 10);
                    Physic.SetNewCorrectVector(ref vect3, Physic.ConvertRadianToDegrees(ship.angle) - 90, 10);
                    missilesObject.Add(new Missile(vect1.X, vect1.Y, Physic.ConvertRadianToDegrees(ship.mAngle), power, ship.ID));
                    missilesObject.Add(new Missile(vect2.X, vect2.Y, Physic.ConvertRadianToDegrees(ship.mAngle), power, ship.ID));
                    missilesObject.Add(new Missile(vect3.X, vect3.Y, Physic.ConvertRadianToDegrees(ship.mAngle), power, ship.ID));
                    break;
            }

            Music.PlaySound(mPlaySounds, "shoot", Controler.mActuallyScreen);
        }

        /// <summary>
        /// Generuje generatory cząsteczek tworzących tło
        /// </summary>
        static public void GenerateBackground()
        {
            Random rand = new Random();
            int x, y;
            x = y = 0;
            for (int i = 0; i < 100; i++)
            {
                x = rand.Next(0, GUI.mWidth);
                y = rand.Next(0, GUI.mHeight);
                particleSourcesObject.Add(new ParticleSource(GetLastParticleSourcesID(), Constants.PARTICLE_BACKGROUND, x, y, 0f, 360f, 2, 100000, colors["backgroundColors"], false, false, 0.01, 0.02));
            }
        }


        // ##########################################
        // #### Metody pomocnicze i kontrolujące ####
        // ##########################################


        /// <summary>
        /// Znaduje generator cząstek po podanym ID
        /// </summary>
        /// <param name="checkID">ID do porównania</param>
        /// <returns>Zwraca obiekt generatora cząsteczek lub null</returns>
        static public ParticleSource ParticleSourceIdEqual(int checkID)
        {
            foreach (ParticleSource particleSource in particleSourcesObject)
            {
                if (particleSource.ID == checkID)
                {
                    return particleSource;
                }
            }
            return null;
        }

        /// <summary>
        /// Kontroluje efekt eksplozji
        /// </summary>
        static public void ControlExplosion()
        {
            ParticleSource toDelete = null;
            foreach (ParticleSource particleSource in particleSourcesObject)
            {
                if (particleSource.type == Constants.PARTICLE_EXPLOSION)
                {
                    if (particleSource.actuallyCreated == particleSource.power)
                    {
                        toDelete = particleSource;
                    }
                }
            }

            if (toDelete != null)
            {
                particleSourcesObject.Remove(toDelete);
            }
        }

        /// <summary>
        /// Sprawdza przyspieszenia statków
        /// </summary>
        static public void ControlTurboMode()
        {
            foreach (Ship ship in shipsObject)
            {
                if (ship.turboMode)
                {
                    ship.turboPower--;
                    if (ship.roadLength < 0)
                    {
                        ship.turboMode = false;
                    }

                    if (ship.turboPower <= 0)
                    {
                        ship.turboMode = false;
                        ship.turboPower = 0;
                    }
                }
                else
                {
                    if (ship.turboPower < Constants.maxTurboPower)
                    {
                        ship.turboPower += 0.1;
                    }
                }
            }
        }

        /// <summary>
        /// Uaktualnianie informacji o kursorze
        /// </summary>
        
        static public void SetMouseInfo(Vector2 mouseVect)
        {
            mouseXDifference = (int)mouseVect.X - mouseX;
            mouseYDifference = (int)mouseVect.Y - mouseY;
            mouseX = (int)mouseVect.X;
            mouseY = (int)mouseVect.Y;
        }

        /// <summary>
        /// Kontroluje wyświetlane ostrzeżenia piorytetami
        /// </summary>
        /// <returns>Kod ostrzeżenia</returns>
        static public int ControlWarnings()
        {
            Ship playerShip = GetPlayerShip();

            // sprawdzanie wpływu pola grawitacyjnego
            foreach (GravityArea gravityArea in gravityAreasObject)
            {
                if (Physic.InGravityArea(playerShip.vector, gravityArea))
                {
                    return Constants.GRAVITY_WARNING;
                }
            }

            // sprawdzanie osłon
            if (playerShip.property.shield <= 2)
            {
                return Constants.SHIELD_WARNING;
            }

            return Constants.EMPTY_WARNING;
        }


        // ############################################
        // #### Metody zarządzające i modyfikujące ####
        // ############################################

        /// <summary>
        /// Przetwarza eventy myszki i klawiatury
        /// </summary>
        /// <param name="mouseEvent">Eventy z myszki</param>
        /// <param name="keybaordEvents">Eventy z klawiatury</param>
        static public void EventProcessing(List<int> mouseEvents, List<int> keybaordEvents)
        {
            // obsługa eventów myszy
            Ship playerShip = GetPlayerShip();
            ParticleSource particleSource = null;

            if (mouseEvents.Count != 0)
            {
                foreach (int mouseEvent in mouseEvents)
                {
                    switch (mouseEvent)
                    {
                        case Constants.CREATE_MISSILE:
                            GameLogic.AddMissile(playerShip, 200);
                            break;
                        case Constants.MOVE_SHIP:
                            particleSource = ParticleSourceIdEqual(playerShip.ID);
                            if (particleSource != null)
                            {
                                particleSource.Wake();
                            }
                            else
                            {
                                throw new Exception("Empty particleSource :: EventProcessing :: MouseEvent :: MOVE_SHIP");
                            }
                            Physic.PrepareShipMove(playerShip, mouseX, mouseY);
                            break;
                        case Constants.TURBO_MODE:
                            if (playerShip.turboPower > 0)
                            {
                                playerShip.turboMode = true;
                            }
                            break;
                        case Constants.NEXT_STEP:
                            if (toNextTutorialStep)
                            {
                                tutorialStep++;
                                toNextTutorialStep = false;
                            }
                            break;
                    }
                }
            }

            // obsługa eventów klawiatury
            if (keybaordEvents.Count != 0)
            {
                foreach (int keyboardInput in keybaordEvents)
                {
                    switch (keyboardInput)
                    {
                        case Constants.CREATE_MISSILE:
                            GameLogic.AddMissile(playerShip, 200);
                            break;
                        case Constants.LEFT:
                            playerShip.mAngle = Physic.ConvertRadianToDegrees(playerShip.mAngle) - 3;
                            break;
                        case Constants.RIGHT:
                            playerShip.mAngle = Physic.ConvertRadianToDegrees(playerShip.mAngle) + 3;
                            break;
                        case Constants.FORWARD:
                            forwardEcho = 0;
                            particleSource = ParticleSourceIdEqual(playerShip.ID);
                            if (particleSource != null)
                            {
                                particleSource.Wake();
                            }
                            else
                            {
                                throw new Exception("Empty particleSource :: EventProcessing :: KeyboardEvent :: FORWARD");
                            }
                            // wymóg funkcji
                            Physic.ChangeVector(playerShip, null, true, false, true);
                            break;
                        case Constants.FORWARD_ECHO:
                            ParticleSourceIdEqual(playerShip.ID).Sleep();
                            forwardEcho = 40;
                            break;
                        case Constants.RUSH:
                            playerShip.mAngle = Physic.ConvertRadianToDegrees(playerShip.mAngle) + 180;
                            break;
                        case Constants.TURBO_MODE:
                            if (playerShip.turboPower > 0)
                            {
                                playerShip.turboMode = true;
                            }
                            break;
                        case Constants.NEXT_STEP:
                            if (toNextTutorialStep)
                            {
                                tutorialStep++;
                                toNextTutorialStep = false;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Zarządza postępem tutoriala
        /// </summary>
        static public void TutorialProcessing()
        {
            Ship playerShip = GetPlayerShip();
            switch (tutorialStep)
            {
                case Constants.TUTORIAL_INIT:
                    CreateParticle();
                    MoveParticles(Controler.mActuallyScreen);
                    MoveParticleSources();
                    RemoveBadParticle();
                    toNextTutorialStep = true;
                    break;
                case 1:
                    CreateParticle();
                    MoveShips(Controler.mActuallyScreen);
                    MoveParticles(Controler.mActuallyScreen);
                    MoveParticleSources();

                    UpdatePlayerDilation();

                    RemoveBadParticle();

                    // sprawdzanie wymogu przejścia kroku
                    if ((playerShip.vector.X > 100 && playerShip.vector.X < 300) &&
                        (playerShip.vector.Y > 100 && playerShip.vector.Y < 200))
                    {
                        tutorialStep++;
                    }
                    break;
                case 2:
                    CreateParticle();
                    MoveShips(Controler.mActuallyScreen);
                    MoveParticles(Controler.mActuallyScreen);
                    MoveParticleSources();

                    ControlTurboMode();
                    UpdatePlayerDilation();

                    RemoveBadParticle();
                    // sprawdzanie wymogu przejścia do następnego kroku
                    if ((playerShip.vector.X > GUI.mWidth - 300 && playerShip.vector.X < GUI.mWidth - 100) &&
                        (playerShip.vector.Y > GUI.mHeight - 200 && playerShip.vector.Y < GUI.mHeight - 150))
                    {
                        tutorialStep++;
                        // i dodanie biernego przeciwnika
                        string name = "Eship" + rand.Next(1, 5);
                        Ship newEnemy = new Ship(GetLastID(), 600, 600, name, 0f, propertyList[name]);
                        shipsObject.Add(newEnemy);
                        particleSourcesObject.Add(new ParticleSource(newEnemy.ID, Constants.PARTICLE_DYNAMIC, newEnemy.vector, 240f, 40f, 40, 10, colors["engineColors"], true, false, 1.5, 1.5));
                    }
                    break;
                case 3:
                    CreateParticle();
                    MoveMissiles();
                    MoveShips(Controler.mActuallyScreen);
                    MoveParticles(Controler.mActuallyScreen);
                    MoveParticleSources();

                    ControlExplosion();
                    ControlTurboMode();
                    UpdatePlayerDilation();

                    RemoveBadMissile();
                    RemoveBadParticle();
                    // sprawdzanie wymogu przejścia do nastepnego kroku
                    if (shipsObject.Count == 1)
                    {
                        tutorialStep++;
                    }
                    break;
                case 4:
                case 5:
                    CreateParticle();
                    AddNewEnemy();

                    MoveMissiles();
                    MoveShips(Controler.mActuallyScreen);
                    MoveParticles(Controler.mActuallyScreen);
                    MoveParticleSources();

                    ControlTurboMode();
                    ControlExplosion();
                    UpdatePlayerDilation();

                    AI.PrepareAILogic(GetShips, GetPlayerShip());

                    RemoveBadMissile();
                    RemoveBadParticle();
                    break;
            }
            
        }

        /// <summary>
        /// Czyści tablice cząsteczek z podanej grupy
        /// </summary>
        /// <param name="idGroup">ID grupy</param>
        static public void ClearParticlesTable(int idGroup)
        {
            for (int i = 0; i < particlesObject.Length; i++)
            {
                if (particlesObject[i] != null)
                {
                    if (particlesObject[i].type == idGroup)
                    {
                        particlesObject[i] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Czyści źródła cząsteczek z wyjątkiem podanej grupy
        /// </summary>
        /// <param name="idGroup">ID grupy</param>
        static private void ClearSourceObjects(int idGroup = -1)
        {
            ParticleSource toDelete = null;
            bool next = true;

            while (next)
            {
                next = false;
                foreach (ParticleSource particleSource in particleSourcesObject)
                {
                    if (particleSource.type != idGroup)
                    {
                        toDelete = particleSource;
                        next = true;
                        break;
                    }
                }
                if (toDelete != null)
                {
                    particleSourcesObject.Remove(toDelete);
                }
            }
        }

        /// <summary>
        /// Czyści zmienne, które muszą być ustawiane domyślnie po zmianie ekranu
        /// </summary>
        /// <param name="actuallyScreen">Aktualne okno gry</param>
        static private void ClearTempVariables(int actuallyScreen)
        {
            // czyszczenie zmiennych combosów
            mActuallyCombo = 1;
            toNextCombo = 0;

            // czyszczenie ostatnich ID
            lastID = 1;
            lastParticleSourceID = 1000;

            // czyszczenie list
            if (actuallyScreen != Constants.CHOOSING)
            {
                particleSourcesObject.Clear();
            }
            else
            {
                gravityAreasObject.Clear();
                missilesObject.Clear();
                ClearSourceObjects();
                ClearParticlesTable(Constants.PARTICLE_GRAVITY_AREA);
                ClearParticlesTable(Constants.PARTICLE_EXPLOSION);   
            }
            shipsObject.Clear();
        }

        /// <summary>
        /// Dodaje nową cząsteczke do tablicy
        /// </summary>
        /// <param name="newParticle">Obiekt cząsteczki</param>
        static private void AddParticleToTable(Particle newParticle)
        {
            for (int j = 0; j != particlesObject.Length; j++)
            {
                if (particlesObject[j] == null)
                {
                    particlesObject[j] = newParticle;
                    break;
                }
            }
        }

        /// <summary>
        /// Aktualizuje zmienną dotyczącą dylatacji czasu gracza
        /// </summary>
        static public void UpdatePlayerDilation()
        {
            playerDilation = Physic.TimeDilationEffect(GetPlayerShip().vector, gravityAreasObject);
        }

        /// <summary>
        /// Przemieszczanie pocisków
        /// </summary>
        static public void MoveMissiles()
        {
            Missile toDelete = null;
            Random rand = new Random();
            double missileDilation;
            double[] dilationTab;

            foreach (Missile missile in missilesObject)
            {
                dilationTab = null;
                if (gameModes[gameMode].timeDilation)
                {
                    missileDilation = Physic.TimeDilationEffect(missile.vector, gravityAreasObject);
                    dilationTab = Physic.CompareDilationEffect(playerDilation, missileDilation);
                }
                missile.pixelCounter = 8;
                while (missile.pixelCounter > 1)
                {
                    // siła pędu po wystrzeleniu pocisku ignoruje pole grawitacyjne
                    if (rand.Next(missile.power) == 0)
                    {
                        foreach (GravityArea gravityArea in gravityAreasObject)
                        {
                            Physic.InGravityAreaChangeVector(ref missile.vector, gravityArea, "missile", dilationTab);
                        }
                    }
                    Physic.ChangeVector(missile, dilationTab);
                    if (missile.power > 1)
                    {
                        missile.power--;
                    }
                    missile.pixelCounter--;
                }

                if ((missile.vector.X > GUI.mWidth) || (missile.vector.X < -20) ||
                    (missile.vector.Y > GUI.mHeight) || (missile.vector.Y < -22))
                {
                    toDelete = missile;
                }
            }
            if (toDelete != null)
            {
                missilesObject.Remove(toDelete);
            }
        }

        /// <summary>
        /// Przemieszczanie statków
        /// </summary>
        /// <param name="actuallyScreen">Aktualne okno gry</param>
        /// <param name="withGravityMove">Czy na statek oddziałuje pole grawitacyjne</param>
        static public void MoveShips(int actuallyScreen, bool withGravityMove = true)
        {
            double[] dilationTab;
            double shipDilation;

            foreach (Ship ship in shipsObject)
            {
                dilationTab = null;
                if (gameModes[gameMode].timeDilation && actuallyScreen != Constants.CHOOSING)
                {
                    shipDilation = Physic.TimeDilationEffect(ship.vector, gravityAreasObject);
                    dilationTab = Physic.CompareDilationEffect(playerDilation, shipDilation);
                }

                // dotyczy hamowania statku
                if (forwardEcho > 0 && ship.ID == 1)
                {
                    Physic.ChangeVector(ship, dilationTab, true, true);
                    forwardEcho--;
                }

                if (withGravityMove)
                {
                    foreach (GravityArea gravityArea in gravityAreasObject)
                    {
                        Physic.InGravityAreaChangeVector(ref ship.vector, gravityArea, "ship", dilationTab);
                    }
                }

                if (ship.roadLength > 0)
                {
                    Physic.ChangeVector(ship, dilationTab);
                }
                else
                {
                    if (ship.ID == 1 && mControlType == Constants.CONTROL_MOUSE && actuallyScreen != Constants.START)
                    {
                        ParticleSource particleSource = ParticleSourceIdEqual(ship.ID);
                        if (particleSource != null)
                        {
                            particleSource.Sleep();
                        }
                        else
                        {
                            throw new Exception("Empty particleSource :: GameLogic :: MoveShip");
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Zmienia wektor cząsteczek poruszających się (dokładnie to silników statków)
        /// </summary>
        static public void MoveParticleSources()
        {
            ParticleSource particleSource = null;

            foreach (Ship ship in shipsObject)
            {
                particleSource = ParticleSourceIdEqual(ship.ID);
                if (particleSource != null)
                {
                    particleSource.vector = ship.vector;
                    Physic.SetNewCorrectVector(ref particleSource.vector, Physic.ConvertRadianToDegrees(particleSource.angle), 10);
                    particleSource.mAngle = Physic.ConvertRadianToDegrees(ship.mAngle) - 180;
                }
                else
                {
                    throw new Exception("Empty particleSource :: GameLogic :: MoveParticleSource");
                }
            }
        }

        /// <summary>
        /// Przemieszcza cząsteczki
        /// </summary>
        static public void MoveParticles(int actuallyScreen)
        {
            ParticleSource particleSource = null;
            double[] dilationTab;
            double shipDilation;

            for (int i = 0; i < particlesObject.Length; i++)
            {
                if (particlesObject[i] != null)
                {
                    if (particlesObject[i].life > 0)
                    {
                        dilationTab = null;
                        if (gameModes[gameMode].timeDilation && actuallyScreen != Constants.CHOOSING && particlesObject[i].type != Constants.PARTICLE_GRAVITY_AREA)
                        {
                            shipDilation = Physic.TimeDilationEffect(particlesObject[i].vector, gravityAreasObject);
                            dilationTab = Physic.CompareDilationEffect(playerDilation, shipDilation);
                        }

                        Physic.ChangeVector(particlesObject[i], dilationTab);
                        if (particlesObject[i].gravityAreaEffect)
                        {
                            foreach (GravityArea gravityArea in gravityAreasObject)
                            {
                                Physic.InGravityAreaChangeVector(ref particlesObject[i].vector, gravityArea, "particle");
                            }
                        }
                    }
                    else
                    {
                        particleSource = ParticleSourceIdEqual(particlesObject[i].idSource);
                        if (particleSource != null)
                        {
                            particleSource.actuallyCreated--;
                        }
                        particlesObject[i] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Przeszukuje obiekty i wybiera te, które mają zostać narysowane
        /// FUNKCJA AKTUALNIE NIEPRZYDATNA
        /// </summary>
        /*static public void SelectObjectToDraw()
        {
            Ship player = GetPlayerShip();
            foreach (Ship ship in shipsObject)
            {
                if (ship.ID == 1)
                {
                    ship.toDraw = Constants.DRAW;
                }
                else
                {
                    if (!Physic.InPurview(player.vector, ship.vector, Constants.viewAngle, player.mAngle))
                    {
                        ship.toDraw = Constants.RADAR_DRAW;
                    }
                    else
                    {
                        ship.toDraw = Constants.DRAW;
                    }

                    // do poprawy zanikania
                    foreach (GravityArea gravityArea in gravityAreasObject)
                    {
                        if (Physic.InGravityArea(ship.vector, gravityArea))
                        {
                            ship.toDraw = Constants.NOT_DRAW;
                            break;
                        }
                    }
                }
            }

            foreach (Missile missile in missilesObject)
            {
                if (!Physic.InPurview(player.vector, missile.vector, Constants.viewAngle, player.mAngle))
                {
                    missile.toDraw = Constants.RADAR_DRAW;
                }
                else
                {
                    missile.toDraw = Constants.DRAW;
                }

                // do poprawy zanikania
                foreach (GravityArea gravityArea in gravityAreasObject)
                {
                    if (Physic.InGravityArea(missile.vector, gravityArea))
                    {
                        missile.toDraw = Constants.NOT_DRAW;
                        break;
                    }
                }
            }
        }*/

        /// <summary>
        /// Obraca wszystkie statki, podany w argumencie szybciej
        /// </summary>
        /// <param name="chooseShipID">ID statku</param>
        static public void RotateShip(int chooseShipID)
        {
            foreach (Ship ship in shipsObject)
            {
                float angle = Physic.ConvertRadianToDegrees(ship.mAngle);

                if (angle < 360)
                {
                    angle += ship.ID == chooseShipID ? 2 : 1;
                }
                else
                {
                    angle = 0;
                }

                ship.mAngle = angle;
            }
        }


        // ######################################
        // #### Metody niszczące i usuwające ####
        // ######################################


        /// <summary>
        /// Niszczy trafione statki
        /// </summary>
        /// <param name="actuallyScreen">Aktualny ekran gry</param>
        /// <returns>-1 oznacza normany bieg rzeczy, 0 oznacza zniszczenie statku gracza</returns>
        static public int DestroyShips(int actuallyScreen)
        {
            Ship toDeleteShip = null;
            Missile toDeleteMissile = null;

            foreach (Ship ship in shipsObject)
            {
                foreach (Missile missile in missilesObject)
                {
                    Vector2 shipGraphic = Controler.TransferGraphicInfo(ship.name);
                    Vector2 missileGraphic = Controler.TransferGraphicInfo("missile1");

                    if (missile.shipID != ship.ID) {
                        if ((missile.vector.X + missileGraphic.X >= ship.vector.X && missile.vector.X <= ship.vector.X + shipGraphic.X) &&
                            (missile.vector.Y + missileGraphic.Y >= ship.vector.Y && missile.vector.Y <= ship.vector.Y + shipGraphic.Y))
                        {
                            if (ship.property.shield < 0)
                            {
                                toDeleteShip = ship;
                                // z powodu scenki
                                if (ship.ID == 1)
                                {
                                    toNextCombo = 0;
                                    mActuallyCombo = 1;
                                }
                            }
                            else
                            {
                                ship.property.shield--;
                                if (ship.ID == 1)
                                {
                                    toNextCombo = 0;
                                    mActuallyCombo = 1;
                                }
                            }
                            toDeleteMissile = missile;
                        }
                    }
                }
            }

            if (toDeleteShip != null)
            {
                if ((actuallyScreen != Constants.START && actuallyScreen != Constants.OPTIONS && actuallyScreen != Constants.CREDITS) || toDeleteShip.ID != 1)
                {
                    // dodawanie eksplozji
                    AddExplosion(toDeleteShip.vector, toDeleteShip.ID);

                    if (toDeleteShip.ID == 1)
                    {
                        return 0;
                    }
                    else
                    {
                        // zmiana kroku, jeśli to tutorial
                        if (actuallyScreen == Constants.TUTORIAL && tutorialStep == 4)
                        {
                            tutorialStep++;
                        }

                        // dodawanie punktów
                        if (!toDeleteShip.extras)
                        {
                            mActuallyPoints += toDeleteShip.property.points * mActuallyCombo;
                        }
                        else
                        {
                            mActuallyPoints += (toDeleteShip.property.points * 10) * mActuallyCombo;
                        }
                        

                        // zmiana mnożnika combosów
                        toNextCombo++;
                        if (toNextCombo == mActuallyCombo)
                        {
                            mActuallyCombo *= 2;
                            toNextCombo = 0;
                        }
                        

                        // usuwanie statku
                        shipsObject.Remove(toDeleteShip);
                        missilesObject.Remove(toDeleteMissile);

                        // usuwanie cząsteczek
                        particleSourcesObject.Remove(ParticleSourceIdEqual(toDeleteShip.ID));
                    }
                }
            }

            if (toDeleteMissile != null)
            {
                missilesObject.Remove(toDeleteMissile);
            }

            return -1;
        }

        /// <summary>
        /// Niszczy statek gracza, który zniknął za ekran
        /// </summary>
        /// <param name="actuallyScreen">Aktualny ekran gry</param>
        /// <returns>-1 oznacza normany bieg rzeczy, 0 oznacza zniszczenie statku gracza</returns>
        static public int DestroyPlayerOutOfRange(int actuallyScreen)
        {
            if ((actuallyScreen == Constants.GAME || actuallyScreen == Constants.TUTORIAL) && (controlType == Constants.CONTROL_KEYBOARD))
            {
                Ship playerShip = GetPlayerShip();

                if ((playerShip.vector.X < -20 || playerShip.vector.X > GUI.mWidth + 20) ||
                    (playerShip.vector.Y < -20 || playerShip.vector.Y > GUI.mHeight + 20))
                {
                    return 0;
                }
            }
            return -1;
        }

        /// <summary>
        /// Niszczy statki, które weszły kolizje
        /// </summary>
        /// <param name="actuallyScreen">Aktualny ekran gry</param>
        /// <returns>-1 oznacza normany bieg rzeczy, 0 oznacza zniszczenie statku gracza</returns>
        static public int CrashShip(int actuallyScreen)
        {
            Ship toDeleteShip = null;
            Ship toDeleteShip2 = null;
            foreach (Ship ship in shipsObject)
            {
                Vector2 shipGraphic = Controler.TransferGraphicInfo(ship.name);
                foreach (Ship ship2 in shipsObject)
                {
                    if (ship.ID != ship2.ID)
                    {
                        Vector2 ship2Graphic = Controler.TransferGraphicInfo(ship2.name);
                        if ((ship2.vector.X + ship2Graphic.X/6 >= ship.vector.X && ship2.vector.X <= ship.vector.X + shipGraphic.X/6) &&
                            (ship2.vector.Y + ship2Graphic.Y/6 >= ship.vector.Y && ship2.vector.Y <= ship.vector.Y + shipGraphic.Y/6))
                        {
                            toDeleteShip = ship;
                            toDeleteShip2 = ship2;
                        }
                    }
                }
            }

            if (toDeleteShip != null)
            {
                if ((actuallyScreen != Constants.START && actuallyScreen != Constants.OPTIONS && actuallyScreen != Constants.CREDITS) || (toDeleteShip.ID != 1 && toDeleteShip2.ID != 1))
                {
                    // dodawanie eksplozji
                    AddExplosion(toDeleteShip.vector, toDeleteShip.ID);

                    if (toDeleteShip.ID == 1 || toDeleteShip2.ID == 1)
                    {
                        return 0;
                    }
                    else
                    {
                        // dodawanie punktów
                        mActuallyPoints += toDeleteShip.property.points + toDeleteShip2.property.points;

                        // usuwanie
                        shipsObject.Remove(toDeleteShip);
                        particleSourcesObject.Remove(ParticleSourceIdEqual(toDeleteShip.ID));
                        shipsObject.Remove(toDeleteShip2);
                        particleSourcesObject.Remove(ParticleSourceIdEqual(toDeleteShip2.ID));
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Usuwanie pocisków, które nie mogą opuścić pola grawitacyjnego
        /// </summary>
        static public void RemoveBadMissile()
        {
            Missile toDelete = null;
            foreach (Missile missile in missilesObject)
            {
                foreach (GravityArea gravityArea in gravityAreasObject)
                {
                    Vector2 gravityAreaGraphic = Controler.TransferGraphicInfo(gravityArea.name);
                    float gravityAreaX = gravityArea.vector.X + gravityAreaGraphic.X / 2;
                    float gravityAreaY = gravityArea.vector.Y + gravityAreaGraphic.Y / 2;

                    if ((missile.vector.X > gravityAreaX - 20 && missile.vector.X < gravityAreaX + 20) &&
                        (missile.vector.Y > gravityAreaY - 20 && missile.vector.Y < gravityAreaY + 20))
                    {
                        toDelete = missile;
                    }
                }
            }
            if (toDelete != null)
            {
                missilesObject.Remove(toDelete);
            }
        }

        /// <summary>
        /// Usuwanie statków nie mogących opuścić pola grawitacyjnego
        /// </summary>
        /// <returns>-1 oznacza zniszczenie przecinwika, 0 statku gracza</returns>
        static public int RemoveBadShip(int actuallyScreen)
        {
            Ship toDelete = null;
            foreach (Ship ship in shipsObject)
            {
                foreach (GravityArea gravityArea in gravityAreasObject)
                {
                    Vector2 gravityAreaGraphic = Controler.TransferGraphicInfo(gravityArea.name);
                    float gravityAreaX = gravityArea.vector.X + gravityAreaGraphic.X / 2;
                    float gravityAreaY = gravityArea.vector.Y + gravityAreaGraphic.Y / 2;

                    Vector2 shipGraphic = Controler.TransferGraphicInfo(ship.name);
                    float shipX = ship.vector.X + shipGraphic.X / 2;
                    float shipY = ship.vector.Y + shipGraphic.Y / 2;

                    if ((shipX > gravityAreaX - 20 && shipX < gravityAreaX + 20) &&
                        (shipY > gravityAreaY - 20 && shipY < gravityAreaY + 20))
                    {
                        toDelete = ship;
                    }
                }
            }
            if (toDelete != null)
            {
                if (actuallyScreen != Constants.START && toDelete.ID == 1)
                {
                    if (toDelete.ID == 1)
                    {
                        return 0;
                    }
                    else
                    {
                        particleSourcesObject.Remove(ParticleSourceIdEqual(toDelete.ID));
                        shipsObject.Remove(toDelete);
                    }
                } 
                else if (toDelete.ID != 1)
                {
                    particleSourcesObject.Remove(ParticleSourceIdEqual(toDelete.ID));
                    shipsObject.Remove(toDelete);
                    AddExplosion(toDelete.vector, toDelete.ID);
                }
            }

            return -1;
        }

        /// <summary>
        /// Niszczy cząsteczki, które utknęły w polu grawitacyjnym
        /// </summary>
        static public void RemoveBadParticle()
        {
            ParticleSource particleSource;
            for (int i = 0; i < Constants.maxParticlesCount; i++)
            {
                if (particlesObject[i] != null)
                {
                    foreach (GravityArea gravityArea in gravityAreasObject)
                    {
                        Vector2 gravityAreaGraphic = Controler.TransferGraphicInfo(gravityArea.name);
                        float gravityAreaX = gravityArea.vector.X + gravityAreaGraphic.X / 2;
                        float gravityAreaY = gravityArea.vector.Y + gravityAreaGraphic.Y / 2;

                        if ((particlesObject[i].vector.X > gravityAreaX - 6 && particlesObject[i].vector.X < gravityAreaX + 6) &&
                            (particlesObject[i].vector.Y > gravityAreaY - 6 && particlesObject[i].vector.Y < gravityAreaY + 6))
                        {
                            particleSource = ParticleSourceIdEqual(particlesObject[i].idSource);
                            if (particleSource != null)
                            {
                                particleSource.actuallyCreated--;
                            }
                            particlesObject[i] = null;
                            break;
                        }
                    }
                }
            }
        }
    }
}