using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Windows.Media.Streaming.Adaptive;

namespace NutzMich.Shared.Models
{
    public class Kategorie
    {
        static Kategorie()
        {
            FillKategorien();
        }

        public string ID { get; set; }
        public string Name { get; set; }

        public Kategorie(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public static Dictionary<string, string> Kategorien = new Dictionary<string, string>();
        private static void FillKategorien()
        {
            Kategorie.Kategorien.Add("Indoor", "Indoor & innen");
            Kategorie.Kategorien.Add("Outdoor", "Outdoor & außen");
            Kategorie.Kategorien.Add("ElektronikMultimedia", "Elektronik & Multimedia");
            Kategorie.Kategorien.Add("SandWasser", "Sand & Wasser");
            Kategorie.Kategorien.Add("Puppen", "Puppen & Zubehör");
            Kategorie.Kategorien.Add("FahrzeugeFlugzeuge", "Fahrzeuge & Flugzeuge");
            Kategorie.Kategorien.Add("TiereStofftiere", "Tiere & Stofftiere");
            Kategorie.Kategorien.Add("BewegungSport", "Bewegung & Sport");
            Kategorie.Kategorien.Add("KreativBasteln", "Kreativ & Basteln");
            Kategorie.Kategorien.Add("BauenKonstruieren", "Bauen & Konstruieren");
            Kategorie.Kategorien.Add("SchienenBahn", "Schienen & Bahnen");
            Kategorie.Kategorien.Add("Plastik", "Plastikspielzeug");
            Kategorie.Kategorien.Add("Holz", "Holzspielzeug");
            Kategorie.Kategorien.Add("HoerspielMusik", "Hörspiele & Musik");
            Kategorie.Kategorien.Add("BrettspielPuzzle", "Brettspiele & Puzzle");
            Kategorie.Kategorien.Add("RollenspielVerkleidung", "Rollenspiel & Verkleidung");
            Kategorie.Kategorien.Add("KuecheKaufladen", "Küche & Kaufladen");
            Kategorie.Kategorien.Add("LernenFoerdern", "Lernen & Fördern");
            Kategorie.Kategorien.Add("SpielesetFiguren", "Spielesets & Figuren");
            Kategorie.Kategorien.Add("EntspannungGeschicklichkeit", "Entspannung & Geschicklichkeit");
            Kategorie.Kategorien.Add("Buecher", "Bücher");
            Kategorie.Kategorien.Add("Instrumente", "Instrumente");
            Kategorie.Kategorien.Add("Werkstatt", "Werkstatt & Zubehör");
            Kategorie.Kategorien.Add("Beauty", "Beauty & Accessoires");
            Kategorie.Kategorien.Add("Krippenalter", "Krippenalter");
            Kategorie.Kategorien.Add("Kindergartenalter", "Kindergartenalter");
            Kategorie.Kategorien.Add("Schulalter", "Schulalter");
            Kategorie.Kategorien.Add("Dekoration", "Dekoration");
            Kategorie.Kategorien.Add("DVD", "DVD");
        }
    }
}
