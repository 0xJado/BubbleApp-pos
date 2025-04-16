using System;
using SQLite;

namespace BubbleLib;

public class CompletedOrder
{
    [PrimaryKey, AutoIncrement]
    public int OrderNumber { get; set; }

    public string OrderId { get; set; }
    public string ItemsSummary { get; set; }
    public decimal TotalEur { get; set; }
    public decimal TotalSol { get; set; }
    public string Wallet { get; set; }
    public string Email { get; set; }
    public string TxSignature { get; set; }
    public DateTime Timestamp { get; set; }
}