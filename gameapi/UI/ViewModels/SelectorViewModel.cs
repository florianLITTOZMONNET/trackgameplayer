using System;
using System.Windows.Input;
using ReactiveUI;

namespace UI.ViewModels;

public class SelectorViewModel : ViewModelBase
{
    public ICommand GoSteamCommand { get; }
    public ICommand GoRiotCommand { get; }
    public ICommand GoSpotifyCommand { get; }

    public SelectorViewModel(Action<ViewModelBase> navigate)
    {
        GoSteamCommand  = ReactiveCommand.Create(() => navigate(new SteamViewModel(navigate)));
        GoRiotCommand   = ReactiveCommand.Create(() => navigate(new PlaceholderViewModel("Riot",    navigate)));
        GoSpotifyCommand= ReactiveCommand.Create(() => navigate(new PlaceholderViewModel("Spotify", navigate)));
    }
}