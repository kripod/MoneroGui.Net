using System.Drawing;
using System.Windows;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class MessageBoxEx
    {
        public byte ButtonResult { get; private set; }

        private MessageBoxEx()
        {
            Icon = StaticObjects.ApplicationIconImage;
            SourceInitialized += delegate {
                this.SetWindowButtonClose(false);
            };

            InitializeComponent();
        }

        public MessageBoxEx(Window owner, string title, string message, Icon icon, string button1Text) : this()
        {
            Initialize(owner, title, message, icon, button1Text);

            Button2.Visibility = Visibility.Collapsed;
            Button3.Visibility = Visibility.Collapsed;

            Button1.Click += Button1_Click;
        }

        public MessageBoxEx(Window owner, string title, string message, Icon icon, string button1Text, string button2Text) : this()
        {
            Initialize(owner, title, message, icon, button1Text);

            ColumnDefinitionButton2.SharedSizeGroup = "A";
            ColumnDefinitionButton2.MinWidth = ColumnDefinitionButton1.MinWidth;

            Button2.Content = button2Text;
            Button3.Visibility = Visibility.Collapsed;

            Button1.Click += Button1_Click;
            Button2.Click += Button2_Click;
        }

        public MessageBoxEx(Window owner, string title, string message, Icon icon, string button1Text, string button2Text, string button3Text) : this()
        {
            Initialize(owner, title, message, icon, button1Text);

            ColumnDefinitionButton2.SharedSizeGroup = "A";
            ColumnDefinitionButton2.MinWidth = ColumnDefinitionButton1.MinWidth;
            ColumnDefinitionButton3.MinWidth = ColumnDefinitionButton1.MinWidth;

            Button2.Content = button2Text;
            Button3.Content = button3Text;

            Button1.Click += Button1_Click;
            Button2.Click += Button2_Click;
            Button3.Click += Button3_Click;
        }

        private void Initialize(Window owner, string title, string message, Icon icon, string button1Text)
        {
            Owner = owner;
            Title = title;

            TextBlockMessage.Text = message;
            Image.Source = icon.ToImageSource();
            Button1.Content = button1Text;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            ButtonResult = 1;
            Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            ButtonResult = 2;
            Close();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            ButtonResult = 3;
            Close();
        }

        public static void Show(Window owner, string title, string message, Icon icon, string button1Text)
        {
            var messageBoxEx = new MessageBoxEx(owner, title, message, icon, button1Text);
            messageBoxEx.ShowDialog();
        }

        public static byte Show(Window owner, string title, string message, Icon icon, string button1Text, string button2Text)
        {
            var messageBoxEx = new MessageBoxEx(owner, title, message, icon, button1Text, button2Text);
            messageBoxEx.ShowDialog();
            return messageBoxEx.ButtonResult;
        }

        public static byte Show(Window owner, string title, string message, Icon icon, string button1Text, string button2Text, string button3Text)
        {
            var messageBoxEx = new MessageBoxEx(owner, title, message, icon, button1Text, button2Text, button3Text);
            messageBoxEx.ShowDialog();
            return messageBoxEx.ButtonResult;
        }
    }
}
