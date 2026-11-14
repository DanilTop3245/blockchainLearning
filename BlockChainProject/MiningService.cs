namespace BlockChainITStep;

public class MiningService
{
    public const string Target = "dani";

    private readonly HashingService _hashingService;

    public MiningService(HashingService hashingService)
    {
        _hashingService = hashingService;
    }

    public void MineBlock(Block block, int difficulty)
    {
        var target = Target;
        if (difficulty != target.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(difficulty), target.Length,
                "Difficulty must match the target length.");
        }

        var nonce = 0;
        while (true)
        {
            block.Nonce = nonce;
            var hash = _hashingService.ComputeBlockHash(block);
            if (hash.StartsWith(target, StringComparison.Ordinal))
            {
                block.Hash = hash;
                Console.WriteLine($"Block mined! Nonce: {block.Nonce}, Hash: {block.Hash}");
                break;
            }

            nonce++;
        }
    }

     public async Task MineBlockAsync(Block block, int difficulty)
    {
        var target = Target;
        if (difficulty != target.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(difficulty), target.Length,
                "Difficulty must match the target length.");
        }

        using var cancellation = new CancellationTokenSource();
        var workerCount = Environment.ProcessorCount;
        var tasks = new Task[workerCount];
        var resultLock = new object();
        var resultNonce = 0;
        var resultHash = string.Empty;

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
                    Author = block.Author
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
                            cancellation.Cancel();
                        }
                    }

                    break;
                }
            });
        }

        await Task.WhenAll(tasks);
        block.Nonce = resultNonce;
        block.Hash = resultHash;
        Console.WriteLine($"Block mined! Nonce: {block.Nonce}, Hash: {block.Hash}");
    }
}