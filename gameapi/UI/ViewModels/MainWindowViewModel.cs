using ReactiveUI;

namespace UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentPage;
    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    public MainWindowViewModel()
    {
        void Navigate(ViewModelBase vm) => CurrentPage = vm;
        _currentPage = new SelectorViewModel(Navigate);
    }
}