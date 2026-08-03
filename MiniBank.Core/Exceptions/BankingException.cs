// Base domain exception for your system
public class BankingException : Exception
{
    public BankingException(string message) : base(message) { }
}