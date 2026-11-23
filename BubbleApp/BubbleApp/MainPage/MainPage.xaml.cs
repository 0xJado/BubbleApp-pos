using BubbleAppNew.CreatedMomentPage;
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

    private async void OnCreateMomentClicked(object sender, EventArgs e)
    {
        var popup = new CreateMomentPopup();

        popup.MomentCreated += customOrder =>
        {
            _vm.AddWaffleToOrder(customOrder);
        };

        await this.ShowPopupAsync(popup);
    }
}