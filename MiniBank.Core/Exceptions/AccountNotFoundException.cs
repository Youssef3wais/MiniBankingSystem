public class AccountNotFoundException : BankingException
{
    public AccountNotFoundException(int accountNumber) : base($"Account with number {accountNumber} was not found.") { }

    public AccountNotFoundException(string message) : base(message) { }

}