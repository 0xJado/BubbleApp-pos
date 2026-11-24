using BubbleLib;
using CommunityToolkit.Maui.Views;

namespace BubbleAppNew.CreatedMomentPage;

public partial class CreateMomentPopup : Popup
{
    public event Action<BubbleWaffle>? MomentCreated;

    public CreateMomentPopup()
    {
        InitializeComponent();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        var priceText = PriceEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name) || !decimal.TryParse(priceText, out var price))
        {
            return;
        }

        var customItem = new BubbleWaffle
        {
            WaffleName = name,
            Quantity = 1,
            CustomPrice = price
        };

        MomentCreated?.Invoke(customItem);
        await CloseAsync();
    }
}