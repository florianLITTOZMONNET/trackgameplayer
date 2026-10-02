using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Core.Models.Ui;
using Infrastructure.Database.Steam;
using ReactiveUI;
using Microsoft.Extensions.DependencyInjection;

namespace UI.ViewModels;

public class SteamViewModel : ViewModelBase
{
    private readonly SteamDatabaseManager _db;
    private string _inputText = "";
    private string _resultText = "";
    
    private List<GameResult> _games = [];
    public List<GameResult> Games
    {
        get => _games;
        set => this.RaiseAndSetIfChanged(ref _games, value);
    }

    public string InputText  { get => _inputText;  set => this.RaiseAndSetIfChanged(ref _inputText,  value); }
    public string ResultText { get => _resultText; set => this.RaiseAndSetIfChanged(ref _resultText, value); }

    public ICommand SearchCommand { get; }
    public ICommand BackCommand   { get; }
    
    
    static string Format(string s)
    {
        long x = long.Parse(s);
        long y = x / 31536000; x %= 31536000;
        long m = x / 2592000;  x %= 2592000;
        long h = x / 3600;     x %= 3600;
        long min = x / 60;     x %= 60;

        return $"{y}Y:{m:00}M:{h:00}h:{min:00}min:{x:00}s";
    }

    public SteamViewModel(Action<ViewModelBase> navigate)
    {
        _db = App.Services.GetRequiredService<SteamDatabaseManager>();

        SearchCommand = ReactiveCommand.Create(() =>
        {
           
            var gameIds = _db.GetLibrary(InputText.Trim());
            Games = gameIds is { Count: > 0 }
                ? gameIds.Select(id => new GameResult
                {
                    Id = id,
                    Name = _db.GameName(id) ?? id,
                    BannerUrl = $"https://cdn.akamai.steamstatic.com/steam/apps/{id}/header.jpg",
                    hours = Format(_db.GetLibraryHours(id))
                }).ToList()
                : [];

            ResultText = Games.Count > 0 ? "" : "No games found for that Steam ID.";
        });

        BackCommand = ReactiveCommand.Create(() => navigate(new SelectorViewModel(navigate)));
    }
    
}