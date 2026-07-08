using NLog;

namespace WorldRank;

public class Wallet
{
    public enum Currency
    {
        EUR,
        USD,
        GBP
    }

    public decimal Balance { get; private set; }
    public Currency WalletCurrency { get; private set; }
    public bool IsBlocked;

    public Wallet(decimal balance, Currency currency, bool isBlocked)
    {
        Balance = balance;
        WalletCurrency = currency;
        IsBlocked = isBlocked;
    }
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


    public Wallet(Currency currency)
    {
        WalletCurrency = currency;
    }

    public void SetBalance(decimal balance)
    {
        if (balance < 0)
        {
            return;
        }
        Balance = balance;
    }

    public override string ToString() =>
        $"Balance -> {Balance} Currency -> {WalletCurrency} IsBlocked -> {IsBlocked}";

    internal void Withdraw(decimal amount) => throw new NotImplementedException();

    public class WalletBlockedException : WalletException
    {
        public WalletBlockedException()
            : base("Operation not allowed: the wallet is blocked.")
        {
        }

        public WalletBlockedException(string message)
            : base(message)
        {
        }

        public class CurrencyMismatchException : WalletException
        {
            public Wallet.Currency Expected { get; }
            public Wallet.Currency Actual { get; }

            public CurrencyMismatchException(Wallet.Currency expected, Wallet.Currency actual)
                : base($"Currency mismatch: expected {expected}, got {actual}.")
            {
                Expected = expected;
                Actual = actual;
            }


        }
    }
}