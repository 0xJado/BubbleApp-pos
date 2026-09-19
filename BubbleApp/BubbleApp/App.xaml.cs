using BubbleAppNew.MainPage;
using BubbleLib;

namespace BubbleAppNew;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}