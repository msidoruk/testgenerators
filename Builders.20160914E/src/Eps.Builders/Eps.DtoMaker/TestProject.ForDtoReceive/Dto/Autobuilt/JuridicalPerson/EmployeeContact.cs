using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.EmployeeContacts'
	public class EmployeeContact
	{
		// Properties
		// 'EmployeeContacts.EmployeeContactId'
		public int EmployeeContactId { get; set; }
		// 'EmployeeContacts.EmployeeId'
		public int? EmployeeId { get; set; }
		// 'EmployeeContacts.EmployeeContactTypeId'
		public int? EmployeeContactTypeId { get; set; }
		// 'EmployeeContacts.ContactValue'
		public string ContactValue { get; set; }
		// Links
		// 'EmployeeContacts.EmployeeId' --> 'Jp.Employees'
		public JuridicalPerson.Employee Employee { get; set; }
		// 'EmployeeContacts.EmployeeContactTypeId' --> 'Dicts.EmployeeContactTypes'
		public Dicts.EmployeeContactType EmployeeContactType { get; set; }
	}
}

