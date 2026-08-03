using System.Diagnostics.CodeAnalysis;
using MiniBank.Core;

public class CurrentAccount: Account{
    public required decimal OverdraftLimit{get; set;}       //default overdraft = 500$ 

    public CurrentAccount() { }

    [SetsRequiredMembers]
    public CurrentAccount(Customer customer, decimal balance = 0, decimal overDraftLimit = 500): base(customer, balance){
        OverdraftLimit = overDraftLimit; 
    }

    public override void Withdraw(decimal amount) {
        if (amount <= 0)
            throw new InvalidAmountException("Amount must be greater than zero.");
        
        if (Balance + OverdraftLimit < amount) 
            throw new InsufficientFundsException($"Can't withdraw {amount}$, Not enough Balance !!!");

        
        Balance -= amount ;
    }

}