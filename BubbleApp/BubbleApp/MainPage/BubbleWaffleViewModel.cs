using System.Collections.ObjectModel;
using BubbleAppNew.BubbleLib;
using BubbleAppNew.OrderConfirmationPage;
using BubbleLib;
using BubbleLib.Models;
using BubbleLib.Services;

namespace BubbleAppNew.MainPage;

public partial class BubbleWaffleViewModel : ObservableObject
{
    private readonly IWaffleService _waffleService;
    private readonly ICoinGeckoService _coinGeckoService;
    private readonly IQrCodeService _qrCodeService;

    public ObservableCollection<BubbleWaffle> WaffleMenu => _waffleService.WaffleMenu;
    public ObservableCollection<BubbleWaffle> OrderedItems => _waffleService.OrderedItems;

    [ObservableProperty] private decimal totalPrice;
    [ObservableProperty] private decimal solanaPrice;

    public BubbleWaffleViewModel(IWaffleService waffleService, ICoinGeckoService coinGeckoService, IQrCodeService qrCodeService)
    {
        _waffleService = waffleService ?? throw new ArgumentNullException(nameof(waffleService), "Der WaffleService darf nicht null sein.");
        _coinGeckoService = coinGeckoService ?? throw new ArgumentNullException(nameof(coinGeckoService));
        _qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
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
        Console.WriteLine("---> QR Code Generation Clicked");
        //Get the latest Solana rate from CoinGecko
        var solanaRate = await _coinGeckoService.GetSolanaEuroExchangeRateAsync();
        Console.WriteLine($"---> Solana Rate: {solanaRate}");

        var orderedItems = new ObservableCollection<BubbleWaffle>(_waffleService.OrderedItems);
        Console.WriteLine($"---> Items in Order: {orderedItems.Count}");

        var euroPrice = _waffleService.GetTotalPrice();
        Console.WriteLine($"---> Euro Price: {euroPrice}");

        var storeWalletAddress = "0x1234567890abcdef1234567890abcdef12345678"; // Replace with actual wallet address
       
        var ocvm = new OrderConfirmationViewModel(
            new ObservableCollection<BubbleWaffle>(orderedItems),
            euroPrice,
            solanaRate,
            storeWalletAddress);
        

        await ocvm.GenerateQrCodeAsync(_qrCodeService);     
        Console.WriteLine("---> QR Code Generated");

        await Shell.Current.GoToAsync(nameof(OrderPage), true, new Dictionary<string, object>
        {
            { "OrderConfirmationViewModel", ocvm }
        });
        
        Console.WriteLine("---> OrderPage Navigation Completed");
    }
}