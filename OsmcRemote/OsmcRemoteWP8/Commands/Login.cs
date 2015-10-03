﻿using OsmcRemoteWP8.Helpers;
using System;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace OsmcRemoteWP8.Commands
{
    public class Login : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return ((App)Application.Current).Settings.CredentialsLoaded;
        }

        public async void Execute(object parameter)
        {
            await DialogsAndCommandsHelper.RunLogin();
        }
    }
}
