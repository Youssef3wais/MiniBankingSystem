# Mini Banking System (C# Console Application)

## Table of Contents
- [Overview](#overview)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Build & Run the Application](#build--run-the-application)
- [Running the Test Suite](#running-the-test-suite)
- [Design & Architecture](#design--architecture)
- [Key OOP Concepts Demonstrated](#key-oop-concepts-demonstrated)
- [Exception Handling](#exception-handling)
- [Data Persistence (Bonus)](#data-persistence-bonus)
- [Mocking with Moq (Bonus)](#mocking-with-moq-bonus)
- [Assumptions & Limitations](#assumptions--limitations)
- [Future Improvements](#future-improvements)

---

## Overview
The **Mini Banking System** is a console‑based C# application that showcases clean‑architecture principles, solid OOP design, LINQ usage, dependency injection, custom exception handling, and comprehensive unit testing with **xUnit** and **Moq**.

It supports:
- Customer creation and management
- Account creation (Savings & Current) with overdraft support for Current accounts
- Deposits, withdrawals, and transfers with transaction history
- In‑memory data storage (easily replaceable via DI)

---

## Project Structure
```
MiniBankingSystem/
│   MiniBankingSystem.slnx
│
├─ MiniBank.ConsoleApp/          # Console UI
│   └─ Program.cs
│
├─ MiniBank.Core/                # Domain, services, interfaces, enums, exceptions
│   ├─ Models/        (Customer, Account, SavingsAccount, CurrentAccount, Transaction)
│   ├─ Enums/         (AccountType, TransactionType)
│   ├─ Exceptions/    (custom exceptions)
│   ├─ Interfaces/   (IAccountRepository, ICustomerRepository, IBankServices)
│   ├─ Repositories/ (In‑memory implementations)
│   └─ Services/     (BankServices – business logic)
│
├─ MiniBank.Tests/                # xUnit test project
│   ├─ Services/      (TransferTests, AccountManagementTests, CustomerManagementTests)
│   ├─ Repositories/ (RepositoryTests)
│   └─ MiniBank.Tests.csproj
└─ README.md
```
---

## Prerequisites
- **.NET SDK 10.0** (or later) installed and available in `PATH`.
- **Git** (optional, for cloning the repo).
- **Moq** NuGet package is already referenced in the test project.

---

## Build & Run the Application
1. Open a terminal and navigate to the solution root:
   ```powershell
   cd C:\vsProjects\MiniBankingSystem
   ```
2. Restore packages and build:
   ```powershell
   dotnet restore
   dotnet build
   ```
3. Run the console UI:
   ```powershell
   dotnet run --project MiniBank.ConsoleApp
   ```
   Follow the on‑screen menu to create customers, accounts, perform deposits, withdrawals, transfers, and view transaction history.

---

## Running the Test Suite
The solution contains **36 passing tests** (well above the required 20).
```powershell
dotnet test MiniBank.Tests
```
The tests cover:
- Deposit behavior (including invalid amounts)
- Withdrawal (including overdraft handling for Current accounts)
- Transfer (multiple edge‑cases, using **Moq** to mock repositories)
- Account and Customer management

---

## Design & Architecture
- **Domain Layer** (`MiniBank.Core.Models`) contains pure POCOs with required members and encapsulated state.
- **Service Layer** (`BankServices`) orchestrates business rules and uses **DI** to depend on repository interfaces.
- **Repositories** are in‑memory (`Dictionary<int,T>`) but expose interfaces (`IAccountRepository`, `ICustomerRepository`) allowing future swap for persistent stores (e.g., JSON, EF Core).
- **Interfaces** define contracts for operations, supporting **unit testing** with mocking.
- **Enums** represent fixed sets (`AccountType`, `TransactionType`).
- **Exceptions** provide clear error messages for invalid operations.
- **LINQ** is used in repository queries (e.g., `GetByCustomerId`).

---

## Key OOP Concepts Demonstrated
| Concept | Implementation |
|---------|----------------|
| **Encapsulation** | Sensitive fields like `Balance` are modified only via methods (`Deposit`, `Withdraw`). |
| **Inheritance** | `SavingsAccount` and `CurrentAccount` inherit from abstract `Account`. |
| **Abstraction** | `Account` defines common behavior; concrete classes add specific rules. |
| **Polymorphism** | `CurrentAccount` overrides `Withdraw` to support overdraft. |
| **Interfaces** | `IBankServices`, repository interfaces enable loose coupling and testability. |

---

## Exception Handling
Custom exceptions (`InvalidAmountException`, `InsufficientFundsException`, `AccountNotFoundException`, etc.) are thrown from the domain/service layer and caught in the UI (`Program.cs`) to provide user‑friendly messages.

---

## Data Persistence (Bonus)
*Not implemented in the current version.*
A possible extension is a **JsonFileRepository** that serialises the dictionaries to JSON on application shutdown and reloads them on startup using `System.Text.Json`.

---

## Mocking with Moq (Bonus)
The test project already uses **Moq** to mock `IAccountRepository` and `ICustomerRepository` in `TransferTests.cs`. This isolates `BankServices` from concrete data stores, allowing verification of interaction patterns (e.g., `Update` called exactly once).

---

## Assumptions & Limitations
- All IDs are auto‑generated using a static counter – suitable for a demo but not thread‑safe for real‑world concurrency.
- In‑memory storage means data is lost when the program exits.
- Console UI is minimal; no validation beyond parsing exceptions.
- Overdraft limit is fixed at **$500** for `CurrentAccount` (configurable via constructor).

---

## Future Improvements
- Implement JSON persistence (bonus) or integrate a lightweight DB (e.g., SQLite).
- Add a logging framework for audit trails.
- Enhance UI with richer input validation and clearer prompts.
- Expand unit tests for edge‑cases like concurrent operations.
- Introduce a configuration system (e.g., appsettings.json) for overdraft limits, initial IDs, etc.

---

*Happy coding!*
