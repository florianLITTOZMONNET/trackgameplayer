using System;
using System.Linq;
using System.Windows.Input;
using Infrastructure.Database.Steam;
using ReactiveUI;

namespace UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly SteamDatabaseManager _db;
    private string _inputText = "";
    private string _resultText = "";

    public string InputText { get => _inputText; set => this.RaiseAndSetIfChanged(ref _inputText, value); }
    public string ResultText { get => _resultText; set => this.RaiseAndSetIfChanged(ref _resultText, value); }

    public ICommand MyCommand { get; }

    public MainWindowViewModel(SteamDatabaseManager db)
    {
        _db = db;
        MyCommand = ReactiveCommand.Create(() =>
        {
            var gameIds = _db.GetLibrary(InputText);
            ResultText = gameIds is { Count: > 0 }
                ? string.Join(Environment.NewLine, gameIds.Select(id => _db.gameName(id) ?? id))
                : "No games found for that Steam ID.";
        });
    }
}