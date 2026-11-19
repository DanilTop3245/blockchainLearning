using System.Security.Cryptography;
using System.Text;

namespace BlockChainITStep;

public class HashingService
{
    public string ComputeBlockHash(Block block)
    {
        // Формируем хеш данных блока для хеширования
        var blockData = $"{block.Index}{block.PrevHash}{block.TimeStamp}{block.Data}{block.Author}{block.Nonce}{block.DifficultyAtMining}";
        return ComputeHash(blockData);
    }

    private string ComputeHash(string data)
    {
        // В криптосумму SHA256 для обмена хешу
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}
