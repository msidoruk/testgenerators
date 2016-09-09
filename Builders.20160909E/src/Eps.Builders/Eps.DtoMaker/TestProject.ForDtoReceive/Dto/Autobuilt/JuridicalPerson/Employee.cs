using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.Employees'
	public class Employee
	{
		// Properties
		// 'Employees.EmployeeId'
		public int EmployeeId { get; set; }
		// 'Employees.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// 'Employees.Position'
		public string Position { get; set; }
		// 'Employees.FirstName'
		public string FirstName { get; set; }
		// 'Employees.LastName'
		public string LastName { get; set; }
		// 'Employees.MiddleName'
		public string MiddleName { get; set; }
		// 'Employees.Citizenship'
		public string Citizenship { get; set; }
		// 'Employees.BirthDate'
		public System.DateTime? BirthDate { get; set; }
		// 'Employees.BirthPlace'
		public string BirthPlace { get; set; }
		// 'Employees.ActivationDate'
		public System.DateTime? ActivationDate { get; set; }
		// 'Employees.EndDate'
		public System.DateTime? EndDate { get; set; }
		// 'Employees.Description'
		public string Description { get; set; }
		// Links
		// 'Employees.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
	}
}

