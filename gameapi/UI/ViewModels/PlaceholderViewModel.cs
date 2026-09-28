using System;
using System.Windows.Input;
using ReactiveUI;

namespace UI.ViewModels;

public class PlaceholderViewModel : ViewModelBase
{
    public string Title { get; }
    public ICommand BackCommand { get; }

    public PlaceholderViewModel(string title, Action<ViewModelBase> navigate)
    {
        Title = title;
        BackCommand = ReactiveCommand.Create(() => navigate(new SelectorViewModel(navigate)));
    }
}