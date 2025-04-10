using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System.Diagnostics;

namespace BubbleAppNew;

public class TxnTracker
{
    private readonly IRpcClient _rpcClient;

    public TxnTracker()
    {
        //Comment out the one you don't need
        //Uses the devnet endpoint
        _rpcClient = ClientFactory.GetClient(Cluster.DevNet);
        //Uses the mainnet endpoint
        //_rpcClient = ClientFactory.GetClient(Cluster.MainNet);
    }

    public async Task<bool> WaitForPaymentAsync(string walletAddress, decimal expectedSol, string orderId, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        var expectedMemo = $"Order-{orderId}";
        ulong expectedLamports = (ulong)(expectedSol * 1_000_000_000m);

        Console.WriteLine($"Waiting for payment: {expectedSol} SOL with memo '{expectedMemo}'");

        while (stopwatch.Elapsed < timeout)
        {
            var sigResult = await _rpcClient.GetSignaturesForAddressAsync(walletAddress, limit: 10);

            if (sigResult.WasSuccessful && sigResult.Result?.Count > 0)
            {
                foreach (var sigInfo in sigResult.Result)
                {
                    var tx = await _rpcClient.GetTransactionAsync(sigInfo.Signature, Commitment.Finalized);
                    if (!tx.WasSuccessful || tx.Result?.Transaction == null || tx.Result.Meta?.Error != null)
                        continue;

                    bool memoMatch = false;

                    //Look inside LogMessages
                    var logs = tx.Result.Meta?.LogMessages;
                    if (logs != null)
                    {
                        foreach (var log in logs)
                        {
                            if (log.Contains(expectedMemo, StringComparison.OrdinalIgnoreCase))
                            {
                                memoMatch = true;
                                Console.WriteLine($"Found memo in logs: {log}");
                                break;
                            }
                        }
                    }

                    if (!memoMatch) continue;

                    //Validate amount received
                    var accountIndex = Array.IndexOf(tx.Result.Transaction.Message.AccountKeys, walletAddress);
                    if (accountIndex == -1) continue;

                    var pre = tx.Result.Meta.PreBalances[accountIndex];
                    var post = tx.Result.Meta.PostBalances[accountIndex];
                    var delta = post - pre;

                    Console.WriteLine($"Delta received: {delta} lamports (expected: {expectedLamports})");

                    if ((ulong)delta >= expectedLamports)
                    {
                        Console.WriteLine($"Payment confirmed for Order-{orderId}! Tx: {sigInfo.Signature}");
                        return true;
                    }
                }
            }

            await Task.Delay(2000);
        }

        Console.WriteLine("Payment not detected within timeout.");
        return false;
    }
}