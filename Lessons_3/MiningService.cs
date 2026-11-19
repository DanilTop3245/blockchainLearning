using System.Diagnostics;

namespace BlockChainITStep;

public class MiningService
{
    public const char Target = '0';

    private readonly HashingService _hashingService;

    public MiningService(HashingService hashingService)
    {
        _hashingService = hashingService;
    }

    public void MineBlock(Block block, int difficulty)
    {
        if (difficulty <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(difficulty),
                "Difficulty must be greater than zero.");
        }

        var target = new string(Target, difficulty);
        var nonce = 0;
        var startTime = Stopwatch.StartNew();
        while (true)
        {
            block.Nonce = nonce;
            var hash = _hashingService.ComputeBlockHash(block);
            if (hash.StartsWith(target, StringComparison.Ordinal))
            {
                block.Hash = hash;
                block.MiningDuration = (decimal)startTime.Elapsed.TotalSeconds; // Сохраняем количество итераций (nonce) как время майнинга
                block.Attempts = nonce + 1;
                Console.WriteLine($"Block mined! Nonce: {block.Nonce}, Hash: {block.Hash}");
                break;
            }

            nonce++;
        }
    }

     public async Task MineBlockAsync(Block block, int difficulty)
    {
        if (difficulty <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(difficulty),
                "Difficulty must be greater than zero.");
        }

        var target = new string(Target, difficulty);
        using var cancellation = new CancellationTokenSource();
        var workerCount = Environment.ProcessorCount;
        var tasks = new Task[workerCount];
        var resultLock = new object();
        var resultNonce = 0;
        var resultHash = string.Empty;
        var startTime = Stopwatch.StartNew();

        for (var workerId = 0; workerId < workerCount; workerId++)
        {
            var startNonce = workerId;
            tasks[workerId] = Task.Run(() =>
            {
                var candidate = new Block
                {
                    Index = block.Index,
                    Data = block.Data,
                    PrevHash = block.PrevHash,
                    TimeStamp = block.TimeStamp,
                    Author = block.Author,
                    DifficultyAtMining = block.DifficultyAtMining
                };

                for (var nonce = startNonce; !cancellation.IsCancellationRequested; nonce += workerCount)
                {
                    candidate.Nonce = nonce;
                    var hash = _hashingService.ComputeBlockHash(candidate);
                    if (!hash.StartsWith(target, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    lock (resultLock)
                    {
                        if (!cancellation.IsCancellationRequested)
                        {
                            resultNonce = nonce;
                            resultHash = hash;
                            block.Attempts = nonce + 1; // Сохраняем количество попыток майнинга
                            block.MiningDuration = (decimal)startTime.Elapsed.TotalSeconds; // Сохраняем время майнинга в секундах
                            cancellation.Cancel();
                        }
                    }

                    break;
                }
            });
        }

        await Task.WhenAll(tasks);
        block.MiningDuration = (decimal)startTime.Elapsed.TotalSeconds;
        block.Nonce = resultNonce;
        block.Hash = resultHash;
        Console.WriteLine($"Block mined! Nonce: {block.Nonce}, Hash: {block.Hash}");
    }
}