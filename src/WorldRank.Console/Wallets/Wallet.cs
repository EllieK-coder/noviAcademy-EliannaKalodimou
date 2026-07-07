using WorldRank.Console.Enums;

namespace WorldRank.Console
{
	public class Wallet
	{
		public decimal Balance { get; private set; }
		public Currency Currency;
		public bool IsBlocked;

		public Wallet(decimal balance, Currency currency, bool isBlocked)
		{
			Balance = balance;
			Currency = currency;
			IsBlocked = false;
		}

		public void SetBalance(decimal balance)
		{
			if (balance < 0)
			{
				return;
			}
			Balance = balance;
		}

		public override string ToString()
		{
			return "Balance -> " + Balance + " Currency ->" + Currency + " IsBlocked -> " + IsBlocked;
		}
	}
}