public class SameAccountTransferException : BankingException
{
    public SameAccountTransferException() : base("Source and destination accounts cannot be the same.") { }
}