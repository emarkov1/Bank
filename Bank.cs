using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

interface IAccount
{
	bool put(int sum);
	bool take(int sum);
	int number { get; }
	int balance { get; }
	Account.AccountType type { get; }
	DateTime since { get; }
	string own { get; }
}

interface IBank
{
	IAccount lookupAccount(int number);
	IAccount lookupAccount(Account.AccountType type, string own);
}

[Serializable]
class Account : IAccount
{
	public enum AccountType
	{
		Ruble,
		Currency
	};

	int number_;
	int balance_;
	AccountType type_;
	DateTime since_;
	string own_;

	public Account(int number, int balance, AccountType type,
			DateTime since, string own)
	{
		number_ = number;
		balance_ = balance;
		type_ = type;
		since_ = since;
		own_ = own;
	}
	~Account()
	{
	}

	public bool put(int sum)
	{
		balance_ += sum;
		return true;
	}
	public bool take(int sum)
	{
		if (sum > balance_) {
			return false;
		}
		sum -= balance_;
		return true;
	}
	public int number
	{
		get
		{
			return number_;
		}
	}
	public int balance
	{
		get
		{
			return balance_;
		}
	}
	public AccountType type
	{
		get
		{
			return type_;
		}
	}
	public DateTime since
	{
		get
		{
			return since_;
		}
	}
	public string own
	{
		get
		{
			return own_;
		}
	}
}

[Serializable]
class AccountRuble : Account
{
	public AccountRuble(int number, int balance, DateTime since,
			string own)
		: base(number, balance, Account.AccountType.Ruble, since, own)
	{
	}
	~AccountRuble()
	{
	}
}

[Serializable]
class AccountCurrency : Account
{
	public AccountCurrency(int number, int balance, DateTime since,
			string own)
		: base(number, balance, Account.AccountType.Currency, since, own)
	{
	}
	~AccountCurrency()
	{
	}
}

class Bank : IBank
{
	const string accounts_file = "accounts.bank";
	ArrayList accounts_;
	Dictionary<Account.AccountType, int> rate_;

	public Bank()
	{
		loadsAccounts();
		rate_ = new Dictionary<Account.AccountType, int>();
		rate_[Account.AccountType.Ruble] = 13;
		rate_[Account.AccountType.Currency] = 11;
	}
	~Bank()
	{
		dumpsAccounts();
		/* accounts_ = null; */
	}

	void loadsAccounts()
	{
		FileStream fs;
		try {
			fs = new FileStream(Bank.accounts_file, FileMode.Open,
				FileAccess.Read);
		}
		catch (FileNotFoundException) {
			accounts_ = new ArrayList();
			return;
		}
		BinaryFormatter bf = new BinaryFormatter();
		accounts_ = (ArrayList)bf.Deserialize(fs);
		fs.Close();
	}
	void dumpsAccounts()
	{
		FileStream fs = new FileStream(Bank.accounts_file, FileMode.Create,
				FileAccess.Write);
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(fs, accounts_);
		fs.Close();
	}
	public IAccount lookupAccount(int number)
	{
		foreach (Account acc in accounts_) {
			if (number == acc.number) {
				return acc;
			}
		}
		return null;
	}
	public IAccount lookupAccount(Account.AccountType type, string own)
	{
		foreach (Account acc in accounts_) {
			if ((type == acc.type)
					&& (own.Equals(acc.own))) {
				return acc;
			}
		}
		return null;
	}
}

class Program
{
	static void Main(string[] args)
	{
		Bank bank = new Bank();
	}
}
