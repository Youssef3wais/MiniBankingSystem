public class InsufficientFundsException : BankingException
{
    public InsufficientFundsException(string message = "Insufficient funds for this operation.") : base(message) { }
}