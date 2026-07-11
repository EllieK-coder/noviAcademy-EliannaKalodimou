using WorldRank.Domain.Entities;

namespace WorldRank.Application.Strategies;

/// <summary>Subtracts funds even if the balance goes negative - chargebacks, penalties.</summary>
public class ForceSubtractFundsStrategy : IFundsStrategy
{
    public FundsOperation Operation => FundsOperation.ForceSubtract;

    public void Execute(Wallet wallet, decimal amount) => wallet.ForceSubtractFunds(amount);
}
