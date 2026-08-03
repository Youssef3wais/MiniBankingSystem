using System.Diagnostics.CodeAnalysis;
using MiniBank.Core;

public class SavingsAccount: Account{

    public SavingsAccount(Customer customer) {
        Customer = customer;
    }

    [SetsRequiredMembers]
    public SavingsAccount(Customer customer, decimal balance = 0): base(customer, balance){
    }

}