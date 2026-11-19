namespace BlockChainITStep;

public class Block
{
    // Номер блока в цепочке
    public int Index { get; set; }

    // Хеш текущего блока
    public string Hash { get; set; } = string.Empty;

    public string Data { get; set; } = string.Empty;

    // Данные блока
    // список транзакций, которые будут храниться в блоке
    public List<Transaction> Transactions { get; set; } = new();

    // Хеш предыдущего блока
    public string PrevHash { get; set; } = string.Empty;

    public DateTime TimeStamp { get; set; }

    public string Author { get; set; } = string.Empty;

    // Число, используемое для доказательства работы (Proof of Work)
    public long Nonce { get; set; } 

    public decimal MiningDuration { get; set; } 
    // Время, затраченное на майнинг блока в миллисекундах
    public int DifficultyAtMining { get; set; } 
    // Сложность майнинга на момент создания блока
    public long Attempts { get; set; } // Количество попыток майнинга блока

}