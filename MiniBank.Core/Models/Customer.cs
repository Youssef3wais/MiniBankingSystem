
using System.Diagnostics.CodeAnalysis;

public class Customer {
    private static int _nextId = 100 ;
    public required int Id { get; init; }
    public required string FullName {get; set;}
    public required string Email {get; set;}
    public required string PhoneNumber {get; set;}

    Customer() {
    }

    [SetsRequiredMembers]
    public Customer(String fullName, string email, string phoneNumber) {
        Id = Interlocked.Increment(ref _nextId);
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public static void SetLastId(int id) {
        _nextId = id;
    }
}