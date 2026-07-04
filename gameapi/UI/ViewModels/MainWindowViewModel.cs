using System.Windows.Input;
using ReactiveUI;

namespace UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _inputText = "";
    private string _resultText = "";
    private string _Gamelist = "";

    public string InputText
    {
        get => _inputText;
        set => this.RaiseAndSetIfChanged(ref _inputText, value);
    }

    public string ResultText
    {
        get => _resultText;
        set => this.RaiseAndSetIfChanged(ref _resultText, value);
    }

    public string Gamelist
    {
        get => _Gamelist;
        set=> this.RaiseAndSetIfChanged(ref _Gamelist, value);
    }

    public ICommand MyCommand { get; }

    public MainWindowViewModel()
    {
        MyCommand = ReactiveCommand.Create(() =>
        {
            //ResultText = $"You typed: {InputText}";
            
        });
    }
}