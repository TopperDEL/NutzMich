using NutzMich.Contracts.Interfaces;
using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using uplink.NET.Models;
using Windows.UI.Xaml.Data;

namespace NutzMich.Shared.ViewModels
{
    [Bindable]
    public class AnfrageViewModel
    {
        public Angebot Angebot{ get; set; }
        public Anfrage Anfrage { get; set; }

        public AnfrageViewModel(Angebot angebot, ILoginService loginService)
        {
            Angebot = angebot;
            Anfrage = new Anfrage(Angebot);
            Anfrage.AnfragerId = loginService.AnbieterID;
            Anfrage.AbholungsZeitpunkt = DateTime.Now;
            Anfrage.LeiheVon = DateTime.Now.AddDays(1);
            Anfrage.LeiheBis = DateTime.Now.AddDays(8);
        }
    }
}
