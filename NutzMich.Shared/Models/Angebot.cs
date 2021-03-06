﻿using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace NutzMich.Contracts.Models
{
    [Bindable]
    public class Angebot
    {
        public string Id { get; set; }
        public string AnbieterId { get; set; }
        public string Ueberschrift { get; set; }
        public string Beschreibung { get; set; }
        public string Zustand { get; set; }
        public string AnfrageAccess { get; set; }
        public string NachrichtenAccess { get; set; }
        public string ThumbnailBase64 { get; set; }
        public int Version { get; set; }
        public List<string> Kategorien { get; set; }
        public DateTime EingestelltAm { get; set; }

        public Angebot()
        {
            Id = Guid.NewGuid().ToString();
            Version = 1;
            Kategorien = new List<string>();
            EingestelltAm = DateTime.MinValue;
        }
    }
}
