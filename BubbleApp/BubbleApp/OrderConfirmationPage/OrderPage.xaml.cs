using BubbleAppNew.BubbleLib;
using Microsoft.Maui;

namespace BubbleAppNew.OrderConfirmationPage;

public partial class OrderPage : ContentPage, IQueryAttributable
{
    private readonly IQrCodeService _qrCodeService;
    public OrderPage(IQrCodeService qrCodeService)
    {
        InitializeComponent();
        _qrCodeService = qrCodeService;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(OrderConfirmationViewModel), out var vm) && vm is OrderConfirmationViewModel viewModel)
        {
            BindingContext = viewModel;

            await viewModel.GenerateQrCodeAsync(_qrCodeService);
        }
        else
        {
            throw new ArgumentException("Invalid query parameter");
        }
    }
}