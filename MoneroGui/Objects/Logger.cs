using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI
{
    class Logger : INotifyPropertyChanged
    {
        public event EventHandler<string> OnMessage;

        private const int MaxLineCount = 300;

        private int LineCount { get; set; }
        public bool IsMaxLineCountReached { get; private set; }

        private string _messages = string.Empty;
        public string Messages {
            get { return _messages; }

            private set {
                _messages = value;
                OnPropertyChanged();
            }
        }

        public void Log(string message)
        {
            var time = DateTime.Now.ToString("[HH:mm:ss] ", Helper.InvariantCulture);
            var allMessages = Messages;

            if (LineCount == MaxLineCount) {
                IsMaxLineCountReached = true;
                allMessages = allMessages.Substring(allMessages.IndexOf(Helper.NewLineString, StringComparison.InvariantCulture) + Helper.NewLineString.Length);
            } else {
                LineCount += 1;
            }

            message = time + message;
            if (Messages.Length != 0) message = Helper.NewLineString + message;

            allMessages += message;
            Messages = allMessages;

            if (OnMessage != null) OnMessage(this, message);
        }

        public void Clear()
        {
            Messages = string.Empty;
            LineCount = 0;
            IsMaxLineCountReached = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
