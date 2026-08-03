using MiniBank.Core;
using Moq;
using Xunit;

namespace MiniBank.Tests;

public class TransferTests {
    private readonly Mock<IAccountRepository> _mockAccountRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly BankServices _bankServices;

    public TransferTests() {
        _mockAccountRepo = new Mock<IAccountRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _bankServices = new BankServices(_mockAccountRepo.Object, _mockCustomerRepo.Object);
    }

    [Fact]
    public void Transfer_ValidAccountsAndAmount_UpdatesBalancesAndRepositories() {
        // Arrange
        var senderCustomer = new Customer("xxx", "xxx@gmail.com", "011xxx");
        var receiverCustomer = new Customer("yyy", "yyy@gmail.com", "012yyy");

        var sourceAccount = new SavingsAccount(senderCustomer, balance: 500);
        var targetAccount = new SavingsAccount(receiverCustomer, balance: 200);

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(sourceAccount.AccountNumber))
            .Returns(sourceAccount);
        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(targetAccount.AccountNumber))
            .Returns(targetAccount);

        // Act
        _bankServices.Transfer(sourceAccount.AccountNumber, targetAccount.AccountNumber, 150);

        // Assert
        Assert.Equal(350, sourceAccount.Balance);
        Assert.Equal(350, targetAccount.Balance);


        // Verify Update was called exactly once for each account
        _mockAccountRepo.Verify(r => r.Update(sourceAccount), Times.Once);
        _mockAccountRepo.Verify(r => r.Update(targetAccount), Times.Once);
    }

    [Fact]
    public void Transfer_SameAccount_ThrowsSameAccountTransferException()
    {
        // Arrange
        int accountNumber = 1001;

        // Act & Assert
        Assert.Throws<SameAccountTransferException>(() =>
            _bankServices.Transfer(accountNumber, accountNumber, 50)
        );

        // Verify repository lookups and updates were never performed
        _mockAccountRepo.Verify(r => r.GetByAccountNumber(It.IsAny<int>()), Times.Never);
        _mockAccountRepo.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Transfer_InsufficientFundsInSource_ThrowsInsufficientFundsExceptionAndPreservesState() {
        // Arrange
        var senderCustomer = new Customer("xxx", "xxx@gmail.com", "011xxx");
        var receiverCustomer = new Customer("yyy", "yyy@gmail.com", "012yyy");

        var sourceAccount = new SavingsAccount(senderCustomer, balance: 500);
        var targetAccount = new SavingsAccount(receiverCustomer, balance: 200);

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(sourceAccount.AccountNumber))
            .Returns(sourceAccount);
        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(targetAccount.AccountNumber))
            .Returns(targetAccount);

        // Act
        Assert.Throws<InsufficientFundsException>(()=>_bankServices.Transfer(sourceAccount.AccountNumber, targetAccount.AccountNumber, 600));
        

        // Assert balances remained unchanged
        Assert.Equal(500, sourceAccount.Balance);
        Assert.Equal(200, targetAccount.Balance);

        // Verify no updates were saved to the repository
        _mockAccountRepo.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Transfer_SourceAccountNotFound_ThrowsAccountNotFoundException() {
        int invalidSource = 9999;
        var receiverCustomer = new Customer("yyy", "yyy@gmail.com", "012yyy");
        var targetAccount = new SavingsAccount(receiverCustomer, balance: 200);

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(invalidSource))
            .Returns((Account?)null);
        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(targetAccount.AccountNumber))
            .Returns(targetAccount);

        // Act & Assert
        Assert.Throws<AccountNotFoundException>(() =>
            _bankServices.Transfer(invalidSource, targetAccount.AccountNumber, 50)
        );

        _mockAccountRepo.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Transfer_DestinationAccountNotFound_ThrowsAccountNotFoundException() {
        int invalidSource = 9999;
        var senderCustomer = new Customer("xxx", "xxx@gmail.com", "011xxx");
        var sourceAccount = new SavingsAccount(senderCustomer, balance: 500);

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(sourceAccount.AccountNumber))
            .Returns(sourceAccount);
        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(invalidSource))
            .Returns((Account?)null);
        

        // Act & Assert
        Assert.Throws<AccountNotFoundException>(() =>
            _bankServices.Transfer(invalidSource, sourceAccount.AccountNumber, 50)
        );

        _mockAccountRepo.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }




}