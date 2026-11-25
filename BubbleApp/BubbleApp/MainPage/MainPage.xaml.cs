using BubbleAppNew.CreatedMomentPage;
using BubbleLib;
using CommunityToolkit.Maui.Extensions;

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

    private async void OnCreateMomentClicked(object? sender, EventArgs e)
    {
        var popup = new CreateMomentPopup();

        BubbleWaffle? customOrder = null;
        popup.MomentCreated += waffle => customOrder = waffle;

        await this.ShowPopupAsync(popup);
        if (customOrder is not null)
        {
            await _vm.AddWaffleToOrderAsync(customOrder);
        }
    }
}