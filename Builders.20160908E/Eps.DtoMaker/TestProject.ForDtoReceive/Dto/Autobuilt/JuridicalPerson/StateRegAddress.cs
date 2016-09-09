using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.StateRegAddresses'
	public class StateRegAddress
	{
		// Properties
		// 'StateRegAddresses.StateRegAddressId'
		public int StateRegAddressId { get; set; }
		// 'StateRegAddresses.StateRegistrationId'
		public int? StateRegistrationId { get; set; }
		// 'StateRegAddresses.JuridicalPersonAddressTypeId'
		public int? JuridicalPersonAddressTypeId { get; set; }
		// 'StateRegAddresses.AddressValue'
		public string AddressValue { get; set; }
		// Links
		// 'StateRegAddresses.StateRegistrationId' --> 'Jp.StateRegistrations'
		public JuridicalPerson.StateRegistration StateRegistration { get; set; }
		// 'StateRegAddresses.JuridicalPersonAddressTypeId' --> 'Dicts.JuridicalPersonAddressTypes'
		public Dicts.JuridicalPersonAddressType JuridicalPersonAddressType { get; set; }
	}
}

