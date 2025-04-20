using BubbleLib;
using SQLite;

namespace UnitTests;

public class OrderDbTests;

public class OrderStorageServiceTests
{ 
    private SQLiteAsyncConnection _connection;
    private OrderStorageService _orderStorageService; 
    
    [SetUp]
    public void Setup()
    { 
        // Use in-memory database for test isolation
        _connection = new SQLiteAsyncConnection(":memory:");
        _connection.CreateTableAsync<CompletedOrder>().Wait();
        _connection.DeleteAllAsync<CompletedOrder>().Wait(); // Ensures order table is clear between tests

        _orderStorageService = new OrderStorageService(":memory:");
    }
    
    [Test]
    public async Task SaveOrderIntoDatabase()
    {
        // Arrange
        var order = new CompletedOrder
        {
            OrderId = "test123", 
            ItemsSummary = "1x Cookie Moment",
            TotalEur = 8.00m,
            TotalSol = 0.075m,
            Wallet = "testWallet",
            Email = "test@email.com",
            TxSignature = "testSignature",
            Timestamp = DateTime.Now
        };

        // Act
        var result = await _orderStorageService.SaveOrderAsync(order);

        // Assert
        Assert.That(result, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetSavedOrdersReturned()
    {
        // Arrange
        await SaveOrderIntoDatabase(); // saves one

        // Act
        var orders = await _orderStorageService.GetOrdersAsync();

        // Assert
        Assert.That(orders.Count, Is.EqualTo(1));
        Assert.That(orders[0].OrderId, Is.EqualTo("test123")); 
    }
    
    [Test]
    public async Task GetOrdersSortedByTimestamp()
    {
        // Arrange - insert 3 orders with increasing timestamps
        var order1 = new CompletedOrder
        {
            OrderId = "order1",
            ItemsSummary = "1x Banana Split",
            TotalEur = 8.00m,
            TotalSol = 0.070m,
            Wallet = "wallet1",
            Email = "a@example.com",
            TxSignature = "tx1",
            Timestamp = DateTime.Now.AddMinutes(-10)
        };

        var order2 = new CompletedOrder
        {
            OrderId = "order2",
            ItemsSummary = "2x Flamingo Moment",
            TotalEur = 16.00m,
            TotalSol = 0.140m,
            Wallet = "wallet2",
            Email = "b@example.com",
            TxSignature = "tx2",
            Timestamp = DateTime.Now.AddMinutes(-5)
        };

        var order3 = new CompletedOrder
        {
            OrderId = "order3",
            ItemsSummary = "1x Cookie",
            TotalEur = 8.00m,
            TotalSol = 0.060m,
            Wallet = "wallet3",
            Email = "c@example.com",
            TxSignature = "tx3",
            Timestamp = DateTime.Now
        };

        await _orderStorageService.SaveOrderAsync(order1);
        await _orderStorageService.SaveOrderAsync(order2);
        await _orderStorageService.SaveOrderAsync(order3);

        // Act
        var orders = await _orderStorageService.GetOrdersAsync();

        // Assert
        Assert.That(orders.Count, Is.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(orders[0].OrderId, Is.EqualTo("order3"));
            Assert.That(orders[1].OrderId, Is.EqualTo("order2"));
            Assert.That(orders[2].OrderId, Is.EqualTo("order1"));
        });
    }
}