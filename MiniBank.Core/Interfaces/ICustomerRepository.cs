
public interface ICustomerRepository {
    void Add(Customer customer);
    Customer? GetByCustomerId(int id);

    IEnumerable<Customer> GetAll();

}