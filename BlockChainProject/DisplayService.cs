using BlockChainITStep;

public class DisplayService
{
    public void DisplayChain(BlockChain blockChain)
    {
        foreach (var block in blockChain.Chain)
        {
            DisplayBlock(block);
        }
    }

    public void DisplayBlock(Block block)
    {
        Console.WriteLine($"Block Index: {block.Index}");
        Console.WriteLine($"Data: {block.Data}");
        Console.WriteLine($"Timestamp: {block.TimeStamp}");
        Console.WriteLine($"Previous Hash: {block.PrevHash}");
        Console.WriteLine($"Block Hash: {block.Hash}");
        Console.WriteLine($"Author: {block.Author}");
        Console.WriteLine($"Nonce: {block.Nonce}");
        Console.WriteLine($"Mining Duration: {block.MiningDuration} seconds");

        foreach (var transaction in block.Transactions)
        {
            Console.WriteLine(
                $"Transaction ID {transaction.Id}" +
                $"\tTransaction: From {transaction.From} " +
                $"To {transaction.To} Amount {transaction.Amount}");
        }

        Console.WriteLine(new string('-', 30));
    }
}