using BlockChainITStep;
using System.Threading.Tasks;

// var blockChainService = new BlockChain();
// var displayService = new DisplayService();

// blockChainService.AddBlock("Alex send Bob 100");
// blockChainService.AddBlock("Bob send Dave 20");
// blockChainService.AddBlock("Dave send Ann 30");
// blockChainService.AddBlock("Ann send Vlad 50");
// blockChainService.AddBlock("Vlad send Danil 50");
// blockChainService.AddBlock("Danil send Andrey 250");

// displayService.DisplayChain(blockChainService);

// var isValid = blockChainService.IsChainValid();
// if (isValid)
// {
//     Console.WriteLine("Blockchain is valid.");
// }
// else
// {
//     Console.WriteLine("Blockchain is not valid.");
// }

// Console.ReadLine();

// blockChainService.Chain[2].Data = "Bob send Charlie 1000";
// displayService.DisplayChain(blockChainService);

// isValid = blockChainService.IsChainValid();
// if (isValid)
// {
//     Console.WriteLine("Blockchain is valid.");
// }
// else
// {
//     Console.WriteLine("Blockchain is not valid.");
// }

var blockChainService = new BlockChain(4);
var displayService = new DisplayService();
var stopwatch = new System.Diagnostics.Stopwatch();
var hashingService = new HashingService();
var miningService = new MiningService(hashingService);

var block1 = new Block
{
    Index = 1,
    Data = "Test",
    PrevHash = "0",
    TimeStamp = DateTime.Now,
    Author = "Danil"
};

var block2 = new Block
{
    Index = block1.Index,
    Data = block1.Data,
    PrevHash = block1.PrevHash,
    TimeStamp = block1.TimeStamp,
    Author = block1.Author
};

// Сравнение производительности майнинга
// Обычный майнинг
stopwatch.Restart();

miningService.MineBlock(block1, 4);

stopwatch.Stop();
Console.WriteLine($"Обычный майнинг: {stopwatch.ElapsedMilliseconds} мс");

// Асинхронный майнинг
stopwatch.Restart();

await miningService.MineBlockAsync(block2, 4);

stopwatch.Stop();
Console.WriteLine($"Многопоточный майнинг: {stopwatch.ElapsedMilliseconds} мс");

// displayService.DisplayChain(blockChainService);
// Console.WriteLine(new string('-', 30));



