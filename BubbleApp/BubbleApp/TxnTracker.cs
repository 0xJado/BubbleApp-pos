using Solnet.Rpc;
using Solnet.Rpc.Types;
using System.Diagnostics;
using BubbleAppNew.BubbleLib;

namespace BubbleAppNew;

public class TxnTracker
{
    private readonly IRpcClient _rpcClient;
        
        public TxnTracker()
        {
            //Comment out the one you don't need
            //Uses the devnet endpoint
            _rpcClient = ClientFactory.GetClient(Cluster.DevNet);
            // Uses the mainnet endpoint
            //_rpcClient = ClientFactory.GetClient(Cluster.MainNet);
        }

        public async Task<bool> WaitForPaymentAsync(string walletAddress, decimal expectedSol, TimeSpan timeout)
        {
            var stopwatch = Stopwatch.StartNew();
            ulong lamportsThreshold = (ulong)((expectedSol - 0.001m) * 1_000_000_000); // Convert SOL to lamports with wiggleroom
            
            Console.WriteLine($"Waiting for {expectedSol} SOL to reach wallet {walletAddress}");
           
            while (stopwatch.Elapsed < timeout)
            {
                var txs = await _rpcClient.GetSignaturesForAddressAsync(walletAddress, limit : 10);
                
                if (txs.WasSuccessful && txs.Result is { Count: > 0 })
                {
                    foreach (var txSig in txs.Result)
                    {
                        var tx = await _rpcClient.GetTransactionAsync(txSig.Signature);

                        if (tx.WasSuccessful && 
                            tx.Result?.Meta?.PreBalances != null &&
                            tx.Result?.Meta?.PostBalances != null &&
                            tx.Result.Transaction?.Message?.AccountKeys != null)
                        {
                            int index = Array.IndexOf(tx.Result.Transaction.Message.AccountKeys, walletAddress);
                            if (index == -1)
                                continue;
                            
                            var pre = tx.Result.Meta.PreBalances[index];
                            var post = tx.Result.Meta.PostBalances[index];
                            
                            var delta = post - pre;

                            if (delta >= lamportsThreshold)
                            {
                                Console.WriteLine($"Payment detected! Tx: {txSig.Signature} | Amount: {delta} lamports");
                                return true;
                            }
                        }
                    }
                }

                await Task.Delay(4000); // Checks every 4 seconds
            }
            Console.WriteLine("Payment not found within timeout period.");
            return false;
        }
}