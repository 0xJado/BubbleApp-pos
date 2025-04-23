using Solnet.Rpc;
using Solnet.Rpc.Models;
using System.Diagnostics;

namespace BubbleAppNew;

/// <summary>
/// Tracks transactions on the Solana blockchain to detect a finalized payment
/// with a matching order ID (used as memo) and the expected SOL amount.
/// </summary>
public class TxnTracker
{
    private readonly IRpcClient _rpcClient;

    /// <summary>
    /// Initializes the transaction tracker with a Solana DevNet RPC client.
    /// </summary>
    public TxnTracker()
    {
        // Uses DevNet for testing; for production, switch to MainNet.
        _rpcClient = ClientFactory.GetClient(Cluster.DevNet);
    }

    /// <summary>
    /// Asynchronously waits for a confirmed transaction on the blockchain that matches the expected memo and lamport value.
    /// </summary>
    /// <param name="walletAddress">The public key of the recipient wallet.</param>
    /// <param name="expectedSol">The expected transaction amount in SOL.</param>
    /// <param name="orderId">The unique order identifier used in the transaction memo.</param>
    /// <param name="timeout">The maximum duration to wait for a matching transaction.</param>
    /// <returns>The signature of the matching transaction, or null if no match is found within the timeout period.</returns>
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
    /// Filters the transaction signatures to only include finalized entries with the specified memo.
    /// </summary>
    /// <param name="signatureInfos">The list of signature status information objects.</param>
    /// <param name="expectedOrderId">The expected memo string to match against.</param>
    /// <returns>Filtered enumerable of matching signature status entries.</returns>
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