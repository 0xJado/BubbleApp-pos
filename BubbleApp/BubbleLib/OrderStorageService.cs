using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace BubbleLib;

/// Interface for managing order storage operations using SQLite.
public interface IOrderStorageService
{
    /// Retrieves a list of completed orders, sorted by timestamp in descending order.
    Task<List<CompletedOrder>> GetOrdersAsync();

    /// Saves a completed order to the database.
    Task<int> SaveOrderAsync(CompletedOrder order);
}

/// Implements persistent storage for completed orders using SQLite.
public class OrderStorageService : IOrderStorageService
{
    private readonly SQLiteAsyncConnection _database;

    /// Initializes a new SQLite connection and creates the CompletedOrder table if it doesn't exist.
    public OrderStorageService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<CompletedOrder>().Wait();
    }

    /// Returns all orders from the database, sorted with the newest first.
    public async Task<List<CompletedOrder>> GetOrdersAsync()
    {
        return await _database.Table<CompletedOrder>()
            .OrderByDescending(o => o.Timestamp)
            .ToListAsync();
    }

    /// Inserts a new completed order into the database.
    public Task<int> SaveOrderAsync(CompletedOrder order) =>
        _database.InsertAsync(order);
}