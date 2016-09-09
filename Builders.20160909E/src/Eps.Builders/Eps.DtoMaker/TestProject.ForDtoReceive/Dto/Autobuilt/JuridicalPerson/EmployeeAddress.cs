using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.EmployeeAddresses'
	public class EmployeeAddress
	{
		// Properties
		// 'EmployeeAddresses.EmployeeAddressId'
		public int EmployeeAddressId { get; set; }
		// 'EmployeeAddresses.EmployeeId'
		public int? EmployeeId { get; set; }
		// 'EmployeeAddresses.AddressType'
		public int? AddressType { get; set; }
		// 'EmployeeAddresses.Address'
		public string Address { get; set; }
		// Links
		// 'EmployeeAddresses.EmployeeId' --> 'Jp.Employees'
		public JuridicalPerson.Employee Employee { get; set; }
	}
}

