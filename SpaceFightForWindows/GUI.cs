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
    /// Klasa służąca do ładowania i wyświetlania wszlekiej grafiki
    /// </summary>
    class GUI
    {
        static private int width;
        static private int height;

        static public int mWidth
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        static public int mHeight
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }
        // kolekcje trzymające grafiki
        static private Dictionary<string, Texture2D> menuGraphic = new Dictionary<string, Texture2D>();
        static private Dictionary<string, Texture2D> shipsGraphic = new Dictionary<string, Texture2D>();
        static private Dictionary<string, Texture2D> missilesGraphic = new Dictionary<string, Texture2D>();
        static private Dictionary<string, Texture2D> gravityAreaGraphic = new Dictionary<string, Texture2D>();
        static private Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();
        static private Texture2D particleTexture = null;
        static private Texture2D warningTexture = null;


        // ################################
        // #### Metody ładujące zasoby ####
        // ################################


        /// <summary>
        /// Funkcja odpowiada za wczytywanie grafik
        /// </summary>
        /// <param name="Content"></param>
        static public void LoadImages(ContentManager Content)
        {
            // statki
            shipsGraphic.Add("Pship1", Content.Load<Texture2D>("img/ships/playerShips/Pship1"));
            shipsGraphic.Add("Pship2", Content.Load<Texture2D>("img/ships/playerShips/Pship2"));
            shipsGraphic.Add("Pship3", Content.Load<Texture2D>("img/ships/playerShips/Pship3"));
            shipsGraphic.Add("Pship4", Content.Load<Texture2D>("img/ships/playerShips/Pship4"));

            shipsGraphic.Add("Eship1", Content.Load<Texture2D>("img/ships/enemyShips/Eship1"));
            shipsGraphic.Add("Eship2", Content.Load<Texture2D>("img/ships/enemyShips/Eship2"));
            shipsGraphic.Add("Eship3", Content.Load<Texture2D>("img/ships/enemyShips/Eship3"));
            shipsGraphic.Add("Eship4", Content.Load<Texture2D>("img/ships/enemyShips/Eship4"));

            shipsGraphic.Add("Eship1Radar", Content.Load<Texture2D>("img/ships/radarShips/Rship1"));
            shipsGraphic.Add("Eship2Radar", Content.Load<Texture2D>("img/ships/radarShips/Rship2"));
            shipsGraphic.Add("Eship3Radar", Content.Load<Texture2D>("img/ships/radarShips/Rship3"));
            shipsGraphic.Add("Eship4Radar", Content.Load<Texture2D>("img/ships/radarShips/Rship4"));
            
            // pociski
            missilesGraphic.Add("missile1", Content.Load<Texture2D>("img/missiles/missile"));
            missilesGraphic.Add("missile_radar", Content.Load<Texture2D>("img/missiles/missile_radar"));

            // pola grawitacyjne
            gravityAreaGraphic.Add("gravityArea1", Content.Load<Texture2D>("img/gravityAreas/gravityArea"));

            // inne
            particleTexture = Content.Load<Texture2D>("img/particle");
            warningTexture = Content.Load<Texture2D>("img/warning");
            
        }

        /// <summary>
        /// Ładowanie czcionek
        /// </summary>
        /// <param name="Content"></param>
        static public void LoadFonts(ContentManager Content)
        {
            fonts.Add("font1", Content.Load<SpriteFont>("font/font1"));
            fonts.Add("font2", Content.Load<SpriteFont>("font/font2"));
            fonts.Add("menuFont42", Content.Load<SpriteFont>("font/menuFont42"));
            fonts.Add("menuFont72", Content.Load<SpriteFont>("font/menuFont72"));
            fonts.Add("menuFont90", Content.Load<SpriteFont>("font/menuFont90"));
        }


        // #########################
        // #### Metody rysujące ####
        // #########################


        /// <summary>
        /// Tworzy koło lub jego element
        /// </summary>
        /// <param name="r">Promień</param>
        /// <param name="graphicDevice">GraphicDevice</param>
        /// <param name="divider">Określa odcinek, ktory ma być rysowany - domyślnie rysowana jest całość</param>
        /// <returns>Tekstura koła</returns>
        static public Texture2D CreateCircle(int r, GraphicsDevice graphicDevice, double divider = 1)
        {
            int outerRadius = r * 2 + 2;
            Texture2D texture = new Texture2D(graphicDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // tworzenie przezroczystego tła
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Color.Transparent;
            }

            // rysowanie koła
            double angleStep = 1f / r;
            int x, y;
            for (double angle = 0; angle < Math.PI * 2 / divider; angle += angleStep)
            {
                x = (int)Math.Round(r + r * Math.Cos(angle));
                y = (int)Math.Round(r + r * Math.Sin(angle));
                data[y * outerRadius + x + 1] = Color.White;
                data[y * outerRadius + x + 1] = Color.White;
            }
            

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Rysuje menu
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="highsocre">Aktualny rekord</param>
        /// <param name="actuallyScreen">Aktualny ekran</param>
        /// <param name="controlType">Typ sterowania</param>
        /// <param name="gameMode">Tryb gry</param>
        /// <param name="highscore">Wynik</param>
        /// <param name="hoverElement">Element do podświetlenia</param>
        /// <param name="playMusic"></param>
        /// <param name="playSounds"></param>
        /// <param name="showingWarnings"></param>
        static public void DrawMenu(ref SpriteBatch spriteBatch, int highscore, int actuallyScreen, int controlType, bool showingWarnings, int gameMode, bool playSounds, bool playMusic, int hoverElement = 0)
        {
            Color color = Color.White;
            switch (actuallyScreen)
            {   
                case Constants.START:
                    int mainWidth = (int)(fonts["menuFont90"].MeasureString("SpaceFight").X);
                    DrawText(ref spriteBatch, "menuFont90", (mHeight - 350) / 2, "SpaceFight", Color.White);
                    if (hoverElement == 1)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont42", (mWidth - mainWidth) / 2 + 30, (mHeight - 150) / 2, "New game", color);
                    color = Color.White;
                    if (hoverElement == 2)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont42", (mWidth - mainWidth) / 2 + 60, (mHeight - 50) / 2, "Exit", color);
                    color = Color.White;
                    if (hoverElement == 3)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont42", (mWidth - mainWidth) / 2 + mainWidth - (int)(fonts["menuFont42"].MeasureString("Options").X) - 30, (mHeight - 150) / 2, "Options", color);
                    color = Color.White;
                    if (hoverElement == 4)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont42", (mWidth - mainWidth) / 2 + mainWidth - (int)(fonts["menuFont42"].MeasureString("Credits").X) - 60, (mHeight - 50) / 2, "Credits", color);
                    color = Color.White;
                    DrawText(ref spriteBatch, "menuFont72", (mHeight + 200) / 2, "Highscore " + highscore.ToString(), Color.White);
                    break;
                case Constants.CHOOSING:
                    int percentHeight = (int)(mHeight * 0.10);
                    DrawText(ref spriteBatch, "menuFont90", percentHeight, "Choose ship:", Color.White);
                    if (hoverElement == 1)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont72", (int)(mHeight - percentHeight * 2.5), "Let's go!", color);
                    color = Color.White;
                    if (hoverElement == 2)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont42", (int)(mHeight - percentHeight * 1.5), "Tutorial game!", color);
                    break;
                case Constants.CREDITS:
                    DrawText(ref spriteBatch, "menuFont90", (mHeight - 650) / 2, "Credits", Color.White);
                    DrawText(ref spriteBatch, "menuFont42", (mHeight - 450) / 2, "Graphic:", Color.White);
                    DrawText(ref spriteBatch, "menuFont42", (mHeight - 350) / 2, "Stephen Challener", Color.White);
                    DrawText(ref spriteBatch, "menuFont42", (mHeight - 230) / 2, "Music:", Color.White);
                    DrawText(ref spriteBatch, "menuFont42", (mHeight - 130) / 2, "Ryan Burks", Color.White);
                    DrawText(ref spriteBatch, "menuFont42", (mHeight - 10) / 2, "Sounds:", Color.White);
                    DrawText(ref spriteBatch, "menuFont42", (mHeight + 90) / 2, "Iwan Gabovitch", Color.White);
                    DrawText(ref spriteBatch, "menuFont42", (mHeight + 210) / 2, "Programming:", Color.White);
                    DrawText(ref spriteBatch, "menuFont42", (mHeight + 310) / 2, "Wojciech Soltysiak", Color.White);
                    if (hoverElement == 1)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont72", (mHeight + 450) / 2, "Back", color);
                    break;
                case Constants.OPTIONS:
                    string controlText = "";
                    string showingWarnText, soundsText, musicText;
                    string gameModeText = "";

                    switch (controlType)
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

                    showingWarnText = showingWarnings ? "Yes" : "No";
                    soundsText = playSounds ? "Yes" : "No";
                    musicText = playMusic ? "Yes" : "No";

                    switch (gameMode)
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

                    DrawText(ref spriteBatch, "menuFont90", (mHeight - 450) / 2, "Options", Color.White);
                    if (hoverElement == 1)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont42", (mHeight - 250) / 2, "Control: " + controlText, color);
                    color = Color.White;
                    if (hoverElement == 2)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont42", (mHeight - 150) / 2, "Show warnings: " + showingWarnText, color);
                    color = Color.White;
                    if (hoverElement == 3)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont42", (mHeight - 50) / 2, "Game mode: " + gameModeText, color);
                    color = Color.White;
                    if (hoverElement == 4)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont42", (mHeight + 50) / 2, "Sounds: " + soundsText, color);
                    color = Color.White;
                    if (hoverElement == 5)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont42", (mHeight + 150) / 2, "Music: " + musicText, color);
                    color = Color.White;
                    if (hoverElement == 6)
                    {
                        color = Color.Silver;
                    }
                    DrawText(ref spriteBatch, "menuFont72", (mHeight + 350) / 2, "Back", color);
                    break;
            }
        }

        /// <summary>
        /// Rysuje komunikaty tutoriala
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="tutorialStep">Aktualny krok</param>
        static public void DrawTutorial(ref SpriteBatch spriteBatch, GraphicsDevice graphicDevice, int tutorialStep, int controlType)
        {
            string stepText1, stepText2;
            stepText1 = stepText2 = "";

            switch (tutorialStep)
            {
                case Constants.TUTORIAL_INIT:
                    if (controlType == Constants.CONTROL_MOUSE)
                    {
                        stepText1 = "Press left mouse button to begin tutorial";
                    }
                    else
                    {
                        stepText1 = "Press SPACE to begin tutorial";
                    }
                    stepText2 = "";
                    break;
                case 1:
                    if (controlType == Constants.CONTROL_KEYBOARD)
                    {
                        stepText1 = "Use arrows to move ship and X to spin.";
                    }
                    else
                    {
                        stepText1 = "Your ship move to mouse cursor.";    
                    }
                    stepText2 = "Go to green area!";
                    DrawRectangle(ref spriteBatch, graphicDevice, 50, 50, 300, 200);
                    break;
                case 2:
                    if (controlType == Constants.CONTROL_MOUSE)
                    {
                        stepText1 = "Press right mouse button to use boost.";
                    }
                    else
                    {
                        stepText1 = "Press Z to use boost.";
                    }
                    stepText2 = "Go to green area!";
                    DrawRectangle(ref spriteBatch, graphicDevice, mWidth - 350, mHeight - 250, 300, 200);
                    break;
                case 3:
                    if (controlType == Constants.CONTROL_MOUSE)
                    {
                        stepText1 = "Press left mouse button to shoot.";
                    }
                    else
                    {
                        stepText1 = "Press SPACE to shoot.";
                    }
                    stepText2 = "Destroy green ship!";
                    break;
                case 4:
                    stepText1 = "";
                    stepText2 = "FIGHT!";
                    break;
                case 5:
                    stepText1 = "";
                    stepText2 = "";
                    break;
            }

            DrawText(ref spriteBatch, "menuFont42", (mHeight - 350) / 2, stepText1, Color.White);
            DrawText(ref spriteBatch, "menuFont72", (mHeight - 200) / 2, stepText2, Color.White);
        }

        /// <summary>
        /// Rysuje informacje dotyczące rozgrywki
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="actuallyPoints">Ilość aktualnie zdobytych punktów</param>
        /// <param name="actuallyCombo">Mnożnik combo</param>
        static public void DrawGameInfo(ref SpriteBatch spriteBatch, int actuallyPoints, int actuallyCombo) 
        {
            GUI.DrawText(ref spriteBatch, "font1", 10, mHeight - 30, "Score: " + actuallyPoints.ToString(), Color.White);
            GUI.DrawText(ref spriteBatch, "font1", 10, mHeight - 60, "Combo: x" + actuallyCombo.ToString(), Color.White);
        }

        /// <summary>
        /// Rysuje pasek przyśpieszenia
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="graphicDevice"></param>
        /// <param name="player">Statek gracza</param>
        static public void DrawTurboPowerBar(ref SpriteBatch spriteBatch, GraphicsDevice graphicDevice, Ship player)
        {
            double divider = 3.5 / (player.turboPower / Constants.maxTurboPower);
            Texture2D circle = CreateCircle(30, graphicDevice, divider);

            Vector2 vect = new Vector2(player.vector.X - 33, player.vector.Y - 33);
            spriteBatch.Begin();
            spriteBatch.Draw(circle, vect, Color.DarkKhaki);
            spriteBatch.End();
        }

        /// <summary>
        /// Rysuje pasek osłon
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="graphicDevice">GraphicDevice</param>
        /// <param name="player">Obiekt statku gracza</param>
        static public void DrawShieldBar(ref SpriteBatch spriteBatch, GraphicsDevice graphicDevice, Ship player)
        {
            double a = player.property.shield == 0 ? 0.01 : (double)player.property.shield / Constants.maxShield;
            double divider = 1.5 / a;
            Texture2D circle = CreateCircle(32, graphicDevice, divider);

            Vector2 vect = new Vector2(player.vector.X - 33, player.vector.Y - 33);
            spriteBatch.Begin();
            spriteBatch.Draw(circle, vect, Color.DarkCyan);
            spriteBatch.End();
        }

        /// <summary>
        /// Rysuje kwadrat
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="graphicDevice">GraphicsDevice</param>
        /// <param name="x">Położenie na osi X</param>
        /// <param name="y">Położenie na osi Y</param>
        /// <param name="w">Szerokość</param>
        /// <param name="h">Wysokość</param>
        static public void DrawRectangle(ref SpriteBatch spriteBatch, GraphicsDevice graphicDevice, int x, int y, int w, int h)
        {
            Texture2D rect = new Texture2D(graphicDevice, w, h);

            Color[] data = new Color[w * h];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.PaleGreen;
            }
            rect.SetData(data);

            Vector2 coor = new Vector2(x, y);
            spriteBatch.Begin();
            spriteBatch.Draw(rect, coor, Color.White * 0.1f);
            spriteBatch.End();
        }

        /// <summary>
        /// Pobieranie wiekości dowolnej grafiki
        /// </summary>
        /// <param name="name">Nazwa pliku</param>
        /// <returns>Wektor z szerokością i wysokością grafiki</returns>
        static public Vector2 GetGraphicInfo(string name)
        {
            Vector2 vect;
            Texture2D texture = null;

            gravityAreaGraphic.TryGetValue(name, out texture);
            if (texture != null)
            {
                vect = new Vector2(texture.Width, texture.Height);
                return vect;
            }

            missilesGraphic.TryGetValue(name, out texture);
            if (texture != null)
            {
                vect = new Vector2(texture.Width, texture.Height);
                return vect;
            }

            shipsGraphic.TryGetValue(name, out texture);
            if (texture != null)
            {
                vect = new Vector2(texture.Width, texture.Height);
                return vect;
            }

            vect = new Vector2(0, 0);
            return vect;
        }

        /// <summary>
        /// Rysuje wszystkie statki na mapie
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="ships">Lista statków</param>
        /// <param name="actuallyScreen">Aktualty ekran</param>
        /// <param name="chooseID">ID wybranego statku (w wypadku ekranu wybierania)</param>
        static public void DrawShips(ref SpriteBatch spriteBatch, List<Ship> ships, int actuallyScreen = 0, int chooseID = 0)
        {
            Vector2 origin;
            Vector2 location;
            Rectangle sourceRectangle;

            spriteBatch.Begin();
            foreach (Ship ship in ships)
            {
                location = ship.vector;
                origin = new Vector2(shipsGraphic[ship.name].Width / 2, shipsGraphic[ship.name].Height / 2);
                sourceRectangle = new Rectangle(0, 0, shipsGraphic[ship.name].Width, shipsGraphic[ship.name].Height);
                if (ship.toDraw == Constants.DRAW)
                {
                    if (!ship.extras)
                    {
                        if (actuallyScreen == Constants.CHOOSING && ship.ID == chooseID)
                        {
                            spriteBatch.Draw(shipsGraphic[ship.name], location, sourceRectangle, Color.White, ship.mAngle, origin, 1.2f, SpriteEffects.None, 1);
                        }
                        else 
                        {
                            // zwykłe wyświetlanie
                            spriteBatch.Draw(shipsGraphic[ship.name], location, sourceRectangle, Color.White, ship.mAngle, origin, 1.0f, SpriteEffects.None, 1);
                        }
                    }
                    else
                    {
                        if (ship.blink < 10)
                        {
                            spriteBatch.Draw(shipsGraphic[ship.name], location, sourceRectangle, Color.DeepSkyBlue, ship.mAngle, origin, 1.0f, SpriteEffects.None, 1);
                            ship.blink++;
                        }
                        else
                        {
                            spriteBatch.Draw(shipsGraphic[ship.name], location, sourceRectangle, Color.CornflowerBlue, ship.mAngle, origin, 1.0f, SpriteEffects.None, 1);
                            ship.blink++;
                            if (ship.blink == 20)
                            {
                                ship.blink = 0;
                            }
                        }
                    }
                }
                // nieaktualny kod dotyczący radarów
                /*else if (ship.toDraw == Constants.RADAR_DRAW)
                {
                    if (!ship.extras)
                    {
                        spriteBatch.Draw(shipsGraphic[ship.name + "Radar"], location, sourceRectangle, Color.White, ship.mAngle, origin, 1.0f, SpriteEffects.None, 1);
                    }
                    else
                    {
                        if (ship.blink < 10)
                        {
                            spriteBatch.Draw(shipsGraphic[ship.name + "Radar"], location, sourceRectangle, Color.DeepSkyBlue, ship.mAngle, origin, 1.0f, SpriteEffects.None, 1);
                            ship.blink++;
                        }
                        else
                        {
                            spriteBatch.Draw(shipsGraphic[ship.name + "Radar"], location, sourceRectangle, Color.CornflowerBlue, ship.mAngle, origin, 1.0f, SpriteEffects.None, 1);
                            ship.blink++;
                            if (ship.blink == 20)
                            {
                                ship.blink = 0;
                            }
                        }
                    }
                    
                }*/
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Rysowanie rakiet na mapie
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="missiles">Lista pocisków</param>
        static public void DrawMissiles(ref SpriteBatch spriteBatch, List<Missile> missiles)
        {
            spriteBatch.Begin();
            foreach (Missile missile in missiles)
            {
                if (missile.toDraw == Constants.DRAW)
                {
                    spriteBatch.Draw(missilesGraphic["missile1"], missile.vector, Color.White);
                }
                // nieaktualny kod dotyczący radarów
                /*else if (missile.toDraw == Constants.RADAR_DRAW)
                {
                    spriteBatch.Draw(missilesGraphic["missile_radar"], missile.vector, Color.White);
                }*/
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Rysowanie pól grawitacyjnych
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="gravityAreas">Lista pól grawitacyjnych</param>
        static public void DrawGravityAreas(ref SpriteBatch spriteBatch, List<GravityArea> gravityAreas)
        {
            spriteBatch.Begin();
            foreach (GravityArea gravityArea in gravityAreas)
            {
                spriteBatch.Draw(gravityAreaGraphic[gravityArea.name], gravityArea.vector, Color.White);
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Rysuje wszystkie cząsteczki
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="graphicDevice">GraphicDevice</param>
        /// <param name="particles">Lista wszystkich cząsteczek</param>
        /// <param name="idGroup">ID grupy cząsteczek, jeśli -1 to rysuje wszystkie</param>
        static public void DrawParticles(ref SpriteBatch spriteBatch, Particle[] particles, int idGroup = -1)
        {
            int index;
            double div;

            spriteBatch.Begin();
            foreach (Particle particle in particles)
            {
                if (particle != null)
                {
                    if ((particle.type == idGroup) || (idGroup == -1))
                    {
                        index = 0;
                        div = (double)particle.life / (double)particle.maxLife;
                        for (int i = particle.colorList.Count - 1; i > 0; i--)
                        {
                            if (div > ((double)i / (double)particle.colorList.Count))
                            {
                                index = i;
                                break;
                            }
                        }
                        spriteBatch.Draw(particleTexture, particle.vector, null, particle.colorList[index], 0f, Vector2.Zero, (float)particle.size, SpriteEffects.None, 0f);
                    }
                }
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Umieszczanie na ekranie tekstu
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="fontName">Nazwa czcionki</param>
        /// <param name="x">oś x</param>
        /// <param name="y">oś y</param>
        /// <param name="text">Tekst do wyświetlenia</param>
        static public void DrawText(ref SpriteBatch spriteBatch, string fontName, int x, int y, string text, Color color)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(fonts[fontName], text, new Vector2(x, y), color);
            spriteBatch.End();
        }

        /// <summary>
        /// Umieszczanie na ekranie tekstu, idealnie na środku
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="fontName">Nazwa czcionki</param>
        /// <param name="y">oś y</param>
        /// <param name="text">Tekst do wyświetlenia</param>
        static public void DrawText(ref SpriteBatch spriteBatch, string fontName, int y, string text, Color color)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(fonts[fontName], text, new Vector2((mWidth - (int)(fonts[fontName].MeasureString(text).X)) / 2, y), color);
            spriteBatch.End();
        }

        /// <summary>
        /// Rysuje ostrzeżenie dla gracza
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="type">Typ ostrzeżenia</param>
        /// <param name="playerShip">Obiekt statku gracza</param>
        static public void DrawWarnings(ref SpriteBatch spriteBatch, int type, Ship playerShip)
        {
            Vector2 warningVector = new Vector2(0, 0);
            string warningMsg = "";
            bool reverse = false;

            // sprawdzanie czy gracz jest w polu ostrzeżeń
            if (playerShip.vector.X > mWidth - 700 && playerShip.vector.Y > mHeight - 250)
            {
                reverse = true;
            }

            if (type != Constants.EMPTY_WARNING)
            {
                switch (type)
                {
                    case Constants.SHIELD_WARNING:
                        warningMsg = "Your shield is weak!";
                        warningVector.X = mWidth - 400;
                        break;
                    case Constants.GRAVITY_WARNING:
                        warningMsg = "You are too near gravity area!";
                        warningVector.X = mWidth - 550;
                        break;
                }

                // przenoszenie ostrzeżeń na drugi kraniec ekranu, żeby nie przeszkadzał
                if (reverse)
                {
                    warningVector.X = 20;
                    warningVector.Y = 20;
                }
                else
                {
                    warningVector.Y = mHeight - 90;
                }

                spriteBatch.Begin();
                spriteBatch.Draw(warningTexture, warningVector, Color.White);
                spriteBatch.End();
                DrawText(ref spriteBatch, "font1", (int)(warningVector.X + 80), (int)(warningVector.Y + 15), warningMsg, Color.White);
            }
        }


        /// #############################
        /// #### Metody przekazujące ####
        /// #############################

        /// <summary>
        /// Sprawdza rozmiar tekstu
        /// Specjalnie dla Controlera
        /// </summary>
        /// <returns>Wektor z wielkością tekstu w danej czcionce</returns>
        static public Vector2 CheckTextSize(string fontName, string text)
        {
            return fonts[fontName].MeasureString(text);
        }
    }
}
