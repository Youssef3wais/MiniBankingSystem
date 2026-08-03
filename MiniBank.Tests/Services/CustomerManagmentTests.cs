using MiniBank.Core;
using Moq;
using Xunit;
namespace MiniBank.Tests;

public class CustomerManagementTests {
    private readonly Mock<IAccountRepository> _mockAccountRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly BankServices _bankServices;

    public CustomerManagementTests() {
        _mockAccountRepo = new Mock<IAccountRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _bankServices = new BankServices(_mockAccountRepo.Object, _mockCustomerRepo.Object);
    }

    [Fact]
    public void CreateCustomer_ValidData_ReturnsCustomerAndAddsToRepository() {
        // Arrange
        string fullName = "Yusef";
        string email = "youssefewais@gmail.com";
        string phone = "01158xx";

        // Act
        var result = _bankServices.CreateCustomer(fullName, email, phone);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fullName, result.FullName);
        Assert.Equal(email, result.Email);
        Assert.Equal(phone, result.PhoneNumber);

        // Verify the customer was added to the repository
        _mockCustomerRepo.Verify(r => r.Add(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public void GetCustomerAccounts_ValidCustomerId_ReturnsMatchingAccounts() {
        // Arrange
        var customer = new Customer("Yusef", "youssefewais@email.com", "01158110");
        var customerId = customer.Id;

        var expectedAccounts = new List<Account>{
            new SavingsAccount(customer, balance: 500),
            new SavingsAccount(customer, balance: 1200),
            new CurrentAccount(customer, 900)
        };

        _mockCustomerRepo
            .Setup(r => r.GetByCustomerId(customerId))
            .Returns(customer);

        // Setup the mock repository to return our expected list when called with customerId
        _mockAccountRepo
            .Setup(r => r.GetByCustomerId(customerId))
            .Returns(expectedAccounts);

        // Act
        var actualAccounts = _bankServices.GetCustomerAccounts(customerId);

        // Assert
        Assert.NotNull(actualAccounts);
        Assert.Equal(expectedAccounts.Count, actualAccounts.Count());
        Assert.Equal(expectedAccounts, actualAccounts);

        // Verify that the repository was called exactly once with the customer ID
        _mockAccountRepo.Verify(r => r.GetByCustomerId(customerId), Times.Once);
    }


}