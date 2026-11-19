using BlockChainITStep;

namespace BlockChainITStep;

public class TransactionService
{
    public Transaction CreateTransaction(string sender, string receiver, decimal amount)
    {
        var transaction = new Transaction(sender, receiver, amount);
        var validationResult = ValidateTransaction(transaction);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException(validationResult.ErrorMessage);
        }

        return transaction;
    }

    public (bool IsValid, string ErrorMessage) ValidateTransaction(
        Transaction transaction)
    {
        if (string.IsNullOrWhiteSpace(transaction.From))
        {
            return (false, "Sender address is required.");
        }

        if (string.IsNullOrWhiteSpace(transaction.To))
        {
            return (false, "Recipient address is required.");
        }

        if (transaction.Amount <= 0)
        {
            return (false, "Transaction amount must be greater than zero.");
        }

        return (true, string.Empty);
    }
}