using MahApps.Metro.Controls;
using System.Windows;

namespace Client.Custom
{
    /// <summary>
    /// Interaction logic for PromptWindow.xaml
    /// </summary>
	public partial class PromptWindow : MetroWindow
    {

        public bool IsClosed { get; private set; } = false;

        public PromptWindow(string question, string defaultAnswer = "")
        {
            InitializeComponent();
            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
        }
        
        private void BtnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Answer
        {
            get { return txtAnswer.Text; }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsClosed = true;
        }
    }
}
