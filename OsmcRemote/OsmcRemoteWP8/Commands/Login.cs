using OsmcRemoteWP8.Helpers;
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
            return true;
        }

        public void Execute(object parameter)
        {
            DialogsAndCommandsHelper.RunLogin();
        }
    }
}
