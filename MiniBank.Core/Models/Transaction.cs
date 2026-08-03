
using System.Diagnostics.CodeAnalysis;

public class Transaction {
    private static int _lastId = 0 ;
    public required int Id{get; init;}
    public required int AccountNumber{ get; init;}
    public required TransactionType Type{get;init;}
    public required decimal Amount{get; init;}
    public required DateTime TimeStamp{ get; init;}
    public string? Description{get; set;}


    public Transaction() {
    }

    
    [SetsRequiredMembers]
    public Transaction(int accountNumber, TransactionType type, decimal amount, string? description = null ) {
        Id = Interlocked.Increment(ref _lastId);
        AccountNumber = accountNumber ;
        Type = type ;
        Amount = amount ;
        TimeStamp = DateTime.Now ;
        Description = description ;
    }
}