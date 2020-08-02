﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace NutzMich.Shared.Models
{
    public class NutzMichCommand
    {
        public Symbol Symbol { get; set; }
        public ICommand Command { get; set; }

        public static ICommand GoBackCommand;
    }
}
