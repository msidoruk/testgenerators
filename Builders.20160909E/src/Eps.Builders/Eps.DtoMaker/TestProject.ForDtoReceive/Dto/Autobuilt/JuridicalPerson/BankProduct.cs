using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.BankProducts'
	public class BankProduct
	{
		// Properties
		// 'BankProducts.BankProductId'
		public int BankProductId { get; set; }
		// 'BankProducts.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// Links
		// 'BankProducts.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
		// 'BankProducts.BankProductId' --> 'Dicts.BankProducts'
		public Dicts.BankProduct BankProduct_ { get; set; }
	}
}

