using BubbleLib;
using Moq;

namespace UnitTests;

public class WaffleServiceTests
{
    private IWaffleService _waffleService;

    [SetUp]
    public void Setup()
    {
        _waffleService = new WaffleService();
    }

    [Test]
    public void AddNewWaffleToOrder()
    {
        // Arrange
        var waffle = new BubbleWaffle { WaffleName = "Berry Moment" };
       
        //Act
        _waffleService.AddWaffleToOrder(waffle);

        // Assert
        Assert.That(_waffleService.OrderedItems.Count, Is.EqualTo(1));
        Assert.That(_waffleService.OrderedItems.First().Quantity, Is.EqualTo(1));
    }

    [Test]
    public void IncrementalAddWaffleToOrder()
    {
        var waffle = new BubbleWaffle { WaffleName = "Caramel Moment" };
        _waffleService.AddWaffleToOrder(waffle);
        _waffleService.AddWaffleToOrder(waffle);

        Assert.That(_waffleService.OrderedItems.Count, Is.EqualTo(1));
        Assert.That(_waffleService.OrderedItems.First().Quantity, Is.EqualTo(2));
    }

    [Test]
    public void DecrementalRemoveWaffleFromOrder()
    {
        var waffle = new BubbleWaffle { WaffleName = "Berry Moment" };
        _waffleService.AddWaffleToOrder(waffle);
        _waffleService.AddWaffleToOrder(waffle);

        _waffleService.RemoveWaffleFromOrder(waffle);

        Assert.That(_waffleService.OrderedItems.First().Quantity, Is.EqualTo(1));
    }

    [Test]
    public void FullyRemoveWaffleFromOrder()
    {
        var waffle = new BubbleWaffle { WaffleName = "Cookie Moment" };
        _waffleService.AddWaffleToOrder(waffle);

        _waffleService.RemoveWaffleFromOrder(waffle);

        Assert.That(_waffleService.OrderedItems.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetCorrectTotalSummedPrice()
    {
        var waffle1 = new BubbleWaffle { WaffleName = "A", Quantity = 1, CustomPrice = 5m };
        var waffle2 = new BubbleWaffle { WaffleName = "B", Quantity = 2, CustomPrice = 7m };

        _waffleService.OrderedItems.Add(waffle1);
        _waffleService.OrderedItems.Add(waffle2);

        var total = _waffleService.GetTotalPrice();

        Assert.That(total, Is.EqualTo(5 + (2 * 7)));
    }
    
    [Test]
    public void ClearOldOrder()
    {
        var waffle1 = new BubbleWaffle { WaffleName = "Kids Moment" };
        var waffle2 = new BubbleWaffle { WaffleName = "Latte Macchiato Moment" };
        _waffleService.AddWaffleToOrder(waffle1);
        _waffleService.AddWaffleToOrder(waffle2);
        Assert.That(_waffleService.OrderedItems.Count, Is.EqualTo(2));
        
        _waffleService.ClearOrder();

        Assert.That(_waffleService.OrderedItems.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void TotalPrice_ShouldCalculateCorrectly_BasedOnQuantityAndUnitPrice()
    {
        // Arrange
        var bubble = new BubbleWaffle
        {
            WaffleName = "Caramel Moment",
            Quantity = 3
        };

        // Act
        var total = bubble.TotalPrice;

        // Assert
        Assert.That(total, Is.EqualTo(24.00m));
    }

}