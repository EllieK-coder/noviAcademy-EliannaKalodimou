using WorldRank;

public class InsufficientFundsException : WalletException
{
    public decimal Requested { get; set; }
    public decimal Available { get; set; }

    public InsufficientFundsException(decimal requested, decimal available)
        : base($"Cannot withdraw {requested}; balance is only {available}.")
    {
        Requested = requested;
        Available = available;
    }
}
