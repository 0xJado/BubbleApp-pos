using BubbleAppNew.MainPage;
using BubbleLib;
using BubbleLib.Services;

namespace BubbleAppNew;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}