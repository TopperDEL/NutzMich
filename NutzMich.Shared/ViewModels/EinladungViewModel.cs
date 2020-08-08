using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace NutzMich.Shared.ViewModels
{
    public class EinladungViewModel: INotifyPropertyChanged
    {
        public string Token { get; private set; }
        public bool TokenErzeugt { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ErzeugeToken { get; set; }

        public void SetzeToken(string token)
        {
            Token = token;
            TokenErzeugt = true;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Token)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TokenErzeugt)));
        }
    }
}
