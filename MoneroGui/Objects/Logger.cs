using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jojatekok.MoneroGUI
{
    class Logger : INotifyPropertyChanged
    {
        private const int MaxLineCount = 300;

        private string _messages = string.Empty;
        public string Messages {
            get { return _messages; }

            private set {
                _messages = value;
                OnPropertyChanged();
            }
        }

        private int LineCount { get; set; }

        internal void Log(string message)
        {
            var time = DateTime.Now.ToString("[HH:mm:ss] ", Helper.InvariantCulture);
            var messages = Messages;

            if (LineCount == MaxLineCount) {
                messages = messages.Substring(messages.IndexOf(Helper.NewLineString, StringComparison.Ordinal) + 2);
            } else {
                LineCount += 1;
            }

            messages += time + message + Helper.NewLineString;
            Messages = messages;
        }

        internal void Clear()
        {
            Messages = string.Empty;
            LineCount = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
