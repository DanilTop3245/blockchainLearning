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
var transactionService = new TransactionService();
var pendingTransactions = new List<Transaction>();

while (true)
{
    Console.WriteLine("Select an option:");
    Console.WriteLine("1. Mine Block");
    Console.WriteLine("2. Add Transaction");
    Console.WriteLine("3. Display Chain");
    Console.WriteLine("4. Validate Chain");
    Console.WriteLine("5. Add 20 Blocks");
    Console.WriteLine("6. Exit");
    Console.WriteLine("7. Print Difficulty History");
    Console.WriteLine("8. Find First Invalid Block");
    Console.WriteLine("9. Find Transaction by Id");
    Console.WriteLine("10. Find Transactions by User");
    Console.WriteLine("11. Create Demo Chain (4 Blocks, 12 Transactions)");
    Console.WriteLine("12. Run Blockchain Events Demo");
    Console.WriteLine("13. Search Block by Index");
    Console.WriteLine("14. Search Events by Text");
    Console.WriteLine("15. Change Second Block Data");

    var option = Console.ReadLine();
    switch (option)
    {
        case "1":
            await blockChainService.AddBlock(pendingTransactions);
            pendingTransactions.Clear();
            break;
        case "2":
            Console.Write("Sender: ");
            var sender = Console.ReadLine() ?? string.Empty;
            Console.Write("Recipient: ");
            var receiver = Console.ReadLine() ?? string.Empty;
            Console.Write("Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount))
            {
                Console.WriteLine("Amount must be a number.");
                break;
            }

            try
            {
                pendingTransactions.Add(
                    transactionService.CreateTransaction(sender, receiver, amount));
                Console.WriteLine("Transaction added.");
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception.Message);
            }
            break;
        case "3":
            displayService.DisplayChain(blockChainService);
            break;
        case "4":
            var isValid = blockChainService.IsChainValid();
            Console.WriteLine($"Is blockchain valid? {isValid}");
            break;
        case "5":
            for (var i = 0; i < 20; i++)
            {
                await blockChainService.AddBlock(pendingTransactions);
                pendingTransactions.Clear();
            }
            break;
        case "6":
            return;
        case "7":
            blockChainService.PrintDifficultyHistory();
            break;
        case "8":
            blockChainService.FindFirstInvalidBlock();
            break;
        case "9":
            Console.Write("Transaction Id: ");
            var transactionId = Console.ReadLine() ?? string.Empty;
            blockChainService.FindTransactionById(transactionId);
            break;
        case "10":
            Console.Write("User: ");
            var user = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("1. All");
            Console.WriteLine("2. Sent");
            Console.WriteLine("3. Received");
            var filter = Console.ReadLine();
            var searchMode = filter switch
            {
                "2" => TransactionSearchMode.Sent,
                "3" => TransactionSearchMode.Received,
                _ => TransactionSearchMode.All
            };
            blockChainService.FindTransactionsByUser(user, searchMode);
            break;
        case "11":
            await blockChainService.CreateDemoChainAsync();
            Console.WriteLine("Demo chain created.");
            break;
        case "12":
            var events = new[]
            {
                "User Danil registered",
                "User Danil purchased a laptop",
                "Danil transferred 100 coins to Alice",
                "Alice received the transfer",
                "Order was delivered"
            };

            foreach (var eventData in events)
            {
                await blockChainService.AddEventBlock(eventData);
            }

            Console.WriteLine("Five event blocks created.");
            displayService.DisplayChain(blockChainService);
            Console.WriteLine($"Blockchain is valid? {blockChainService.IsValid()}");

            Console.WriteLine("Search result for 'transfer':");
            blockChainService.SearchEventByText("transfer");

            blockChainService.Chain[2].Data = "Danil transferred 1000 coins to Alice";
            Console.WriteLine("Second event block data was changed.");
            Console.WriteLine($"Blockchain is valid? {blockChainService.IsValid()}");
            blockChainService.FindFirstInvalidBlock();
            break;
        case "13":
            Console.Write("Block Index: ");
            if (int.TryParse(Console.ReadLine(), out var blockIndex))
            {
                blockChainService.SearchBlockByIndex(blockIndex);
            }
            else
            {
                Console.WriteLine("Index must be a number.");
            }
            break;
        case "14":
            Console.Write("Event text: ");
            blockChainService.SearchEventByText(Console.ReadLine() ?? string.Empty);
            break;
        case "15":
            if (blockChainService.Chain.Count > 2)
            {
                blockChainService.Chain[2].Data = "Changed event data";
                Console.WriteLine("Second block data changed.");
                Console.WriteLine($"Blockchain is valid? {blockChainService.IsValid()}");
                blockChainService.FindFirstInvalidBlock();
            }
            else
            {
                Console.WriteLine("Create event blocks first.");
            }
            break;
        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}





