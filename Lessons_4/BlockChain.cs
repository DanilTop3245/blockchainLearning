using System.Collections.Generic;

namespace BlockChainITStep;

public enum TransactionSearchMode
{
    All,
    Sent,
    Received
}

public class BlockChain
{
    public List<Block> Chain { get; set; }
    private readonly HashingService _hashingService;
    private readonly MiningService _miningService;
    private readonly TransactionService _transactionService = new();
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
            Transactions = new(),
            PrevHash = "0",
            TimeStamp = DateTime.Parse("2023-01-01T00:00:00Z"),
            Author = "Dani" // Устанавливаем автора генезис-блока
        };

        genesisBlock.Hash = _hashingService.ComputeBlockHash(genesisBlock);
        Chain.Add(genesisBlock);
    }

    public async Task AddBlock(List<Transaction> transactions)
    {
        foreach (var transaction in transactions)
        {
            var validationResult = _transactionService.ValidateTransaction(transaction);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(
                    $"Invalid transaction: {validationResult.ErrorMessage}");
            }
        }

        var transactionCopy = new List<Transaction>(transactions);
        var lastBlock = Chain[Chain.Count - 1];
        var newBlock = new Block
        {
            Index = lastBlock.Index + 1,
            Transactions = transactionCopy,
            PrevHash = lastBlock.Hash,
            TimeStamp = DateTime.Now,
            Author = "Danil" // Устанавливаем автора блока
        };
        
        newBlock.DifficultyAtMining = _difficulty; // Сохраняем текущую сложность майнинга в блоке
        await _miningService.MineBlockAsync(newBlock, _difficulty);
        Chain.Add(newBlock);
        AdjustDifficulty(); // Корректируем сложность после добавления нового блока
    }

    public async Task AddEventBlock(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("Event data is required.", nameof(data));
        }

        var lastBlock = Chain[Chain.Count - 1];
        var newBlock = new Block
        {
            Index = lastBlock.Index + 1,
            Data = data,
            PrevHash = lastBlock.Hash,
            TimeStamp = DateTime.Now,
            Author = "Danil"
        };

        newBlock.DifficultyAtMining = _difficulty;
        await _miningService.MineBlockAsync(newBlock, _difficulty);
        Chain.Add(newBlock);
        AdjustDifficulty();
    }

    public bool IsValid()
    {
        for (int i = 0; i < Chain.Count; i++)
        {
            var currentBlock = Chain[i];
            var expectedHash = _hashingService.ComputeBlockHash(currentBlock);

            if (currentBlock.Hash != expectedHash)
            {
                return false;
            }

            if (i == 0)
            {
                if (currentBlock.PrevHash != "0")
                {
                    return false;
                }

                continue;
            }

            var previousBlock = Chain[i - 1];
            if (currentBlock.PrevHash != previousBlock.Hash)
            {
                return false;
            }

            var target = new string(MiningService.Target,
                currentBlock.DifficultyAtMining);
            if (!currentBlock.Hash.StartsWith(target, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsChainValid()
    {
        return IsValid();
    }

    public void SearchBlockByIndex(int Index)
    {
        var block = Chain.Find(b => b.Index == Index);
        if (block != null)
        {
            new DisplayService().DisplayBlock(block);
        }
        else
        {
            Console.WriteLine($"Block with index {Index} not found.");
        }
    }

    public void SearchEventByText(string text)
    {
        var blocks = Chain.FindAll(b =>
            b.Data.Contains(text, StringComparison.OrdinalIgnoreCase));
        if (blocks.Count > 0)
        {
            foreach (var block in blocks)
            {
                new DisplayService().DisplayBlock(block);
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
        for (int i = 0; i < Chain.Count; i++)
        {
            var currentBlock = Chain[i];

            // Проверка хеша текущего блока
            if (currentBlock.Hash != _hashingService.ComputeBlockHash(currentBlock))
            {
                Console.WriteLine($"Invalid block found at index {currentBlock.Index}: Hash mismatch.");
                return;
            }

            if (i == 0)
            {
                if (currentBlock.PrevHash != "0")
                {
                    Console.WriteLine(
                        $"Invalid block found at index {currentBlock.Index}: " +
                        "Genesis previous hash mismatch.");
                    return;
                }

                continue;
            }

            var previousBlock = Chain[i - 1];

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

    public void FindTransactionById(string id)
    {
        foreach (var block in Chain)
        {
            var transaction = block.Transactions.Find(t => t.Id == id);
            if (transaction != null)
            {
                Console.WriteLine($"Transaction found in Block Index: {block.Index}");
                Console.WriteLine(transaction);
                return;
            }
        }

        Console.WriteLine($"Transaction with Id {id} was not found.");
    }

    public void FindTransactionsByUser(
        string user,
        TransactionSearchMode searchMode)
    {
        var found = false;

        foreach (var block in Chain)
        {
            var transactions = block.Transactions.FindAll(transaction =>
                searchMode switch
                {
                    TransactionSearchMode.Sent => transaction.From == user,
                    TransactionSearchMode.Received => transaction.To == user,
                    _ => transaction.From == user || transaction.To == user
                });

            if (transactions.Count > 0)
            {
                found = true;
                Console.WriteLine($"Transactions found in Block Index: {block.Index}");
                foreach (var transaction in transactions)
                {
                    Console.WriteLine(transaction);
                    Console.WriteLine(new string('-', 30));
                }
            }
        }

        if (!found)
        {
            Console.WriteLine(
                $"No transactions found for user {user} with filter {searchMode}.");
        }
    }

    public async Task CreateDemoChainAsync()
    {
        var demoTransactions = new[]
        {
            _transactionService.CreateTransaction("Alice", "Bob", 10),
            _transactionService.CreateTransaction("Bob", "Charlie", 5),
            _transactionService.CreateTransaction("Charlie", "Alice", 2),
            _transactionService.CreateTransaction("Alice", "Dave", 15),
            _transactionService.CreateTransaction("Dave", "Bob", 7),
            _transactionService.CreateTransaction("Bob", "Alice", 3),
            _transactionService.CreateTransaction("Charlie", "Dave", 11),
            _transactionService.CreateTransaction("Dave", "Alice", 4),
            _transactionService.CreateTransaction("Alice", "Charlie", 8),
            _transactionService.CreateTransaction("Bob", "Dave", 6),
            _transactionService.CreateTransaction("Dave", "Charlie", 9),
            _transactionService.CreateTransaction("Charlie", "Bob", 1)
        };

        for (var i = 0; i < demoTransactions.Length; i += 3)
        {
            await AddBlock(new List<Transaction>(demoTransactions[i..(i + 3)]));
        }
    }


}
