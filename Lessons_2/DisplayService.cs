using BlockChainITStep;

public class DisplayService
{
    public void DisplayChain(BlockChain blockChain)
    {
        foreach (var block in blockChain.Chain)
        {
            Console.WriteLine($"Block Index: {block.Index}");
            Console.WriteLine($"Block Hash: {block.Hash}");
            Console.WriteLine($"Previous Hash: {block.PrevHash}");
            Console.WriteLine($"Data: {block.Data}");
            Console.WriteLine($"Timestamp: {block.TimeStamp}");
            Console.WriteLine(new string('-', 30));
        }
    }
}