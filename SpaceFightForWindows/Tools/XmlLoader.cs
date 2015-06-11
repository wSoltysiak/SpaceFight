using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SpaceShooter.ElementsClass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;

namespace SpaceShooter.Tools
{
    class XmlTool
    {
        /// <summary>
        /// Przetwarza xml z właściwościami staków na dane.
        /// </summary>
        /// <returns>Lista asocjacyjna z właściwościami statków</returns>
        static public Dictionary<string, Property> LoadShipProperty()
        {
            Dictionary<string, Property> propertyList = new Dictionary<string, Property>();
            XDocument xDoc = XDocument.Load("Content/xml/ShipProperty.xml");
            string name, speed, gunCount, shield, points;

            foreach (XElement ship in xDoc.Elements("shipProperties").Elements("ship"))
            {
                name = ship.Attribute("name").Value;
                speed = ship.Element("speed").Value;
                gunCount = ship.Element("gunCount").Value;
                shield = ship.Element("shield").Value;
                points = ship.Element("points").Value;

                Property newProperty = new Property(Convert.ToDouble(speed), Convert.ToInt32(gunCount), Convert.ToInt32(shield), Convert.ToInt32(points));
                propertyList.Add(name, newProperty);
            }

            return propertyList;
        }

        /// <summary>
        /// Przetwarza xml kolorów cząsteczek i zapisuje je do listy.
        /// </summary>
        /// <returns>Lista asocjacyjna kolorów</returns>
        static public Dictionary<string, List<Color>> LoadColors()
        {
            Dictionary<string, List<Color>> colors = new Dictionary<string, List<Color>>();
            List<Color> colorTab = new List<Color>();
            XDocument xDoc = XDocument.Load("Content/xml/ColorList.xml");
            string name, r, g, b;

            foreach (XElement colorList in xDoc.Elements("Colors").Elements("ColorList"))
            {
                colorTab = new List<Color>();
                name = colorList.Attribute("name").Value;
                foreach (XElement color in colorList.Elements("color"))
                {
                    r = color.Element("r").Value;
                    g = color.Element("g").Value;
                    b = color.Element("b").Value;
                    Color newColor = new Color(Convert.ToInt32(r), Convert.ToInt32(g), Convert.ToInt32(b));
                    colorTab.Add(newColor);
                }
                colors.Add(name, colorTab);
            }
            List<Color> andrzej = colors["engineColors"];
            return colors;
        }

        /// <summary>
        /// Tworzy plik ustawień
        /// </summary>
        static public void CreateSettingsFile()
        {
            using (FileStream stream = new FileStream(@"Settings.xml", FileMode.Create))
            {
                stream.Position = 0;
                XDocument xDoc = new XDocument(
                    new XDeclaration("1.0", "utf8", "yes"),
                    new XElement("settings",
                            new XElement("highscoreNormal", "0"),
                            new XElement("highscoreHard", "0"),
                            new XElement("highscoreTime", "0"),
                            new XElement("controlType", "401"),
                            new XElement("showingWarnings", "1"),
                            new XElement("playSounds", "1"),
                            new XElement("playMusic", "1"),
                            new XElement("gameMode", "701")
                        )
                  );
                stream.Seek(0, SeekOrigin.Begin);
                xDoc.Save(stream);
                stream.SetLength(stream.Position);
            }
        }

        /// <summary>
        /// Sprawdza, czy plik istnieje
        /// </summary>
        /// <param name="fileName">Nazwa pliku</param>
        /// <returns></returns>
        static public bool IsFileExists(string fileName)
        {
            return File.Exists(Directory.GetCurrentDirectory() + "\\" + fileName);
        }

        /// <summary>
        /// Przetwarza xml ustawień gry na liste 
        /// </summary>
        /// <returns>Lista asocjacyjna z ustawieniami</returns>
        static public Dictionary<string, int> LoadSettings()
        {
            Dictionary<string, int> settings = new Dictionary<string, int>();
            if (!IsFileExists("Settings.xml"))
            {
                CreateSettingsFile();
                settings.Add("highscoreNormal", 0);
                settings.Add("highscoreHard", 0);
                settings.Add("highscoreTime", 0);
                settings.Add("controlType", 401);
                settings.Add("showingWarnings", 1);
                settings.Add("playSounds", 1);
                settings.Add("playMusic", 1);
                settings.Add("gameMode", 701);
            }
            else
            {
                XDocument xDoc = XDocument.Load(Directory.GetCurrentDirectory() + "\\Settings.xml");
                settings.Add("highscoreNormal", Convert.ToInt32(xDoc.Element("settings").Element("highscoreNormal").Value));
                settings.Add("highscoreHard", Convert.ToInt32(xDoc.Element("settings").Element("highscoreHard").Value));
                settings.Add("highscoreTime", Convert.ToInt32(xDoc.Element("settings").Element("highscoreTime").Value));
                settings.Add("controlType", Convert.ToInt32(xDoc.Element("settings").Element("controlType").Value));
                settings.Add("showingWarnings", Convert.ToInt32(xDoc.Element("settings").Element("showingWarnings").Value));
                settings.Add("playSounds", Convert.ToInt32(xDoc.Element("settings").Element("playSounds").Value));
                settings.Add("playMusic", Convert.ToInt32(xDoc.Element("settings").Element("playMusic").Value));
                settings.Add("gameMode", Convert.ToInt32(xDoc.Element("settings").Element("gameMode").Value));
            }

            return settings;
        }

        /// <summary>
        /// Aktualizuje xml z ustawieniami
        /// </summary>
        /// <param name="name">Nazwa pola</param>
        /// <param name="value">Wartość pola</param>
        static public void UpdateSettings(string name, string value)
        {
            using (Stream stream = new FileStream(@"Settings.xml", FileMode.Open, FileAccess.ReadWrite))
            {
                stream.Position = 0;
                XDocument xDoc = XDocument.Load(stream);
                xDoc.Element("settings").Element(name).Value = value;
                stream.Seek(0, SeekOrigin.Begin);
                xDoc.Save(stream);
                stream.SetLength(stream.Position);
            }
        }
    }
}
