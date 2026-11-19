using BlockChainITStep;

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

// const int difficulty = 4;
// var blockChainService = new BlockChain(difficulty);
// var displayService = new DisplayService();
// var stopwatch = new System.Diagnostics.Stopwatch();
// var hashingService = new HashingService();
// var miningService = new MiningService(hashingService);

// var block1 = new Block
// {
//     Index = 1,
//     Data = "Test",
//     PrevHash = "0",
//     TimeStamp = DateTime.Now,
//     Author = "Danil"
// };

// var block2 = new Block
// {
//     Index = block1.Index,
//     Data = block1.Data,
//     PrevHash = block1.PrevHash,
//     TimeStamp = block1.TimeStamp,
//     Author = block1.Author
// };

// // Сравнение производительности майнинга
// // Обычный майнинг
// stopwatch.Restart();

// miningService.MineBlock(block1, difficulty);

// stopwatch.Stop();
// Console.WriteLine($"Обычный майнинг: {stopwatch.ElapsedMilliseconds} мс");

// // Асинхронный майнинг
// stopwatch.Restart();

// await miningService.MineBlockAsync(block2, difficulty);

// stopwatch.Stop();
// Console.WriteLine($"Многопоточный майнинг: {stopwatch.ElapsedMilliseconds} мс");



const int difficulty = 1;
var displayService = new DisplayService();
var blockChainService = new BlockChain(difficulty);

while (true)
{
    Console.WriteLine("Select an option:");
    Console.WriteLine("1. Add Block");
    Console.WriteLine("2. Display Chain");
    Console.WriteLine("3. Validate Chain");
    Console.WriteLine("4. Add 20 Blocks");
    Console.WriteLine("5. Add 8 Blocks");
    Console.WriteLine("6. Exit");
    Console.WriteLine("7. Print Difficulty History");
    Console.WriteLine("8. Change Data in Block 2");
    Console.WriteLine("9. Recalculate Hash of Block 2");
    Console.WriteLine("10. Find First Invalid Block");

    var option = Console.ReadLine();
    switch (option)
    {
        case "1":
            await blockChainService.AddBlock("Bob sends 100 coins to Alice");
            break;
        case "2":
            displayService.DisplayChain(blockChainService);
            break;
        case "3":
            var isValid = blockChainService.IsChainValid();
            Console.WriteLine($"Is blockchain valid? {isValid}");
            break;
        case "4":
            for (var i = 0; i < 20; i++)
            {
                await blockChainService.AddBlock($"Block {i + 1} data");
            }
            break;
        case "5":
            for (var i = 0; i < 8; i++)
            {
                await blockChainService.AddBlock($"Block {i + 1} data");
            }
            break;
        case "6":
            return;
        case "7":
            blockChainService.PrintDifficultyHistory();
            break;
        case "8":
            if (blockChainService.Chain.Count <= 2)
            {
                Console.WriteLine("Add at least 2 blocks first.");
                break;
            }

            blockChainService.Chain[2].Data = "Tampered block data";
            Console.WriteLine("Data in block 2 was changed.");
            break;
        case "9":
            if (blockChainService.Chain.Count <= 2)
            {
                Console.WriteLine("Add at least 2 blocks first.");
                break;
            }

            var hashingService = new HashingService();
            blockChainService.Chain[2].Hash =
                hashingService.ComputeBlockHash(blockChainService.Chain[2]);
            Console.WriteLine("Hash of block 2 was recalculated.");
            break;
        case "10":
            blockChainService.FindFirstInvalidBlock();
            break;
        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}





