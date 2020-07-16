using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.WebUI;

namespace NutzMich.Shared.Models
{
    public class Profil
    {
        public string AnbieterID { get; set; }
        public string Nickname { get; set; }
        public bool Nichtraucher { get; set; }
        public bool TierfreierHaushalt { get; set; }
        public string UeberMich { get; set; }
        public string ProfilbildBase64 { get; set; }

        public static Profil GetFallback(string anbieterID)
        {
            return new Profil() { AnbieterID = anbieterID, Nickname = anbieterID };
        }
    }
}
