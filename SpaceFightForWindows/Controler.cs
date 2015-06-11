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
    /// Klasa odpowiadająca za komunikacje między innymi klasami
    /// </summary>
    class Controler
    {
        // aktualne okno gry
        static private int actuallyScreen = Constants.START;
        static public int mActuallyScreen
        {
            get
            {
                return actuallyScreen;
            }
        }

        // element, który jest podświetlony - 0 = żaden
        static public int hoverElement = 0;

        // zmienna zmiany okna gry
        static private int nextScreenToChange = 0;

        // obiekt wybranego statku
        static private Ship chooseShip = null;

        // zmienna dotycząca kliknięcia myszką
        static private MouseState previousMouseState = Mouse.GetState();
        static private KeyboardState previousKeyState = Keyboard.GetState();

        /// <summary>
        /// Ustala wielkość okna
        /// </summary>
        /// <param name="width">Szerokość w px</param>
        /// <param name="height">Wysokość w px</param>
        static public void SetWindowWidthAndHeight(int width, int height)
        {
            GUI.mWidth = width;
            GUI.mHeight = height;
        }

        static public void Initialize()
        {
            GameLogic.GetShipsProperty();
            GameLogic.GetColors();
            GameLogic.SetGameModes();
            GameLogic.GetSettings();
            GameLogic.CreateShips(actuallyScreen, chooseShip);
            GameLogic.GenerateBackground();
            GameLogic.CreateGravityAreas(actuallyScreen, GUI.mWidth, GUI.mHeight);
            Logger.Info("Inicjalizacja gry.");
        }
        static public void Load(ContentManager Content)
        {
            GUI.LoadImages(Content);
            Logger.Info("Załadowanie grafik.");
            GUI.LoadFonts(Content);
            Logger.Info("Załadowanie czcionek.");
            Music.LoadMusic(Content);
            Music.LoadSounds(Content);
        }
        static public void Draw(ref SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            try
            {
                switch (actuallyScreen)
                {
                    case Constants.START:
                    case Constants.OPTIONS:
                    case Constants.CREDITS:
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_BACKGROUND);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_GRAVITY_AREA);
                        GUI.DrawGravityAreas(ref spriteBatch, GameLogic.GetGravityAreas);
                        GUI.DrawMenu(ref spriteBatch, GameLogic.mHighscore, actuallyScreen, GameLogic.mControlType, GameLogic.mShowingWarnings, GameLogic.mGameMode, GameLogic.mPlaySounds, GameLogic.mPlayMusic, hoverElement);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_EXPLOSION);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_DYNAMIC);
                        GUI.DrawMissiles(ref spriteBatch, GameLogic.GetMissiles);
                        GUI.DrawShips(ref spriteBatch, GameLogic.GetShips);
                        break;
                    case Constants.CHOOSING:
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_BACKGROUND);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_GRAVITY_AREA);
                        GUI.DrawGravityAreas(ref spriteBatch, GameLogic.GetGravityAreas);
                        GUI.DrawMenu(ref spriteBatch, GameLogic.mHighscore, actuallyScreen, GameLogic.mControlType, GameLogic.mShowingWarnings, GameLogic.mGameMode, GameLogic.mPlaySounds, GameLogic.mPlayMusic, hoverElement);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_EXPLOSION);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_DYNAMIC);
                        GUI.DrawMissiles(ref spriteBatch, GameLogic.GetMissiles);
                        GUI.DrawShips(ref spriteBatch, GameLogic.GetShips, actuallyScreen, chooseShip.ID);
                        break;
                    case Constants.TUTORIAL:
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_BACKGROUND);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_GRAVITY_AREA);
                        GUI.DrawGravityAreas(ref spriteBatch, GameLogic.GetGravityAreas);
                        GUI.DrawTutorial(ref spriteBatch, graphicsDevice, GameLogic.mTutorialStep, GameLogic.mControlType);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_EXPLOSION);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_DYNAMIC);
                        GUI.DrawMissiles(ref spriteBatch, GameLogic.GetMissiles);
                        GUI.DrawShips(ref spriteBatch, GameLogic.GetShips);
                        switch (GameLogic.mTutorialStep)
                        {
                            case 2:
                                GUI.DrawTurboPowerBar(ref spriteBatch, graphicsDevice, GameLogic.GetPlayerShip());
                                break;
                            case 3:
                            case 4:
                            case 5:
                                GUI.DrawShieldBar(ref spriteBatch, graphicsDevice, GameLogic.GetPlayerShip());
                                GUI.DrawTurboPowerBar(ref spriteBatch, graphicsDevice, GameLogic.GetPlayerShip());
                                break;
                        }
                        break;
                    case Constants.GAME:
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_BACKGROUND);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_GRAVITY_AREA);
                        GUI.DrawGravityAreas(ref spriteBatch, GameLogic.GetGravityAreas);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_EXPLOSION);
                        GUI.DrawParticles(ref spriteBatch, GameLogic.GetParticles, Constants.PARTICLE_DYNAMIC);
                        GUI.DrawMissiles(ref spriteBatch, GameLogic.GetMissiles);
                        GUI.DrawShips(ref spriteBatch, GameLogic.GetShips);
                        GUI.DrawTurboPowerBar(ref spriteBatch, graphicsDevice, GameLogic.GetPlayerShip());
                        GUI.DrawShieldBar(ref spriteBatch, graphicsDevice, GameLogic.GetPlayerShip());
                        GUI.DrawGameInfo(ref spriteBatch, GameLogic.mActuallyPoints, GameLogic.mActuallyCombo);
                        if (GameLogic.mShowingWarnings)
                        {
                            GUI.DrawWarnings(ref spriteBatch, GameLogic.ControlWarnings(), GameLogic.GetPlayerShip());
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
               Logger.Error(ex, "Błąd podczas pętli rysowania");
            }
        }
        static public void Update()
        {
            try
            {
                GameLogic.EventProcessing(HandleMouseEvent(), HandleKeyboardEvent());
                CheckChangeScreen();
                Music.PlayMusic(GameLogic.mPlayMusic, actuallyScreen);

                switch (actuallyScreen)
                {
                    case Constants.START:
                    case Constants.OPTIONS:
                    case Constants.CREDITS:
                        hoverElement = CheckActuallyHover();
                        GameLogic.CreateParticle();
                        GameLogic.AddNewEnemy();

                        GameLogic.MoveMissiles();
                        GameLogic.MoveShips(actuallyScreen);
                        GameLogic.MoveParticles(actuallyScreen);
                        GameLogic.MoveParticleSources();

                        GameLogic.ControlTurboMode();
                        GameLogic.ControlExplosion();
                        GameLogic.UpdatePlayerDilation();
                        
                        AI.PrepareAILogic(GameLogic.GetShips, GameLogic.GetPlayerShip());
                        AI.PrepareMenuAILogic(GameLogic.GetShips, GameLogic.GetPlayerShip());

                        GameLogic.RemoveBadMissile();
                        GameLogic.DestroyShips(actuallyScreen);
                        GameLogic.RemoveBadShip(actuallyScreen);
                        GameLogic.CrashShip(actuallyScreen);
                        GameLogic.RemoveBadParticle();
                        break;
                    case Constants.CHOOSING:
                        hoverElement = CheckActuallyHover();
                        GameLogic.CreateParticle();
                        GameLogic.MoveMissiles();
                        GameLogic.RemoveBadMissile();
                        GameLogic.MoveParticles(actuallyScreen);
                        GameLogic.RotateShip(chooseShip.ID);
                        break;
                    case Constants.TUTORIAL:
                        GameLogic.TutorialProcessing();
                        break;
                    case Constants.GAME:
                        GameLogic.CreateParticle();
                        GameLogic.AddNewEnemy();

                        GameLogic.MoveMissiles();
                        GameLogic.MoveShips(actuallyScreen);
                        GameLogic.MoveParticles(actuallyScreen);
                        GameLogic.MoveParticleSources();

                        GameLogic.ControlTurboMode();
                        GameLogic.ControlExplosion();
                        GameLogic.UpdatePlayerDilation();
                        
                        AI.PrepareAILogic(GameLogic.GetShips, GameLogic.GetPlayerShip());
                        
                        GameLogic.RemoveBadMissile();
                        GameLogic.RemoveBadParticle();
                        break;
                }


                previousMouseState = Mouse.GetState();
                previousKeyState = Keyboard.GetState();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Błąd podczas aktualizowania stanu rozgrywki");
            }
        }

        /// <summary>
        /// Sprawdza, i ewentualnie wykonuje zmiane okna gry.
        /// </summary>
        static private void CheckChangeScreen()
        {
            switch (actuallyScreen)
            {
                case Constants.GAME:
                    if (GameLogic.DestroyShips(actuallyScreen) == 0 || GameLogic.RemoveBadShip(actuallyScreen) == 0 || GameLogic.CrashShip(actuallyScreen) == 0 || GameLogic.DestroyPlayerOutOfRange(actuallyScreen) == 0)
                    {
                        nextScreenToChange = Constants.START;
                    }
                    break;
                case Constants.TUTORIAL:
                    if (GameLogic.DestroyPlayerOutOfRange(actuallyScreen) == 0)
                    {
                        nextScreenToChange = Constants.START;
                        GameLogic.mTutorialStep = Constants.TUTORIAL_INIT;
                    }

                    if (GameLogic.mTutorialStep > 2)
                    {
                        if (GameLogic.DestroyShips(actuallyScreen) == 0 || GameLogic.RemoveBadShip(actuallyScreen) == 0 || GameLogic.CrashShip(actuallyScreen) == 0)
                        {
                            nextScreenToChange = Constants.START;
                            GameLogic.mTutorialStep = Constants.TUTORIAL_INIT;
                        }
                    }
                    break;
            }

            if (nextScreenToChange != 0)
            {
                ChangeScreen(nextScreenToChange);
                nextScreenToChange = 0;
            }
        }

        /// <summary>
        /// Zawiera instrukcje dotyczące zmiany ekranów
        /// <param name="newScreen">ID nowego ekranu</param>
        /// </summary>
        static private void ChangeScreen(int newScreen)
        {
            switch (newScreen)
            {
                case Constants.START:
                    if (actuallyScreen != Constants.OPTIONS && actuallyScreen != Constants.CREDITS)
                    {
                        UpdateHighscore();
                        chooseShip = null;
                        GameLogic.CreateShips(newScreen);
                        GameLogic.CreateGravityAreas(newScreen, GUI.mWidth, GUI.mHeight);
                        GameLogic.ClearParticlesTable(Constants.PARTICLE_GRAVITY_AREA);
                    }
                    break;
                case Constants.CHOOSING:
                    GameLogic.CreateShips(newScreen, chooseShip, GUI.mWidth, GUI.mHeight);
                    chooseShip = GameLogic.GetShips[0];
                    break;
                case Constants.GAME:
                    GameLogic.mActuallyPoints = 0;
                    GameLogic.CreateShips(newScreen, chooseShip);
                    GameLogic.CreateGravityAreas(newScreen, GUI.mWidth, GUI.mHeight);
                    GameLogic.ClearParticlesTable(Constants.PARTICLE_GRAVITY_AREA);
                    break;
                case Constants.OPTIONS:
                    // instrukcje do opcji
                    break;
                case Constants.TUTORIAL:
                    GameLogic.mActuallyPoints = 0;
                    GameLogic.CreateShips(newScreen, chooseShip);
                    GameLogic.CreateGravityAreas(newScreen, GUI.mWidth, GUI.mHeight);
                    GameLogic.ClearParticlesTable(Constants.PARTICLE_GRAVITY_AREA);
                    break;
            }

            actuallyScreen = newScreen;
        }

        /// <summary>
        /// Sprawdza czy kursor jest nad jakimś polem wyboru
        /// </summary>
        /// <returns>Kod podświetlonego elementu</returns>
        static private int CheckActuallyHover()
        {
            Vector2 mouseVect = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            switch (actuallyScreen)
            {
                case Constants.START:
                    int mainWidth = (int)GUI.CheckTextSize("menuFont90", "SpaceFight").X;
                    if (CheckClick((GUI.mWidth - mainWidth) / 2 + 30, (GUI.mHeight - 150) / 2, GUI.CheckTextSize("menuFont42", "New game"), mouseVect))
                    {
                        return 1;
                    }
                    else if (CheckClick((GUI.mWidth - mainWidth) / 2 + 60, (GUI.mHeight - 50) / 2, GUI.CheckTextSize("menuFont42", "Exit"), mouseVect))
                    {
                        return 2;
                    }
                    else if (CheckClick((GUI.mWidth - mainWidth) / 2 + mainWidth - (int)GUI.CheckTextSize("menuFont42", "Options").X - 30, (GUI.mHeight - 150) / 2, GUI.CheckTextSize("menuFont42", "Options"), mouseVect))
                    {
                        return 3;
                    }
                    else if (CheckClick((GUI.mWidth - mainWidth) / 2 + mainWidth - (int)GUI.CheckTextSize("menuFont42", "Credits").X - 60, (GUI.mHeight - 50) / 2, GUI.CheckTextSize("menuFont42", "Credits"), mouseVect))
                    {
                        return 4;
                    }
                    break;
                case Constants.OPTIONS:
                    // z powodu zmiennego tekstu
                    string controlText, showingWarnText, gameModeText, soundsText, musicText;
                    controlText = showingWarnText = gameModeText = soundsText = musicText = "";
                    switch (GameLogic.mControlType)
                    {
                        case Constants.CONTROL_KEYBOARD:
                            controlText = "Keyboard";
                            break;
                        case Constants.CONTROL_MOUSE:
                            controlText = "Mouse";
                            break;
                        case Constants.CONTROL_MIXED:
                            controlText = "Mixed";
                            break;
                    }
                    showingWarnText = GameLogic.mShowingWarnings ? "Yes" : "No";
                    soundsText = GameLogic.mPlaySounds ? "Yes" : "No";
                    musicText = GameLogic.mPlayMusic ? "Yes" : "No";
                    switch (GameLogic.mGameMode)
                    {
                        case Constants.MODE_NORMAL:
                            gameModeText = "Normal";
                            break;
                        case Constants.MODE_HARD:
                            gameModeText = "Hard";
                            break;
                        case Constants.MODE_TIME:
                            gameModeText = "Time fun";
                            break;
                    }


                    if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont72", "Back").X) / 2, (GUI.mHeight + 350) / 2, GUI.CheckTextSize("menuFont72", "Back"), mouseVect))
                    {
                        return 6;
                    }
                    else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Control: " + controlText).X) / 2, (GUI.mHeight - 250) / 2, GUI.CheckTextSize("menuFont42", "Control: " + controlText), mouseVect))
                    {
                        return 1;
                    }
                    else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Show warnings: " + showingWarnText).X) / 2, (GUI.mHeight - 150) / 2, GUI.CheckTextSize("menuFont42", "Show warnings: " + showingWarnText), mouseVect))
                    {
                        return 2;
                    }
                    else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Game mode: " + gameModeText).X) / 2, (GUI.mHeight - 50) / 2, GUI.CheckTextSize("menuFont42", "Game mode: " + gameModeText), mouseVect))
                    {
                        return 3;
                    }
                    else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Sounds: " + soundsText).X) / 2, (GUI.mHeight + 50) / 2, GUI.CheckTextSize("menuFont42", "Sounds: " + soundsText), mouseVect))
                    {
                        return 4;
                    }
                    else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Music: " + musicText).X) / 2, (GUI.mHeight + 150) / 2, GUI.CheckTextSize("menuFont42", "Music: " + musicText), mouseVect))
                    {
                        return 5;
                    }
                    break;
                case Constants.CREDITS:
                    if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont72", "Back").X) / 2, (GUI.mHeight + 450) / 2, GUI.CheckTextSize("menuFont72", "Back"), mouseVect))
                    {
                        return 1;
                    }
                    break;
                case Constants.CHOOSING:
                    int percentHeight = (int)(GUI.mHeight * 0.10);

                    if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont72", "Let's go!").X) / 2, (int)(GUI.mHeight - percentHeight * 2.5), GUI.CheckTextSize("menuFont72", "Let's go!"), mouseVect))
                    {
                        return 1;
                    }
                    else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Tutorial game!").X) / 2, (int)(GUI.mHeight - percentHeight * 1.5), GUI.CheckTextSize("menuFont42", "Tutorial game!"), mouseVect))
                    {
                        return 2;
                    }
                    break;
            }

            return 0;
        }

        /// <summary>
        /// Odpowiada za event myszy
        /// Rozpoznaje konkretne elementy na mapie i przekazuje wartość do warstwy logiki
        /// </summary>
        /// <returns>Zwraca listę intów, pierwszy element listy to kod eventu, drugi to dodatkowe dane</returns>
        static private List<int> HandleMouseEvent()
        {
            Vector2 playerGraphic;
            List<int> newInputList = new List<int>();
            Vector2 mouseVect = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released)
            {
                Ship playerShip = null;
                switch (actuallyScreen)
                {
                    case Constants.START:
                        int mainWidth = (int)GUI.CheckTextSize("menuFont90", "SpaceFight").X;
                        if (CheckClick((GUI.mWidth - mainWidth) / 2 + 30, (GUI.mHeight - 150) / 2, GUI.CheckTextSize("menuFont42", "New game"), mouseVect))
                        {
                            nextScreenToChange = Constants.CHOOSING;
                            newInputList.Add(0);
                        }
                        else if (CheckClick((GUI.mWidth - mainWidth) / 2 + 60, (GUI.mHeight - 50) / 2, GUI.CheckTextSize("menuFont42", "Exit"), mouseVect))
                        {
                            MainGame.exitGame = true;
                        }
                        else if (CheckClick((GUI.mWidth - mainWidth) / 2 + mainWidth - (int)GUI.CheckTextSize("menuFont42", "Options").X - 30, (GUI.mHeight - 150) / 2, GUI.CheckTextSize("menuFont42", "Options"), mouseVect))
                        {
                            nextScreenToChange = Constants.OPTIONS;
                            newInputList.Add(0);
                        }
                        else if (CheckClick((GUI.mWidth - mainWidth) / 2 + mainWidth - (int)GUI.CheckTextSize("menuFont42", "Credits").X - 60, (GUI.mHeight - 50) / 2, GUI.CheckTextSize("menuFont42", "Credits"), mouseVect))
                        {
                            nextScreenToChange = Constants.CREDITS;
                            newInputList.Add(0);
                        }
                        break;
                    case Constants.OPTIONS:
                        // z powodu zmiennego tekstu
                        string controlText, showingWarnText, gameModeText, soundsText, musicText;
                        controlText = showingWarnText = gameModeText = soundsText = musicText = "";
                        switch (GameLogic.mControlType)
                        {
                            case Constants.CONTROL_KEYBOARD:
                                controlText = "Keyboard";
                                break;
                            case Constants.CONTROL_MOUSE:
                                controlText = "Mouse";
                                break;
                            case Constants.CONTROL_MIXED:
                                controlText = "Mixed";
                                break;
                        }
                        showingWarnText = GameLogic.mShowingWarnings ? "Yes" : "No";
                        soundsText = GameLogic.mPlaySounds ? "Yes" : "No";
                        musicText = GameLogic.mPlayMusic ? "Yes" : "No";
                        switch (GameLogic.mGameMode)
                        {
                            case Constants.MODE_NORMAL:
                                gameModeText = "Normal";
                                break;
                            case Constants.MODE_HARD:
                                gameModeText = "Hard";
                                break;
                            case Constants.MODE_TIME:
                                gameModeText = "Time fun";
                                break;
                        }


                        if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont72", "Back").X) / 2, (GUI.mHeight + 350) / 2, GUI.CheckTextSize("menuFont72", "Back"), mouseVect))
                        {
                            nextScreenToChange = Constants.START;
                            newInputList.Add(0);
                        }
                        else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Control: " + controlText).X) / 2, (GUI.mHeight - 250) / 2, GUI.CheckTextSize("menuFont42", "Control: " + controlText), mouseVect))
                        {
                            if (GameLogic.mControlType == Constants.CONTROL_KEYBOARD)
                            {
                                GameLogic.mControlType = Constants.CONTROL_MOUSE;
                            } 
                            else if (GameLogic.mControlType == Constants.CONTROL_MOUSE)
                            {
                                GameLogic.mControlType = Constants.CONTROL_MIXED;
                            }
                            else if (GameLogic.mControlType == Constants.CONTROL_MIXED)
                            {
                                GameLogic.mControlType = Constants.CONTROL_KEYBOARD;
                            }
                            XmlTool.UpdateSettings("controlType", GameLogic.mControlType.ToString());
                            newInputList.Add(0);
                        }
                        else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Show warnings: " + showingWarnText).X) / 2, (GUI.mHeight - 150) / 2, GUI.CheckTextSize("menuFont42", "Show warnings: " + showingWarnText), mouseVect))
                        {
                            GameLogic.mShowingWarnings = !GameLogic.mShowingWarnings;
                            XmlTool.UpdateSettings("showingWarnings", Convert.ToInt32(GameLogic.mShowingWarnings).ToString());
                            newInputList.Add(0);
                        }
                        else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Game mode: " + gameModeText).X) / 2, (GUI.mHeight - 50) / 2, GUI.CheckTextSize("menuFont42", "Game mode: " + gameModeText), mouseVect))
                        {
                            if (GameLogic.mGameMode == Constants.MODE_NORMAL)
                            {
                                GameLogic.mGameMode = Constants.MODE_HARD;
                                GameLogic.mHighscore = GameLogic.gameModes[GameLogic.mGameMode].highscore;
                            }
                            else if (GameLogic.mGameMode == Constants.MODE_HARD)
                            {
                                GameLogic.mGameMode = Constants.MODE_TIME;
                            }
                            else if (GameLogic.mGameMode == Constants.MODE_TIME)
                            {
                                GameLogic.mGameMode = Constants.MODE_NORMAL;
                            }
                            XmlTool.UpdateSettings("gameMode", GameLogic.mGameMode.ToString());
                            newInputList.Add(0);
                        }
                        else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Sounds: " + soundsText).X) / 2, (GUI.mHeight + 50) / 2, GUI.CheckTextSize("menuFont42", "Sounds: " + soundsText), mouseVect))
                        {
                            GameLogic.mPlaySounds = !GameLogic.mPlaySounds;
                            XmlTool.UpdateSettings("playSounds", Convert.ToInt32(GameLogic.mPlaySounds).ToString());
                            newInputList.Add(0);
                        }
                        else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Music: " + musicText).X) / 2, (GUI.mHeight + 150) / 2, GUI.CheckTextSize("menuFont42", "Music: " + musicText), mouseVect))
                        {
                            GameLogic.mPlayMusic = !GameLogic.mPlayMusic;
                            XmlTool.UpdateSettings("playMusic", Convert.ToInt32(GameLogic.mPlayMusic).ToString());
                            newInputList.Add(0);
                        }
                        break;
                    case Constants.CREDITS:
                        if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont72", "Back").X) / 2, (GUI.mHeight + 450) / 2, GUI.CheckTextSize("menuFont72", "Back"), mouseVect))
                        {
                            nextScreenToChange = Constants.START;
                            newInputList.Add(0);
                        }
                        break;
                    case Constants.CHOOSING:
                        int percentHeight = (int)(GUI.mHeight * 0.10);

                        foreach (Ship ship in GameLogic.GetShips)
                        {
                            playerGraphic = GUI.GetGraphicInfo(ship.name);
                            // nie korzysta z funckji CheckClick, bo mija sie to z sensem
                            if ((mouseVect.X >= ship.vector.X - playerGraphic.X / 2 && mouseVect.X <= ship.vector.X + playerGraphic.X / 2) &&
                                (mouseVect.Y >= ship.vector.Y - playerGraphic.Y / 2 && mouseVect.Y <= ship.vector.Y + playerGraphic.Y / 2))
                            {
                                newInputList.Add(0);
                                chooseShip = ship;
                            }
                        }
                        if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont72", "Let's go!").X) / 2, (int)(GUI.mHeight - percentHeight * 2.5), GUI.CheckTextSize("menuFont72", "Let's go!"), mouseVect))
                        {
                            nextScreenToChange = Constants.GAME;
                            newInputList.Add(0);
                        }
                        else if (CheckClick((GUI.mWidth - (int)GUI.CheckTextSize("menuFont42", "Tutorial game!").X) / 2, (int)(GUI.mHeight - percentHeight * 1.5), GUI.CheckTextSize("menuFont42", "Tutorial game!"), mouseVect))
                        {
                            nextScreenToChange = Constants.TUTORIAL;
                            newInputList.Add(0);
                        }
                        break;
                    case Constants.TUTORIAL:
                        playerShip = GameLogic.GetPlayerShip();
                        playerGraphic = GUI.GetGraphicInfo(playerShip.name);

                        // rozpoczęcie tutorialu
                        if ((GameLogic.mControlType == Constants.CONTROL_MOUSE) && (GameLogic.mTutorialStep == 0))
                        {
                            newInputList.Add(Constants.NEXT_STEP);
                        }

                        if ((GameLogic.mControlType == Constants.CONTROL_MOUSE) && (GameLogic.mTutorialStep > 2))
                        {
                            // nie korzysta z funckji CheckClick, bo mija sie to z sensem
                            if ((mouseVect.X >= playerShip.vector.X - playerGraphic.X / 2 && mouseVect.X <= playerShip.vector.X + playerGraphic.X / 2) &&
                                (mouseVect.Y >= playerShip.vector.Y - playerGraphic.Y / 2 && mouseVect.Y <= playerShip.vector.Y + playerGraphic.Y / 2))
                            {
                                // miejsce dla eventu na statku
                            }
                            else
                            {
                                newInputList.Add(Constants.CREATE_MISSILE);
                            }
                        }
                        break;
                    case Constants.GAME:
                        playerShip = GameLogic.GetPlayerShip();
                        playerGraphic = GUI.GetGraphicInfo(playerShip.name);

                        if (GameLogic.mControlType == Constants.CONTROL_MOUSE)
                        {
                            // nie korzysta z funckji CheckClick, bo mija sie to z sensem
                            if ((mouseVect.X >= playerShip.vector.X - playerGraphic.X / 2 && mouseVect.X <= playerShip.vector.X + playerGraphic.X / 2) &&
                                (mouseVect.Y >= playerShip.vector.Y - playerGraphic.Y / 2 && mouseVect.Y <= playerShip.vector.Y + playerGraphic.Y / 2))
                            {
                                // miejsce dla eventu na statku
                            }
                            else
                            {
                                newInputList.Add(Constants.CREATE_MISSILE);
                            }
                        }
                        break;
                }
            }
            else
            {
                if ((actuallyScreen == Constants.GAME || (actuallyScreen == Constants.TUTORIAL && GameLogic.mTutorialStep > 0)) && (GameLogic.mControlType == Constants.CONTROL_MIXED || GameLogic.mControlType == Constants.CONTROL_MOUSE))
                {
                    GameLogic.SetMouseInfo(mouseVect);
                    newInputList.Add(Constants.MOVE_SHIP);
                }
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed &&
                previousMouseState.RightButton == ButtonState.Released)
            {
                if (GameLogic.mControlType == Constants.CONTROL_MOUSE)
                {
                    if (actuallyScreen == Constants.GAME)
                    {
                        newInputList.Add(Constants.TURBO_MODE);
                    }

                    if (actuallyScreen == Constants.TUTORIAL && GameLogic.mTutorialStep > 1)
                    {
                        newInputList.Add(Constants.TURBO_MODE);
                    }
                }
            }


            return newInputList;
        }

        /// <summary>
        /// Odpowiada za eventy klawiatury
        /// </summary>
        /// <returns>Lista przechwyconych kodów</returns>
        static private List<int> HandleKeyboardEvent()
        {
            List<int> newInputList = new List<int>();
            KeyboardState keyState = Keyboard.GetState();

            if (actuallyScreen != Constants.START)
            {
                if (keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape))
                {
                    nextScreenToChange = Constants.START;
                    newInputList.Add(0);
                }
            }

            if (actuallyScreen == Constants.GAME)
            {
                if (GameLogic.mControlType == Constants.CONTROL_KEYBOARD || GameLogic.mControlType == Constants.CONTROL_MIXED)
                {
                    if (keyState.IsKeyDown(Keys.Space) && previousKeyState.IsKeyUp(Keys.Space))
                    {
                        newInputList.Add(Constants.CREATE_MISSILE);
                    }

                    if (keyState.IsKeyDown(Keys.Z))
                    {
                        newInputList.Add(Constants.TURBO_MODE);
                    }    
                }

                if (GameLogic.mControlType == Constants.CONTROL_KEYBOARD)
                {
                    if (keyState.IsKeyDown(Keys.Left))
                    {
                        newInputList.Add(Constants.LEFT);
                    }

                    if (keyState.IsKeyDown(Keys.Right))
                    {
                        newInputList.Add(Constants.RIGHT);
                    }

                    if (keyState.IsKeyDown(Keys.Up))
                    {
                        newInputList.Add(Constants.FORWARD);
                    }

                    if (keyState.IsKeyDown(Keys.X) && previousKeyState.IsKeyUp(Keys.X))
                    {
                        newInputList.Add(Constants.RUSH);
                    }

                    if (keyState.IsKeyUp(Keys.Up) && previousKeyState.IsKeyDown(Keys.Up))
                    {
                        newInputList.Add(Constants.FORWARD_ECHO);
                    }
                }  
            }
            else if (actuallyScreen == Constants.TUTORIAL)
            {
                if ((GameLogic.mControlType == Constants.CONTROL_KEYBOARD || GameLogic.mControlType == Constants.CONTROL_MIXED) &&
                    (keyState.IsKeyDown(Keys.Space) && previousKeyState.IsKeyUp(Keys.Space)) && 
                    (GameLogic.mTutorialStep == 0))
                {
                    newInputList.Add(Constants.NEXT_STEP);
                }

                if ((GameLogic.mControlType == Constants.CONTROL_KEYBOARD || GameLogic.mControlType == Constants.CONTROL_MIXED))
                {
                    if (keyState.IsKeyDown(Keys.Space) && previousKeyState.IsKeyUp(Keys.Space) && (GameLogic.mTutorialStep > 2))
                    {
                        newInputList.Add(Constants.CREATE_MISSILE);
                    }

                    if (keyState.IsKeyDown(Keys.Z) && (GameLogic.mTutorialStep > 1))
                    {
                        newInputList.Add(Constants.TURBO_MODE);
                    }
                }

                if (GameLogic.mControlType == Constants.CONTROL_KEYBOARD && (GameLogic.mTutorialStep > Constants.TUTORIAL_INIT))
                {
                    if (keyState.IsKeyDown(Keys.Left))
                    {
                        newInputList.Add(Constants.LEFT);
                    }

                    if (keyState.IsKeyDown(Keys.Right))
                    {
                        newInputList.Add(Constants.RIGHT);
                    }

                    if (keyState.IsKeyDown(Keys.Up))
                    {
                        newInputList.Add(Constants.FORWARD);
                    }

                    if (keyState.IsKeyDown(Keys.X) && previousKeyState.IsKeyUp(Keys.X))
                    {
                        newInputList.Add(Constants.RUSH);
                    }

                    if (keyState.IsKeyUp(Keys.Up) && previousKeyState.IsKeyDown(Keys.Up))
                    {
                        newInputList.Add(Constants.FORWARD_ECHO);
                    }
                }  
            }

            return newInputList;
        }

        /// <summary>
        /// Sprawdza, czy klikniecie miesci sie w polu
        /// </summary>
        /// <param name="x">Górny róg (x) pola</param>
        /// <param name="y">Górny róg (y) pola</param>
        /// <param name="dVect">Wektor z zakresem pola</param>
        /// <param name="mouse">Klikniecie</param>
        /// <returns>true - kliknięcie w pole, false - poza pole</returns>
        static private bool CheckClick(int x, int y, Vector2 dVect, Vector2 mouse)
        {
            if ((mouse.X >= x && mouse.X <= x + dVect.X) && (mouse.Y >= y && mouse.Y <= y + dVect.Y))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Przekazuje dane z GUI
        /// Funkcja służy tylko do komunikacji między GameLogic, a GUI.
        /// </summary>
        /// <returns></returns>
        static public Vector2 TransferGraphicInfo(string name)
        {
            return GUI.GetGraphicInfo(name);
        }

        /// <summary>
        /// Aktualizuje odpowiednie highscore
        /// </summary>
        static private void UpdateHighscore()
        {
            if (GameLogic.mActuallyPoints > GameLogic.mHighscore)
            {
                GameLogic.mHighscore = GameLogic.mActuallyPoints;
                switch (GameLogic.mGameMode)
                {
                    case Constants.MODE_NORMAL:
                        XmlTool.UpdateSettings("highscoreNormal", GameLogic.mActuallyPoints.ToString());
                        break;
                    case Constants.MODE_HARD:
                        XmlTool.UpdateSettings("highscoreHard", GameLogic.mActuallyPoints.ToString());
                        break;
                    case Constants.MODE_TIME:
                        XmlTool.UpdateSettings("highscoreTime", GameLogic.mActuallyPoints.ToString());
                        break;
                }
            }
        }
    }
}
