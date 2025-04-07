using BubbleAppNew.CreatedMomentPage;
using BubbleLib.Models;
using CommunityToolkit.Maui.Views;

namespace BubbleAppNew.MainPage;

public partial class MainPage
{
    private readonly BubbleWaffleViewModel _vm;

    public MainPage(BubbleWaffleViewModel viewModel)
    {
        InitializeComponent();
        _vm = viewModel;
        BindingContext = _vm;
    }

    private void OnCreateMomentClicked(object sender, EventArgs e)
    {
        var popup = new CreateMomentPopup();

        popup.MomentCreated += customOrder =>
        {
            _vm.AddWaffleToOrder(customOrder);
        };

        this.ShowPopup(popup);
    }
}