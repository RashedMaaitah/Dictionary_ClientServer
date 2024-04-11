using Client.Custom;
using Client.Exceptions;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    /// <summary>
    /// Constant to end the connection with the server
    /// </summary>
    const string EXIT = "tFPnUEsZD37Ad";

    /// <summary>
    /// TCP client Socket
    /// </summary>
    readonly TcpClient? client;

    /// <summary>
    /// Network stream for communication
    /// </summary>
    readonly NetworkStream? stream;

    /// <summary>
    /// Stream reader to read from <see cref="stream"/>
    /// </summary>
    readonly StreamReader? reader;

    /// <summary>
    /// Stream writer to write to <see cref="stream"/>
    /// </summary>
    readonly StreamWriter? writer;

    private int PORT_NUMBER;
    public MainWindow()
    {
        InitializeComponent();

        PromptPortNumber("Enter the port number to connect");

        try
        {
            client = new("localhost", PORT_NUMBER);
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Close();
        }
    }

    /// <summary>
    /// Choose the port number to connect to
    /// </summary>
    /// <param name="msg"></param>
    private void PromptPortNumber(string msg)
    {

        PromptWindow prompt = new(msg, "4040");
        while (true)
        {
            prompt.ShowDialog();
            if (prompt.IsClosed)
            {
                Close();
                return;
            }
            var successfullParse = int.TryParse(prompt.Answer, out PORT_NUMBER);
            if (!successfullParse)
            {
                prompt = new("Enter a valid number value");
                continue;
            }
            if (!ValidPortNumber(PORT_NUMBER))
            {
                prompt = new("Port number must be >= 1024 or <= 49151");
                continue;
            }
            if (successfullParse && ValidPortNumber(PORT_NUMBER))
            {
                break;
            }
        }
    }

    /// <summary>
    /// On Closing the application close all the resources
    /// </summary>
    /// <param name="e"></param>
    protected override void OnClosing(CancelEventArgs e)
    {
        writer?.WriteLine(EXIT);

        writer?.Close();
        reader?.Close();
        client?.Close();
    }

    /// <summary>
    /// Requests the server socket for the meaning of a <paramref name="word"/>
    /// </summary>
    /// <param name="word"></param>
    /// <returns> Meaning of <paramref name="word"/> </returns>
    private string Read(string word)
    {
        writer?.WriteLine(word);
        writer?.Flush();

        int serverResponseCode = int.Parse(reader?.ReadLine() ?? "500");
        string serverResponse = reader?.ReadLine() ?? "";

        if (serverResponseCode == 404)
        {
            MessageBox.Show("Not found");
            throw new ResourceNotFoundException();
        }
        if (serverResponseCode == 500)
        {
            throw new IOException();
        }

        return serverResponse.Replace(":", ":\n").Replace(".~", ".\n").Replace("~", "");
    }

    /// <summary>
    /// Handle Submit button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Submit_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Word_TextBox.Text) || string.IsNullOrEmpty(Word_TextBox.Text))
        {
            Required_Label.Visibility = Visibility.Visible;
            return;
        }
        try
        {
            Result_TextBox.Text = Read(Word_TextBox.Text);
            WordsCount_TextBlock.Text = $"Words: {Result_TextBox.Text.Split(' ').Length}";
        }
        catch (ResourceNotFoundException ex)
        {
            // Output the error to the debug if debugging mode is active
            Debug.WriteLine(ex.Message);
        }
        catch (IOException ex)
        {
            // Output the error to the debug if debugging mode is active
            Debug.WriteLine(ex.Message);

            MessageBox.Show("Internal Server Exception");
            PromptPortNumber("Change the port number?");
        }
        finally
        {
            Word_TextBox?.Focus();
            Required_Label.Visibility = Visibility.Hidden;
        }
    }

    /// <summary>
    /// Handle the Clear button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        Word_TextBox?.Clear();
        Result_TextBox?.Clear();
        WordsCount_TextBlock.Text = string.Empty;
        Word_TextBox?.Focus();

        Required_Label.Visibility = Visibility.Hidden;
    }

    /// <summary>
    /// Close the MainWindow
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Exit_MenuItem_Click(object sender, RoutedEventArgs e) => Close();

    /// <summary>
    /// Drag and moves the window on Mouse left button down
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MetroWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    /// <summary>
    /// Validates the port number 
    /// </summary>
    /// <param name="port"></param>
    /// <returns> true if the port number is valid </returns>
    private static bool ValidPortNumber(int port)
    {
        if (port is < 1024 or > 49151)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Change theme based on <paramref name="option"/>
    /// </summary>
    /// <param name="option"> the mene option chosen number</param>
    private void ChangeTheme(int option = 0)
    {
        if(Main_Grid is null || Word_TextBox is null || Result_TextBox is null ||
            App_Label is null || Def_Label is null || Word_Label is null ||
            SubmitBtn_Border is null || Submit is null || Clear is null)
        {
            return;
        }

        // <--- Grid Style --->
        var gridBgColor = new string[] { "#f4f7f5", "#bde0fe" };

        Main_Grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(gridBgColor[option]));


        // <--- Labels Style --->
        var labelForgroundOptions = new string[] { "#575a5e", "#457b9d" };
        var labelDropShadowColor = new string[] { "#EEEFEE", "#bde0fe" };

        var dropDownShadow = new DropShadowEffect
        {
            Color = (Color)ColorConverter.ConvertFromString(labelDropShadowColor[option]),
            Opacity = 0.6,
            ShadowDepth = 6,
            BlurRadius = 2,
            Direction = -90
        };
        App_Label.Foreground = Word_Label.Foreground = Def_Label.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(labelForgroundOptions[option]));
        App_Label.Effect = Word_Label.Effect = Def_Label.Effect = dropDownShadow;


        // <--- TextBoxes Style --->
        var tbBorderBrush = new string[] { "#BDE0FE" };
        var tbBg = new string[] { "#A7A2A9", "#A2D2FF" };

        Word_TextBox.BorderBrush = Result_TextBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(tbBorderBrush[0]));
        Word_TextBox.Background = Result_TextBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(tbBg[option]));


        // <--- Buttons style --->
        var btnsBgs = new string[] { "#F7F7F6", "#F7F7F6" };
        var btnBorderBrushs = new string[] { "#575a5e", "#457b9d" };
        var btnFgs = new string[] { "#575a5e", "#457b9d" };

        SubmitBtn_Border.BorderBrush = ClearBtn_Border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(btnBorderBrushs[option]));
        Submit.Foreground = Clear.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(btnFgs[option]));
        Submit.Background = Clear.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(btnsBgs[option]));

    }

    private void Theme_MenuItem_Clicked(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menu)
        {
            return;
        }

        switch (menu.Header)
        {
            case "Default":
                ChangeTheme();
                break;
            case "Sky":
                ChangeTheme(1);
                break;
            default:
                break;
        }
    }


}