using System;
using System.Windows;
using System.Windows.Threading;

namespace Jojatekok.MoneroGUI
{
    class Logger : DependencyObject
    {
        private const int MaxLineCount = 300;

        public static readonly DependencyProperty MessagesProperty = DependencyProperty.RegisterAttached(
            "Messages",
            typeof(string),
            typeof(Logger),
            new PropertyMetadata(string.Empty)
        );

        public string Messages {
            get { return Dispatcher.Invoke(() => GetValue(MessagesProperty) as string, DispatcherPriority.DataBind); }
            private set { Dispatcher.Invoke(() => SetValue(MessagesProperty, value), DispatcherPriority.DataBind); }
        }

        private int LineCount { get; set; }
        public bool IsMaxLineCountReached { get; private set; }

        public void Log(string message)
        {
            var time = DateTime.Now.ToString("[HH:mm:ss] ", Helper.InvariantCulture);
            var allMessages = Messages;

            if (LineCount == MaxLineCount) {
                IsMaxLineCountReached = true;
                allMessages = allMessages.Substring(allMessages.IndexOf(Helper.NewLineString, StringComparison.Ordinal) + Helper.NewLineString.Length);
            } else {
                LineCount += 1;
            }

            message = time + message;
            if (Messages.Length != 0) message = Helper.NewLineString + message;

            allMessages += message;
            Messages = allMessages;
        }

        public void Clear()
        {
            Messages = string.Empty;
            LineCount = 0;
            IsMaxLineCountReached = false;
        }
    }
}
