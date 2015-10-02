using System;
using Windows.Storage;

namespace OsmcRemoteWP8.Data
{
    public class SettingsData
    {
        private string _serverAddress;
        private string _userName;
        private string _password;

        public string ServerAddress
        {
            get
            {
                return _serverAddress;
            }
            set
            {
                if (_serverAddress != value)
                {
                    _serverAddress = value;
                    SaveServerAddress();
                }
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    SaveUserName();
                }
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                if (_password != value)
                {
                    _password = value;
                    SavePassword();
                }
            }
        }

        public bool CredentialsLoaded
        {
            get; private set;
        }
        
        public void LoadCredentials()
        {
            CredentialsLoaded = false;
            _serverAddress = "";
            _userName = "";
            _password = "";
            var ls = ApplicationData.Current.LocalSettings;
            object sa;
            if (!ls.Values.TryGetValue("ServerAddress", out sa))
            {
                return;
            }
            _serverAddress = (string)sa;
            object un;
            if (!ls.Values.TryGetValue("UserName", out un))
            {
                return;
            }
            _userName = (string)un;
            object pw;
            if (!ls.Values.TryGetValue("Password", out pw))
            {
                return;
            }
            _password = (string)pw;
            CredentialsLoaded = true;
        }

        void SaveServerAddress()
        {
            var ls = ApplicationData.Current.LocalSettings;
            ls.Values["ServerAddress"] = _serverAddress;
        }


        private void SaveUserName()
        {
            var ls = ApplicationData.Current.LocalSettings;
            ls.Values["UserName"] = _userName;
        }


        void SavePassword()
        {
            var ls = ApplicationData.Current.LocalSettings;
            ls.Values["Password"] = _password;
        }
    }
}
