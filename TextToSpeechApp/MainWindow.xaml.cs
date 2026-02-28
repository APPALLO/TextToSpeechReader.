using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextToSpeechApp.ViewModels;

namespace TextToSpeechApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int _lastHighlightStart = -1;
    private int _lastHighlightLength = -1;

    public MainWindow()
    {
        InitializeComponent();
        
        // ViewModel'den gelen eventleri dinle
        if (DataContext is MainViewModel vm)
        {
            vm.HighlightRequested += OnHighlightRequested;
            vm.TextLoaded += OnTextLoaded;
        }
        
        // DataContext değişirse (nadiren olur ama iyi bir pratiktir)
        DataContextChanged += (s, e) =>
        {
            if (e.NewValue is MainViewModel newVm)
            {
                newVm.HighlightRequested += OnHighlightRequested;
                newVm.TextLoaded += OnTextLoaded;
            }
        };

        if (MainTextBox != null)
        {
            MainTextBox.SelectionChanged += OnSelectionChanged;
        }
    }

    private void OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && !vm.IsPlaying)
        {
            vm.CaretPosition = MainTextBox.SelectionStart;
        }
    }

    private void OnTextLoaded(object? sender, string text)
    {
        if (Dispatcher.CheckAccess())
        {
            if (MainTextBox != null)
            {
                MainTextBox.Text = text;
                _lastHighlightStart = -1;
                _lastHighlightLength = -1;
            }
            return;
        }

        _ = Dispatcher.InvokeAsync(() =>
        {
            if (MainTextBox != null)
            {
                MainTextBox.Text = text;
                _lastHighlightStart = -1;
                _lastHighlightLength = -1;
            }
        });
    }

    private void OnHighlightRequested(object? sender, int position)
    {
        if (Dispatcher.CheckAccess())
        {
            Highlight(position);
            return;
        }

        _ = Dispatcher.InvokeAsync(() => Highlight(position), System.Windows.Threading.DispatcherPriority.Background);
    }

    private void Highlight(int position)
    {
        if (MainTextBox == null) return;
        if (position < 0) return;

        string text = MainTextBox.Text;
        if (position >= text.Length) return;

        int start = position;
        int length = 1;

        // Metin sınırlarını kontrol et
        if (start < 0 || start >= text.Length) return;

        // Geriye doğru kelime başlangıcını bul
        while (start > 0 && !char.IsWhiteSpace(text[start - 1]) && !char.IsPunctuation(text[start - 1]))
        {
            start--;
        }

        // İleriye doğru kelime sonunu bul
        while (start + length < text.Length && !char.IsWhiteSpace(text[start + length]) && !char.IsPunctuation(text[start + length]))
        {
            length++;
        }

        if (start == _lastHighlightStart && length == _lastHighlightLength) return;

        _lastHighlightStart = start;
        _lastHighlightLength = length;

        if (!MainTextBox.IsKeyboardFocusWithin)
        {
            MainTextBox.Focus();
        }

        MainTextBox.Select(start, length);
    }
}
