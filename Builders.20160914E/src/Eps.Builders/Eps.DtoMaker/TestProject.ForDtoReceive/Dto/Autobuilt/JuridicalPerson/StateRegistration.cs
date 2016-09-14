using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.StateRegistrations'
	public class StateRegistration
	{
		// Properties
		// 'StateRegistrations.StateRegistrationId'
		public int StateRegistrationId { get; set; }
		// 'StateRegistrations.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// 'StateRegistrations.RegistrationDate'
		public System.DateTime? RegistrationDate { get; set; }
		// 'StateRegistrations.RegistrationPlace'
		public string RegistrationPlace { get; set; }
		// 'StateRegistrations.RegisteredByDept'
		public string RegisteredByDept { get; set; }
		// 'StateRegistrations.OGRN'
		public string OGRN { get; set; }
		// 'StateRegistrations.INN'
		public string INN { get; set; }
		// 'StateRegistrations.KPP'
		public string KPP { get; set; }
		// 'StateRegistrations.OKATO'
		public string OKATO { get; set; }
		// 'StateRegistrations.OKTMO'
		public string OKTMO { get; set; }
		// 'StateRegistrations.OKFS'
		public string OKFS { get; set; }
		// 'StateRegistrations.OKPO'
		public string OKPO { get; set; }
		// 'StateRegistrations.OKOPF'
		public string OKOPF { get; set; }
		// 'StateRegistrations.ActivationDate'
		public System.DateTime? ActivationDate { get; set; }
		// 'StateRegistrations.EndDate'
		public System.DateTime? EndDate { get; set; }
		// 'StateRegistrations.Description'
		public string Description { get; set; }
		// Links
		// 'StateRegistrations.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
	}
}

