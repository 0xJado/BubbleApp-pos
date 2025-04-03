using System.Collections.ObjectModel;
using BubbleAppNew.OrderConfirmationPage;
using BubbleLib;
using BubbleLib.Models;
using BubbleLib.Services;

namespace BubbleAppNew.MainPage;

public partial class BubbleWaffleViewModel : ObservableObject
{
    private readonly IWaffleService _waffleService;
    private readonly ICoinGeckoService _coinGeckoService;

    public ObservableCollection<BubbleWaffle> WaffleMenu => _waffleService.WaffleMenu;
    public ObservableCollection<BubbleWaffle> OrderedItems => _waffleService.OrderedItems;

    [ObservableProperty] private decimal totalPrice;
    [ObservableProperty] private decimal solanaPrice;

    public BubbleWaffleViewModel(IWaffleService waffleService, ICoinGeckoService coinGeckoService)
    {
        _waffleService = waffleService ?? throw new ArgumentNullException(nameof(waffleService), "Der WaffleService darf nicht null sein.");
        _coinGeckoService = coinGeckoService ?? throw new ArgumentNullException(nameof(coinGeckoService));
        UpdatePricesAsync();
    }

    [RelayCommand]
    public void AddWaffleToOrder(BubbleWaffle waffle)
    {
        _waffleService.AddWaffleToOrder(waffle);
        UpdatePricesAsync();
    }

    [RelayCommand]
    private void RemoveWaffleFromOrder(BubbleWaffle? waffle)
    {
        if (waffle == null) return;
        _waffleService.RemoveWaffleFromOrder(waffle);
        UpdatePricesAsync();
    }

    private async Task UpdatePricesAsync()
    {
        TotalPrice = _waffleService.GetTotalPrice();
        var solanaRate = await _coinGeckoService.GetSolanaEuroExchangeRateAsync();
        SolanaPrice = TotalPrice / solanaRate;
    }
    
    [RelayCommand]
    private async Task GenerateQrCodeAsync()
    {
        //Get the latest Solana rate from CoinGecko
        var solanaRate = await _coinGeckoService.GetSolanaEuroExchangeRateAsync();
        
        var orderedItems = _waffleService.OrderedItems.ToList();
        var euroPrice = _waffleService.GetTotalPrice();
        var storeWalletAddress = "0x1234567890abcdef1234567890abcdef12345678"; // Replace with actual wallet address
       
        var ocvm = new OrderConfirmationViewModel(
            orderedItems,
            euroPrice,
            solanaRate,
            storeWalletAddress);
        
        var confirmationPage = new OrderConfirmationPage
        {
            BindingContext = ocvm
        };
        
        await Shell.Current.GoToAsync(nameof(OrderConfirmationPage), true, new Dictionary<string, object>
        {
            { "BindingContext", ocvm }
        });
    }
}