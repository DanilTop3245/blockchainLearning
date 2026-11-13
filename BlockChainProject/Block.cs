using System.Security.Cryptography;
using System.Text;

namespace BlockChainITStep;

public class Block
{
    // Номер блока в цепочке
    public int Index { get; set; }

    // Хеш текущего блока
    public string Hash { get; set; } = string.Empty;

    // Данные блока
    public string Data { get; set; } = string.Empty;

    // Хеш предыдущего блока
    public string PrevHash { get; set; } = string.Empty;

    public DateTime TimeStamp { get; set; }

    public string Author { get; set; } = string.Empty;

    // Число, используемое для доказательства работы (Proof of Work)
    public int Nonce { get; set; } 

}