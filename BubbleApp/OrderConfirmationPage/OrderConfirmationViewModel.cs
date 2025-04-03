using System.Collections.ObjectModel;
using BubbleLib.Models;

namespace BubbleAppNew.OrderConfirmationPage;

public class OrderConfirmationViewModel
{
    public ObservableCollection<BubbleWaffle> OrderedItems { get; set; }
    
    public string OrderId { get; set; }
    public string StoreWalletAddress { get; set; }
    
    public decimal EuroPrice { get; set; }
    public decimal SolanaRate { get; set; }
    public decimal TotalSol => Math.Round(EuroPrice * SolanaRate, 3);
    
    public string ShortWalletAddress =>
        string.IsNullOrWhiteSpace(StoreWalletAddress) ? "" : $"{StoreWalletAddress[..4]}...{StoreWalletAddress[^6..]}";

    public OrderConfirmationViewModel(
        List<BubbleWaffle> orderedItems,
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
}