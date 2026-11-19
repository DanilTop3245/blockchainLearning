using System.Security.Cryptography;
using System.Text;

namespace BlockChainITStep;

public class Transaction
{
    public int Id { get; set; }

    public string Sender { get; set; } = string.Empty;

    public string Receiver { get; set; } = string.Empty;

    public decimal Amount { get; set; }

}