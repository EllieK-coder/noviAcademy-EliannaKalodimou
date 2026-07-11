using WorldRank.Domain.Entities;

namespace WorldRank.Application.Strategies;

/// <summary>Subtracts funds; the Wallet guards against going below zero.</summary>
public class SubtractFundsStrategy : IFundsStrategy
{
    public FundsOperation Operation => FundsOperation.Subtract;

    public void Execute(Wallet wallet, decimal amount) => wallet.Withdraw(amount);
}
