using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace MiniBank.Core;

public abstract class Account {
    private static int _nextAccountNumber = 100;
    public required int AccountNumber { get; init; }
    public required Customer Customer { get; init; }
    public decimal Balance { get; protected set; }
    public required DateTime CreateDate { get; init; }

    private readonly List<Transaction> _transactions = new List<Transaction>();

    [JsonInclude]
    public IReadOnlyCollection<Transaction> Transactions {
        get => _transactions.AsReadOnly();
        init {
            _transactions.Clear();
            if (value != null) _transactions.AddRange(value);
        }
    }
    public Account() {
    }

    [SetsRequiredMembers]
    public Account(Customer customer, decimal balance = 0) {
        AccountNumber = Interlocked.Increment(ref _nextAccountNumber);
        Customer = customer;
        Balance = balance;
        CreateDate = DateTime.Now ;
        Transaction transaction = new Transaction(AccountNumber, TransactionType.Deposit, balance, $"Account is created with {balance}");
        AddTransaction(transaction);
    }
    public void AddTransaction(Transaction transaction) {
        _transactions.Add(transaction);
    }

    public void Deposit(decimal amount) {
        if (amount <= 0)
            throw new InvalidAmountException("Amount must be greater than zero.");

        Balance += amount;
    }
    public virtual void Withdraw(decimal amount) {
        if (amount <= 0)
            throw new InvalidAmountException("Amount must be greater than zero.");
        
        if (Balance < amount) 
            throw new InsufficientFundsException($"Current amount is {Balance}$, Can't withdraw {amount}$ !!!");

        Balance -= amount ;
    }

    
    public IReadOnlyCollection<Transaction> GetTransactions() {
        return Transactions ;
    }




}
