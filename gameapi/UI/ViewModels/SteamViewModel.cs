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
        long y = x / 525600;   x %= 525600;
        long m = x / 43800;    x %= 43800;
        long d = x / 1440;     x %= 1440;
        long h = x / 60;       x %= 60;
        long min = x;
        return $"{y}Y:{m:00}M:{d:00}D:{h:00}h:{min:00}min";
    }

    public SteamViewModel(Action<ViewModelBase> navigate)
    {
        _db = App.Services.GetRequiredService<SteamDatabaseManager>();

        SearchCommand = ReactiveCommand.Create(() =>
        {
            string player_id = InputText.Trim();
            var gameIds = _db.GetLibrary(player_id);
            Games = gameIds is { Count: > 0 }
                ? gameIds.Select(id => new GameResult
                {
                    Id = id,
                    Name = _db.GameName(id) ?? id,
                    BannerUrl = $"https://cdn.akamai.steamstatic.com/steam/apps/{id}/header.jpg",
                    hours = Format(_db.GetGameHours(player_id,id))
                }).ToList()
                : [];

            ResultText = Games.Count > 0 ? "" : "No games found for that Steam ID.";
        });

        BackCommand = ReactiveCommand.Create(() => navigate(new SelectorViewModel(navigate)));
    }
    
}