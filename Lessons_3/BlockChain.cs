using System.Collections.Generic;

namespace BlockChainITStep;

public class BlockChain
{
    public List<Block> Chain { get; set; }
    private readonly HashingService _hashingService;
    private readonly MiningService _miningService;
    private int _difficulty = 1;
    public readonly decimal _targetBlockTime = 2; // Целевое время для нахождения блока в секундах
    public readonly int _adjustmentEveryNBlocks = 3; // Количество блоков, после которых будет происходить корректировка сложности


    public BlockChain(int difficulty)
    {
        Chain = new List<Block>();
        _hashingService = new HashingService();
        _miningService = new MiningService(_hashingService);
        _difficulty = difficulty; // Устанавливаем сложность майнинга (количество ведущих нулей)
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
        
        newBlock.DifficultyAtMining = _difficulty; // Сохраняем текущую сложность майнинга в блоке
        await _miningService.MineBlockAsync(newBlock, _difficulty);
        Chain.Add(newBlock);
        AdjustDifficulty(); // Корректируем сложность после добавления нового блока
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
            var target = new string(MiningService.Target, currentBlock.DifficultyAtMining);
            if (!currentBlock.Hash.StartsWith(target, StringComparison.Ordinal))
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

    private void AdjustDifficulty()
    {
        var minedBlockCount = Chain.Count - 1;
        if (minedBlockCount == 0 || minedBlockCount % _adjustmentEveryNBlocks != 0)
        {
            return;
        }

        var recentBlock = Chain.Where(b => b.Index > 0).TakeLast(_adjustmentEveryNBlocks).ToList();
        var avgTime = recentBlock.Average(b => b.MiningDuration);
        if (avgTime < _targetBlockTime)
        {
            _difficulty++;
            Console.WriteLine($"Difficulty increased to {_difficulty}");
        }
        else if (avgTime > _targetBlockTime)
        {
            _difficulty = Math.Max(1, _difficulty - 1); // Сложность не может быть меньше 1
            Console.WriteLine($"Difficulty decreased to {_difficulty}");
        }
    }

    public void PrintDifficultyHistory()
    {
        Console.WriteLine("Difficulty History:");
        foreach (var block in Chain)
        {
            Console.WriteLine(
                $"Block Index: {block.Index}, " +
                $"Difficulty: {block.DifficultyAtMining}, " +
                $"Attempts: {block.Attempts}, " +
                $"Mining Duration: {block.MiningDuration:F3} seconds");
        }
    }

    public void FindFirstInvalidBlock()
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
                Console.WriteLine($"Invalid block found at index {currentBlock.Index}: Hash mismatch.");
                return;
            }

            // Проверка хеша предыдущего блока
            if (currentBlock.PrevHash != previousBlock.Hash)
            {
                Console.WriteLine($"Invalid block found at index {currentBlock.Index}: Previous hash mismatch.");
                return;
            }

            // Проверка Proof of Work
            var target = new string(
                MiningService.Target,
                currentBlock.DifficultyAtMining);
            if (!currentBlock.Hash.StartsWith(target, StringComparison.Ordinal))
            {
                Console.WriteLine(
                    $"Invalid block found at index {currentBlock.Index}: " +
                    "Proof of Work mismatch.");
                return;
            }
        }

        Console.WriteLine("No invalid blocks found in the chain.");
    }

    // public void FindTransactionById(int Id)
    // {
    //     foreach (var block in Chain)
    //     {
    //         var transaction = block.Transactions?.Find(t => t.Id == Id);
    //         if (transaction != null)
    //         {
    //             Console.WriteLine($"Transaction found in Block Index: {block.Index}");
    //             Console.WriteLine($"Transaction ID: {transaction.Id}");
    //             Console.WriteLine($"Sender: {transaction.Sender}");
    //             Console.WriteLine($"Receiver: {transaction.Receiver}");
    //             Console.WriteLine($"Amount: {transaction.Amount}");
    //             return;
    //         }
    //     }
    // }

    // public void FindTransactionsByUser(string user)
    // {
    //     foreach (var block in Chain)
    //     {
    //         var transactions = block.Transactions?.FindAll(t => t.Sender == user || t.Receiver == user);
    //         if (transactions != null && transactions.Count > 0)
    //         {
    //             Console.WriteLine($"Transactions found in Block Index: {block.Index}");
    //             foreach (var transaction in transactions)
    //             {
    //                 Console.WriteLine($"Transaction ID: {transaction.Id}");
    //                 Console.WriteLine($"Sender: {transaction.Sender}");
    //                 Console.WriteLine($"Receiver: {transaction.Receiver}");
    //                 Console.WriteLine($"Amount: {transaction.Amount}");
    //                 Console.WriteLine(new string('-', 30));
    //             }
    //         }
    //     }
    // }


}
