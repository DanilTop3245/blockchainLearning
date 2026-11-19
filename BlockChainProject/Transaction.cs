namespace BlockChainITStep;

public class Transaction
{
    public string Id { get; set; } = string.Empty;

    public string From { get; set; } = string.Empty;

    public string To { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime Timestamp { get; set; }

    public Transaction(string from, string to, decimal amount)
    {
        Id = Guid.NewGuid().ToString();
        From = from;
        To = to;
        Amount = amount;
        Timestamp = DateTime.Now;
    }

    public string ToRawString()
    {
        return $"{Id}-{From}-{To}-{Amount}-{Timestamp}";
    }

    public override string ToString()
    {
        return $"Transaction ID: {Id}, From: {From}, To: {To}, Amount: {Amount}, Timestamp: {Timestamp}";
    }
}

