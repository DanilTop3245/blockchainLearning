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

var blockChainService = new BlockChain(4);
var displayService = new DisplayService();

blockChainService.AddBlock("Register user: Alice");

if (blockChainService.IsChainValid())
{
    Console.WriteLine("The blockchain is valid.");
}
else
{
    Console.WriteLine("The blockchain is not valid.");
}

displayService.DisplayChain(blockChainService);
Console.WriteLine(new string('-', 30));
// blockChainService.SearchBlockByIndex(2);
// blockChainService.SearchEventByText("PS5");