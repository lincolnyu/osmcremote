using System;
using System.Windows.Input;
using OsmcRemoteUwp.Helpers;

namespace OsmcRemoteUwp.Commands
{
    public class Settings : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            await DialogsAndCommandsHelper.ShowAuthenticationDialog();
        }
    }
}
