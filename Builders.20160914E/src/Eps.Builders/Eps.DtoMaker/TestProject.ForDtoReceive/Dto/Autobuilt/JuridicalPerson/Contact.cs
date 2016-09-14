using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.Contacts'
	public class Contact
	{
		// Properties
		// 'Contacts.ContactId'
		public int ContactId { get; set; }
		// 'Contacts.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// 'Contacts.JuridicalPersonContactTypeId'
		public int? JuridicalPersonContactTypeId { get; set; }
		// 'Contacts.ContactValue'
		public string ContactValue { get; set; }
		// Links
		// 'Contacts.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
		// 'Contacts.JuridicalPersonContactTypeId' --> 'Dicts.JuridicalPersonContactTypes'
		public Dicts.JuridicalPersonContactType JuridicalPersonContactType { get; set; }
	}
}

