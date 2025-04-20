using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace BubbleLib;

public interface IOrderStorageService
{
    Task<List<CompletedOrder>> GetOrdersAsync();
    Task<int> SaveOrderAsync(CompletedOrder order);
}

public class OrderStorageService : IOrderStorageService
{
    private readonly SQLiteAsyncConnection _database;

    public OrderStorageService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<CompletedOrder>().Wait();
    }

    public async Task<List<CompletedOrder>> GetOrdersAsync()
    {
        return await _database.Table<CompletedOrder>()
            .OrderByDescending(o => o.Timestamp)
            .ToListAsync();
    }

    public Task<int> SaveOrderAsync(CompletedOrder order) =>
        _database.InsertAsync(order);
}