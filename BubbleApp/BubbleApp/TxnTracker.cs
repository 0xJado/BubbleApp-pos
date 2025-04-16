using Solnet.Rpc;
using Solnet.Rpc.Models;
using System.Diagnostics;

namespace BubbleAppNew;

/// <summary>
/// Tracks transactions on the Solana blockchain to detect a finalized payment with a matching order ID and expected SOL amount.
/// </summary>
public class TxnTracker
{
    private readonly IRpcClient _rpcClient;

    public TxnTracker()
    {
        // Uses DevNet for testing; switch to MainNet when needed
        _rpcClient = ClientFactory.GetClient(Cluster.DevNet);
    }

    /// <summary>
    /// Waits for a payment matching a given orderId (via memo) and expected SOL amount.
    /// </summary>
    public async Task<string?> WaitForPaymentAsync(string walletAddress, decimal expectedSol, string orderId, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        ulong expectedLamports = (ulong)(expectedSol * 1_000_000_000m);
        string expectedMemo = $"Order-{orderId}";

        Console.WriteLine($"Waiting for transaction with Order ID '{expectedMemo}' and {expectedLamports} lamports...");

        while (stopwatch.Elapsed < timeout)
        {
            var sigResult = await _rpcClient.GetSignaturesForAddressAsync(walletAddress, limit: 10);

            if (!sigResult.WasSuccessful || sigResult.Result == null)
            {
                await Task.Delay(1000);
                continue;
            }

            foreach (var sigInfo in GetFinalizedSignaturesWithExpectedOrderId(sigResult.Result, expectedMemo))
            {
                var tx = await _rpcClient.GetTransactionAsync(sigInfo.Signature);

                if (!tx.WasSuccessful || tx.Result?.Transaction == null || tx.Result.Meta?.Error != null)
                    continue;

                var keys = tx.Result.Transaction.Message.AccountKeys;
                var index = Array.IndexOf(keys, walletAddress);
                if (index == -1) continue;

                var pre = tx.Result.Meta.PreBalances[index];
                var post = tx.Result.Meta.PostBalances[index];
                var delta = post - pre;

                Console.WriteLine($"Transaction {sigInfo.Signature} has lamport delta: {delta}");

                if (delta >= expectedLamports)
                {
                    Console.WriteLine($"Payment confirmed for Order-{orderId}. Tx: {sigInfo.Signature}");
                    return sigInfo.Signature;
                }
            }

            await Task.Delay(1000);
        }

        Console.WriteLine("No matching transaction found within timeout.");
        return null;
    }

    /// <summary>
    /// Filters only finalized transactions that contain the expected order ID in their memo.
    /// Fixes an issue where we would get confirmed transactions with the same amount but a different
    /// txn ID from previous orders that were called from the initial call. 
    /// </summary>
    private static IEnumerable<SignatureStatusInfo> GetFinalizedSignaturesWithExpectedOrderId(
        List<SignatureStatusInfo> signatureInfos,
        string expectedOrderId)
    {
        // Pre-filter by memo and confirmation status to reduce load
        return signatureInfos.Where(sig =>
            (sig.Memo?.Contains(expectedOrderId, StringComparison.OrdinalIgnoreCase) ?? false) &&
            sig.ConfirmationStatus == "finalized");
    }
}