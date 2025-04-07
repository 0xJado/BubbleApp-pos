using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using BubbleAppNew.BubbleLib;
using BubbleLib.Models;

namespace BubbleAppNew.OrderConfirmationPage;

public class OrderConfirmationViewModel : INotifyPropertyChanged
{
    public ObservableCollection<BubbleWaffle> OrderedItems { get; set; }

    private string _storeWalletAddress;
    public string StoreWalletAddress
    {
        get => _storeWalletAddress;
        set
        {
            _storeWalletAddress = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShortWalletAddress));
        }
    }

    private decimal _euroPrice;
    public decimal EuroPrice
    {
        get => _euroPrice;
        set
        {
            _euroPrice = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalSol));
        }
    }

    private decimal _solanaRate;
    public decimal SolanaRate
    {
        get => _solanaRate;
        set
        {
            _solanaRate = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalSol));
        }
    }

    public decimal TotalSol => Math.Round(EuroPrice / SolanaRate, 3);

    public string ShortWalletAddress =>
        string.IsNullOrWhiteSpace(StoreWalletAddress)
            ? string.Empty
            : $"{StoreWalletAddress[..6]}...{StoreWalletAddress[^4..]}";

    private ImageSource _qrCodeImage;
    public ImageSource QrCodeImage
    {
        get => _qrCodeImage;
        set
        {
            _qrCodeImage = value;
            OnPropertyChanged();
        }
    }

    public string OrderId { get; }

    public OrderConfirmationViewModel(
        ObservableCollection<BubbleWaffle> orderedItems,
        decimal euroPrice,
        decimal solanaRate,
        string storeWalletAddress)
    {
        OrderedItems = orderedItems;
        EuroPrice = euroPrice;
        SolanaRate = solanaRate;
        StoreWalletAddress = storeWalletAddress;
        OrderId = Guid.NewGuid().ToString().Split('-')[0];
    }

    public async Task GenerateQrCodeAsync(IQrCodeService qrCodeService)
    {
        if (string.IsNullOrWhiteSpace(StoreWalletAddress) || TotalSol <= 0)
            return;
        try
        {
            var solUri = $"solana:{StoreWalletAddress}?amount={TotalSol}";
            QrCodeImage = await Task.Run(() => qrCodeService.GenerateQrCode(solUri));
            Console.WriteLine("Generated QR Code successfully generated");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating QR Code: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
