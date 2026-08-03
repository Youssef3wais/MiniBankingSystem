public class InvalidAmountException : BankingException
{
    public InvalidAmountException(string message = "Amount must be greater than zero.") : base(message) { }
}