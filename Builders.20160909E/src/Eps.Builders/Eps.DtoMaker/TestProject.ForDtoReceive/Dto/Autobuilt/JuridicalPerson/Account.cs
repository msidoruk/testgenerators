using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.Accounts'
	public class Account
	{
		// Properties
		// 'Accounts.AccountId'
		public int AccountId { get; set; }
		// 'Accounts.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// 'Accounts.JuridicalPersonAccountTypeId'
		public int? JuridicalPersonAccountTypeId { get; set; }
		// 'Accounts.Bik'
		public string Bik { get; set; }
		// 'Accounts.AccountNumber'
		public string AccountNumber { get; set; }
		// 'Accounts.Description'
		public string Description { get; set; }
		// Links
		// 'Accounts.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
		// 'Accounts.JuridicalPersonAccountTypeId' --> 'Dicts.JuridicalPersonAccountTypes'
		public Dicts.JuridicalPersonAccountType JuridicalPersonAccountType { get; set; }
	}
}

