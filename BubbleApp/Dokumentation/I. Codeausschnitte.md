
{#apdx-codeausschnitte}
## Code-Ausschnitt

Hier ist exemplarisch die Klasse TxnTracker (Transaktions Tracker) abgebildet. Sie ist dafür verantwortlich den Zahlungseingang zu einer Bestellung zu verifizieren. Eine Bestellung gilt als bezahlt, wenn auf der Solana Blockchain eine finalisierte Transaktion vorhanden ist, die der Wallet des Betreibers zugeordnet ist, die OrderID der Bestellung und den richtigen Solana Betrag enthält. 

Der Code wurde für eine bessere Lesbarkeit in dieser Dokumentation leicht umformatiert.

```csharp
using Solnet.Rpc;
using Solnet.Rpc.Models;
using System.Diagnostics;
using Solnet.Rpc.Core.Http;

namespace BubbleApp;

/// <summary>
/// Tracks transactions on the Solana blockchain to detect a finalized payment
/// with a matching order ID (used as memo) and the expected SOL amount.
/// </summary>
public class TxnTracker
{
    private readonly IRpcClient _rpcClient;
    private const decimal LAMPORTS_PER_SOL = 1_000_000_000m;

    /// <summary>
    /// Initializes the transaction tracker with a Solana DevNet RPC client.
    /// </summary>
    /// <param name="rpcClient">
    /// The Rpc Client to reach the Solana blockchain.
    /// </param>
    public TxnTracker(IRpcClient rpcClient)
    {
        // Die Kommunikation mit der Solana Blockchain erfolgt über den
        // IRpcClient aus der Solnet Bibliothek.
        // Dependency injection ist verantwortlich eine konkrete Instanz
        // bereitzustellen.
        _rpcClient = rpcClient;
    }

    /// <summary>
    /// Asynchronously waits for a confirmed transaction on the blockchain that
    /// matches the expected memo and lamport value.
    /// </summary>
    /// <param name="walletAddress">The public key of the recipient wallet.</param>
    /// <param name="expectedSol">The expected transaction amount in SOL.</param>
    /// <param name="orderId">
    /// The unique order identifier used in the transaction memo.
    /// </param>
    /// <param name="timeout">
    /// The maximum duration to wait for a matching transaction.
    /// </param>
    /// <returns>
    /// The signature of the matching transaction, or null if no match is found
    /// within the timeout period.
    /// </returns>
    public async Task<string?> WaitForPaymentAsync(
        string walletAddress,
        decimal expectedSol,
        string orderId,
        TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        ulong expectedLamports = (ulong)(expectedSol * LAMPORTS_PER_SOL);
        string expectedMemo = $"Order-{orderId}";

        Console.WriteLine(
            $"Waiting for transaction with Order ID '{expectedMemo}' " +
            $"and {expectedLamports} lamports...");

        while (stopwatch.Elapsed < timeout)
        {
            var sigResult = await _rpcClient.GetSignaturesForAddressAsync(
                                                walletAddress,
                                                limit: 10);

            if (!sigResult.WasSuccessful || sigResult.Result == null)
            {
                await Task.Delay(1000);
                continue;
            }

            foreach (var sigInfo in GetFinalizedSignaturesWithExpectedOrderId(
                                        sigResult.Result,
                                        expectedMemo))
            {
                var tx = await _rpcClient.GetTransactionAsync(sigInfo.Signature);

                if (IsRequestResultRelevant(tx))
                    continue;

                if (!TryGetLamportAmount(tx.Result,
                                         walletAddress,
                                         out var lamportAmount)) 
                    continue;

                Console.WriteLine(
                    $"Transaction {sigInfo.Signature} has lamport amount: " +
                    $"{lamportAmount}");

                if (lamportAmount >= expectedLamports)
                {
                    Console.WriteLine(
                        $"Payment confirmed for Order-{orderId}. " + 
                        $"Tx: {sigInfo.Signature}");
                    return sigInfo.Signature;
                }
            }

            await Task.Delay(1000);
        }

        Console.WriteLine("No matching transaction found within timeout.");
        return null;
    }

    /// <summary>
    /// Tries to get the lamport amount from the transaction metadata.
    /// </summary>
    /// <param name="transactionMetadata">The transactions Metadata.</param>
    /// <param name="walletAddress">The Owner's Wallet Address.</param>
    /// <param name="amount">The amount of Solana (Lamports).</param>
    /// <returns>
    /// If the amount is found, then returns true, otherwise, false.
    /// </returns>
    private static bool TryGetLamportAmount(
        TransactionMetaSlotInfo transactionMetadata,
        string walletAddress,
        out ulong amount)
    {
        amount = 0;
        // Get Solana value is not supported in the current version of Solnet,
        // Delta is used instead because the Pre- and Postbalances can be extracted
        // natively from the blockchain. This is adequate for the MVP.
        var keys = transactionMetadata.Transaction.Message.AccountKeys;
        var index = Array.IndexOf(keys, walletAddress);
        if (index == -1) 
            return false;

        var pre = transactionMetadata.Meta.PreBalances[index];
        var post = transactionMetadata.Meta.PostBalances[index];
        amount = post - pre;
        return true;
    }

    /// <summary>
    /// Checks if the transaction is relevant by verifying if it was successful and
    /// if there are any errors in the metadata.
    /// </summary>
    /// <param name="tx">Metadata of the RequestResult.</param>
    /// <returns>True if the Result is relevant, otherwise, false.</returns>
    private static bool IsRequestResultRelevant(
        RequestResult<TransactionMetaSlotInfo> tx)
    {
        return !tx.WasSuccessful
               || tx.Result?.Transaction == null
               || tx.Result.Meta?.Error != null;
    }

    /// <summary>
    /// Filters the transaction signatures to only include finalized entries with the
    /// specified memo.
    /// </summary>
    /// <param name="signatureInfos">
    /// The list of signature status information objects.
    /// </param>
    /// <param name="expectedOrderId">
    /// The expected memo string to match against.
    /// </param>
    /// <returns>Filtered enumerable of matching signature status entries.</returns>
    private static IEnumerable<SignatureStatusInfo>
        GetFinalizedSignaturesWithExpectedOrderId(
            List<SignatureStatusInfo> signatureInfos,
            string expectedOrderId)
    {
        // Pre-filter by memo and confirmation status to reduce load
        return signatureInfos.Where(sig =>
        {
            (sig.Memo?.Contains(
                            expectedOrderId,
                            StringComparison.OrdinalIgnoreCase) ?? false)
            && sig.ConfirmationStatus == "finalized"
        });
    }
}
```
