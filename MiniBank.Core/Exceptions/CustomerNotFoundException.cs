public class CustomerNotFoundException : BankingException
{
    public CustomerNotFoundException(string message = "Customer NotFound") : base(message) { }
}