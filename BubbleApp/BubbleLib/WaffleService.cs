using System.Collections.ObjectModel;
using System.Linq;

namespace BubbleLib;

public interface IWaffleService
{
    ObservableCollection<BubbleWaffle> WaffleMenu { get; }
    ObservableCollection<BubbleWaffle> OrderedItems { get; }
    
    void AddWaffleToOrder(BubbleWaffle waffle);
    void RemoveWaffleFromOrder(BubbleWaffle waffle);
    decimal GetWafflePrice(); 
    decimal GetTotalPrice();
    void ClearOrder();
}

public class WaffleService : IWaffleService
{
    public ObservableCollection<BubbleWaffle> WaffleMenu { get; } = new()
    {
        new BubbleWaffle { WaffleName = "Cookie Moment" },
        new BubbleWaffle { WaffleName = "Latte Macchiato Moment" },
        new BubbleWaffle { WaffleName = "Caramel Moment" },
        new BubbleWaffle { WaffleName = "Berry Moment" },
        new BubbleWaffle { WaffleName = "Flamingo Moment" },
        new BubbleWaffle { WaffleName = "Banana Split Moment" },
        new BubbleWaffle { WaffleName = "Schlumpf Moment" },
        new BubbleWaffle { WaffleName = "Kids Moment" }
    };

    public void ClearOrder()
    {
        OrderedItems.Clear();
    }
    
    public ObservableCollection<BubbleWaffle> OrderedItems { get; } = new();
    
    public void AddWaffleToOrder(BubbleWaffle waffle)
    {
        var existingWaffle = OrderedItems.FirstOrDefault(w => w.WaffleName == waffle.WaffleName);
        if (existingWaffle != null)
        {
            existingWaffle.Quantity++;
        }
        else
        {
            OrderedItems.Add(new BubbleWaffle 
            { 
                WaffleName = waffle.WaffleName, 
                Quantity = 1 ,
                CustomPrice = waffle.CustomPrice
            });
        }
    }
    public void RemoveWaffleFromOrder(BubbleWaffle waffle)
    {
        var existingWaffle = OrderedItems.FirstOrDefault(w => w.WaffleName == waffle.WaffleName);
        if (existingWaffle != null)
        {
            if (existingWaffle.Quantity > 1)
            {
                existingWaffle.Quantity--;
            }
            else
            {
                OrderedItems.Remove(existingWaffle);
            }
        }
    }

    public decimal GetWafflePrice() => 8.00m;
   
    public decimal GetTotalPrice()
    {
        return OrderedItems.Sum(w => w.Quantity * w.WafflePrice);
    }
} 