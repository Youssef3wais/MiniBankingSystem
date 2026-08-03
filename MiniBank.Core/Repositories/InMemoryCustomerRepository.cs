
public class InMemoryCustomerRepository: ICustomerRepository {
    private readonly Dictionary<int, Customer> _customers = new Dictionary<int, Customer>();

    public void Add(Customer customer) {
        _customers.Add(customer.Id, customer);
    }

    public IEnumerable<Customer> GetAll() {
        return _customers.Values;
    }

    public Customer? GetByCustomerId(int customerId) {
        return _customers.GetValueOrDefault(customerId);
    }

}