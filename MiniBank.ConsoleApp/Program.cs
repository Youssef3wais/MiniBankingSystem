using System;

namespace MiniBank.ConsoleUI;

public class Program {

    private static void Main(string[] args) {
        // Setup
        IAccountRepository accountRepo = new InMemoryAccountRepository();
        ICustomerRepository customerRepo = new InMemoryCustomerRepository();
        IBankServices _bankServices = new BankServices(accountRepo, customerRepo);

        bool running = true;
        while (running) {
            Console.WriteLine("==========================\n   Mini Banking System\n==========================");
            Console.WriteLine("1. Create Customer\n2. Create Account\n3. Deposit\n4. Withdraw\n5. Transfer\n6. View Transaction History\n7. View All Customers\n8. View Customer Accounts\n9. Exit");
            Console.Write("\nSelect an option (1-9): ");

            string? choice = Console.ReadLine()?.Trim();

            Console.Clear();
            try {
                switch (choice) {
                    case "1":
                        CreateCustomer(_bankServices);
                        break;
                    case "2":
                        CreateAccount(_bankServices);
                        break;
                    case "3":
                        Deposit(_bankServices);
                        break;
                    case "4":
                        Withdraw(_bankServices);
                        break;
                    case "5":
                        Transfer(_bankServices);
                        break;
                    case "6":
                        ViewTransactionHistory(_bankServices);
                        break;
                    case "7":
                        ViewAllCustomers(customerRepo);
                        break;
                    case "8":
                        ViewCustomerAccounts(_bankServices);
                        break;
                    case "9":
                        running = false;
                        Console.WriteLine("Bye!!!");
                        break;
                    default:
                        Console.WriteLine("Invalid menu selection. Please enter a number from 1 to 9.");
                        break;
                }
            } catch (BankingException ex) {
                Console.WriteLine($"\nBank Error: {ex.Message}");
            } catch (FormatException) {
                Console.WriteLine("\nInput Error: Invalid number format entered.");
            }

            if (running) {
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        Console.ReadKey();
    }

    private static void CreateCustomer(IBankServices bankServices) {
        Console.WriteLine("------ Creating Customer ------");
        Console.Write("Enter Full Name: ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Enter Email: ");
        string email = Console.ReadLine() ?? "";
        Console.Write("Enter Phone Number: ");
        string phone = Console.ReadLine() ?? "";

        var customer = bankServices.CreateCustomer(name, email, phone);
        Console.WriteLine($"\nCustomer Created! ID: {customer.Id}");
    }

    private static void CreateAccount(IBankServices bankServices) {
        Console.WriteLine("------ Creating Account ------");
        Console.Write("Enter Customer ID: ");
        int customerId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Select Type (1: Savings, 2: Current): ");
        int typeChoice = int.Parse(Console.ReadLine() ?? "1");
        AccountType type = typeChoice == 2 ? AccountType.Current : AccountType.Savings;

        Console.Write("Enter Initial Deposit: ");
        decimal deposit = decimal.Parse(Console.ReadLine() ?? "0");

        var account = bankServices.CreateAccount(customerId, type, deposit);
        Console.WriteLine($"\nAccount #{account.AccountNumber} Created! Balance: ${account.Balance}");
    }

    private static void Deposit(IBankServices bankServices) {
        Console.WriteLine("------ Deposit ------");
        Console.Write("Enter Account Number: ");
        int accNo = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Enter Amount: ");
        decimal amount = decimal.Parse(Console.ReadLine() ?? "0");

        bankServices.Deposit(accNo, amount);
        Console.WriteLine("\nDeposit Successful!");
    }

    private static void Withdraw(IBankServices bankServices) {
        Console.WriteLine("------ Withdraw ------");
        Console.Write("Enter Account Number: ");
        int accNo = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Enter Amount: ");
        decimal amount = decimal.Parse(Console.ReadLine() ?? "0");

        bankServices.Withdraw(accNo, amount);
        Console.WriteLine("\nWithdrawal Successful!");
    }

    private static void Transfer(IBankServices bankServices) {
        Console.WriteLine("------ Transfer ------");
        Console.Write("Enter Source Account Number: ");
        int fromAcc = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Enter Destination Account Number: ");
        int toAcc = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Enter Amount: ");
        decimal amount = decimal.Parse(Console.ReadLine() ?? "0");

        bankServices.Transfer(fromAcc, toAcc, amount);
        Console.WriteLine("\nTransfer Successful!");
    }

    private static void ViewTransactionHistory(IBankServices bankServices) {
        Console.WriteLine("------ View Transaction History ------");
        Console.Write("Enter Account Number: ");
        int accNo = int.Parse(Console.ReadLine() ?? "0");

        var transactions = bankServices.GetTransactionHistory(accNo);
        Console.WriteLine($"\n--- Transactions for Account #{accNo} ---");
        foreach (var tx in transactions) {
            Console.WriteLine($"[{tx.TimeStamp:yyyy-MM-dd HH:mm}] {tx.Type}: ${tx.Amount} - {tx.Description}");
        }
    }

    private static void ViewAllCustomers(ICustomerRepository customerRepo) {
        Console.WriteLine("------ All Registered Customers ------");
        var customers = customerRepo.GetAll();

        if (!customers.Any()) {
            Console.WriteLine("No customers found in the system.");
            return;
        }

        foreach (var c in customers) {
            Console.WriteLine($"[ID: {c.Id}] Name: {c.FullName} | Email: {c.Email} | Phone: {c.PhoneNumber}");
        }
    }

    private static void ViewCustomerAccounts(IBankServices bankServices) {
        Console.WriteLine("------ Customer Accounts ------");
        Console.Write("Enter Customer ID: ");
        int customerId = int.Parse(Console.ReadLine() ?? "0");

        var accounts = bankServices.GetCustomerAccounts(customerId);

        if (!accounts.Any()) {
            Console.WriteLine($"No accounts found for Customer ID #{customerId}.");
            return;
        }

        Console.WriteLine($"\n--- Accounts for Customer #{customerId} ---");
        foreach (var acc in accounts) {
            Console.WriteLine($"Account #{acc.AccountNumber} | Type: {acc.GetType} | Balance: ${acc.Balance}");
        }
    }
}