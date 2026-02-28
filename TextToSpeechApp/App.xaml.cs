using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace TextToSpeechApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"Beklenmeyen bir hata oluştu: {e.Exception.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true; // Uygulamanın çökmesini engelle
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            MessageBox.Show($"Kritik hata: {ex.Message}", "Kritik Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

