using System.Collections.Generic;

namespace BlockChainITStep;

public class BlockChain
{
    public List<Block> Chain { get; set; }
    private readonly HashingService _hashingService;
    private readonly MiningService _miningService;
    public int Difficulty { get; private set; } = 4;


    public BlockChain(int difficulty)
    {
        Chain = new List<Block>();
        _hashingService = new HashingService();
        _miningService = new MiningService(_hashingService);
        this.Difficulty = difficulty; // Устанавливаем сложность майнинга (количество ведущих нулей)
        AddGenesisBlock("Genesis Block");

    }

    private void AddGenesisBlock(string data)
    {
        var genesisBlock = new Block
        {
            Index = 0,
            Data = data,
            PrevHash = "0",
            TimeStamp = DateTime.Parse("2023-01-01T00:00:00Z"),
            Author = "Dani" // Устанавливаем автора генезис-блока
        };

        genesisBlock.Hash = _hashingService.ComputeBlockHash(genesisBlock);
        Chain.Add(genesisBlock);
    }

    public async Task AddBlock(string data) {
        var lastBlock = Chain[Chain.Count - 1];
        var newBlock = new Block
        {
            Index = lastBlock.Index + 1,
            Data = data,
            PrevHash = lastBlock.Hash,
            TimeStamp = DateTime.Now,
            Author = "Danil" // Устанавливаем автора блока
        };
        
        await _miningService.MineBlockAsync(newBlock, Difficulty);
        Chain.Add(newBlock);
    }

    public bool IsChainValid()
    {
        for (int i = 1; i < Chain.Count; i++)
        {
            var currentBlock = Chain[i];
            var previousBlock = Chain[i - 1];

            if (currentBlock.Index == 0)
            {
                continue; // Пропускаем проверку для генезис-блока
            }

            // Проверка хеша текущего блока
            if (currentBlock.Hash != _hashingService.ComputeBlockHash(currentBlock))
            {
                return false;
            }

            // Проверка хеша предыдущего блока
            if (currentBlock.PrevHash != previousBlock.Hash)
            {
                return false;
            }
            if (currentBlock.Hash.Substring(0, Difficulty) != MiningService.Target[..Difficulty])
            {
                return false;
            }
        }

        return true;
    }

    public void SearchBlockByIndex(int Index)
    {
        var block = Chain.Find(b => b.Index == Index);
        if (block != null)
        {
            Console.WriteLine($"Block Index: {block.Index}");
            Console.WriteLine($"Block Hash: {block.Hash}");
            Console.WriteLine($"Previous Hash: {block.PrevHash}");
            Console.WriteLine($"Data: {block.Data}");
            Console.WriteLine($"Timestamp: {block.TimeStamp}");
        }
        else
        {
            Console.WriteLine($"Block with index {Index} not found.");
        }
    }

    public void SearchEventByText(string text)
    {
        var blocks = Chain.FindAll(b => b.Data.Contains(text));
        if (blocks.Count > 0)
        {
            foreach (var block in blocks)
            {
                Console.WriteLine($"Block Index: {block.Index}");
                Console.WriteLine($"Block Hash: {block.Hash}");
                Console.WriteLine($"Previous Hash: {block.PrevHash}");
                Console.WriteLine($"Data: {block.Data}");
                Console.WriteLine($"Timestamp: {block.TimeStamp}");
                Console.WriteLine(new string('-', 30));
            }
        }
        else
        {
            Console.WriteLine($"No blocks found containing the text '{text}'.");
        }
    }
}
