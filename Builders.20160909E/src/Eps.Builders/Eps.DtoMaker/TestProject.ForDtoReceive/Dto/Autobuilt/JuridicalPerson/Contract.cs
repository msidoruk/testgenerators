using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
using TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.Contracts'
	public class Contract
	{
		// Properties
		// 'Contracts.ContractId'
		public int ContractId { get; set; }
		// 'Contracts.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// 'Contracts.JuridicalPersonContactTypeId'
		public int? JuridicalPersonContactTypeId { get; set; }
		// 'Contracts.ContractNumber'
		public string ContractNumber { get; set; }
		// 'Contracts.ContractDate'
		public string ContractDate { get; set; }
		// 'Contracts.ContractDocumentScanId'
		public int? ContractDocumentScanId { get; set; }
		// 'Contracts.SignedEmployeeId'
		public int? SignedEmployeeId { get; set; }
		// 'Contracts.TerminationDate'
		public System.DateTime? TerminationDate { get; set; }
		// 'Contracts.TerminationReason'
		public string TerminationReason { get; set; }
		// 'Contracts.TerminationDocumentScanId'
		public int? TerminationDocumentScanId { get; set; }
		// 'Contracts.Description'
		public string Description { get; set; }
		// 'Contracts.ActivationDate'
		public System.DateTime? ActivationDate { get; set; }
		// Links
		// 'Contracts.SignedEmployeeId' --> 'Jp.Employees'
		public JuridicalPerson.Employee SignedEmployee { get; set; }
		// 'Contracts.TerminationDocumentScanId' --> 'Eps.DocumentScans'
		public Eps.DocumentScan TerminationDocumentScan { get; set; }
		// 'Contracts.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
		// 'Contracts.JuridicalPersonContactTypeId' --> 'Dicts.JuridicalPersonContactTypes'
		public Dicts.JuridicalPersonContactType JuridicalPersonContactType { get; set; }
		// 'Contracts.ContractDocumentScanId' --> 'Eps.DocumentScans'
		public Eps.DocumentScan ContractDocumentScan { get; set; }
	}
}

