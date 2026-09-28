using System;
using System.Linq;
using System.Windows.Input;
using Infrastructure.Database.Steam;
using ReactiveUI;
using Microsoft.Extensions.DependencyInjection;

namespace UI.ViewModels;

public class SteamViewModel : ViewModelBase
{
    private readonly SteamDatabaseManager _db;
    private string _inputText = "";
    private string _resultText = "";

    public string InputText  { get => _inputText;  set => this.RaiseAndSetIfChanged(ref _inputText,  value); }
    public string ResultText { get => _resultText; set => this.RaiseAndSetIfChanged(ref _resultText, value); }

    public ICommand SearchCommand { get; }
    public ICommand BackCommand   { get; }

    public SteamViewModel(Action<ViewModelBase> navigate)
    {
        _db = App.Services.GetRequiredService<SteamDatabaseManager>();

        SearchCommand = ReactiveCommand.Create(() =>
        {
            var gameIds = _db.GetLibrary(InputText);
            ResultText = gameIds is { Count: > 0 }
                ? string.Join(Environment.NewLine, gameIds.Select(id => _db.gameName(id) ?? id))
                : "No games found for that Steam ID.";
        });

        BackCommand = ReactiveCommand.Create(() => navigate(new SelectorViewModel(navigate)));
    }
    
}