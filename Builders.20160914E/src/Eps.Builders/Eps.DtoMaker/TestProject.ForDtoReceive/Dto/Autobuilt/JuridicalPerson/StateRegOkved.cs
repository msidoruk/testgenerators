using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.StateRegOkveds'
	public class StateRegOkved
	{
		// Properties
		// 'StateRegOkveds.StateRegOkvedId'
		public int StateRegOkvedId { get; set; }
		// 'StateRegOkveds.StateRegistrationId'
		public int? StateRegistrationId { get; set; }
		// 'StateRegOkveds.Okved'
		public string Okved { get; set; }
		// Links
		// 'StateRegOkveds.StateRegistrationId' --> 'Jp.StateRegistrations'
		public JuridicalPerson.StateRegistration StateRegistration { get; set; }
	}
}

