using Client.Exceptions;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;


namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
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

    public MainWindow()
    {
        InitializeComponent();

        try
        {
            client = new("localhost", 4040);
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
    /// On Closing the application close all the resources
    /// </summary>
    /// <param name="e"></param>
    protected override void OnClosing(CancelEventArgs e)
    {
        writer?.WriteLine("~~~");

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
        if(serverResponseCode == 500)
        {
            MessageBox.Show("Internal Server Exception");
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
        if(string.IsNullOrWhiteSpace(Word_TextBox.Text) || string.IsNullOrEmpty(Word_TextBox.Text))
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
            Console.WriteLine(ex.Message);
        }
        catch (IOException ex) 
        {
            Console.WriteLine(ex.Message);
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
}