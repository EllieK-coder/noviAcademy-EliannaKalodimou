using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Domain.Entities;

public class Wallet : IWallet
{
    public Currency Currency { get; }
    public int PlayerId { get; }
    public decimal Balance { get; private set; }
    public bool IsBlocked { get; private set; }

    // EF Core materialisation only; app code uses the public constructor below.
    private Wallet() { }

    public Wallet(int playerId, Currency currency, decimal balance, bool isBlocked = false)
    {
        if (balance < 0)
            throw new InsufficientFundsException(balance);

        PlayerId = playerId;
        Balance = balance;
        Currency = currency;
        IsBlocked = isBlocked;
    }

    public void Block() => IsBlocked = true;

    public void Unblock() => IsBlocked = false;

    public void SetBalance(decimal balance)
    {
        if (balance < 0)
            throw new InsufficientFundsException(balance);

        Balance = balance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidAmountException(amount);

        if (IsBlocked)
            throw new WalletBlockedException(Currency);

        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidAmountException(amount);

        if (IsBlocked)
            throw new WalletBlockedException(Currency);

        var newBalance = Balance - amount;
        if (newBalance < 0)
            throw new InsufficientFundsException(newBalance);

        Balance = newBalance;
    }

    public void ForceSubtractFunds(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidAmountException(amount);

        Balance -= amount;
    }

    public override string ToString() => $"Balance -> {Balance} Currency -> {Currency} IsBlocked -> {IsBlocked}";
}