using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.EmployeeDocuments'
	public class EmployeeDocument
	{
		// Properties
		// 'EmployeeDocuments.EmployeeDocumentId'
		public int EmployeeDocumentId { get; set; }
		// 'EmployeeDocuments.EmployeeId'
		public int? EmployeeId { get; set; }
		// 'EmployeeDocuments.EmployeeDocumentTypeId'
		public int? EmployeeDocumentTypeId { get; set; }
		// 'EmployeeDocuments.DocumentType'
		public string DocumentType { get; set; }
		// 'EmployeeDocuments.Number'
		public string Number { get; set; }
		// 'EmployeeDocuments.IssueLocation'
		public string IssueLocation { get; set; }
		// 'EmployeeDocuments.IssuedByDept'
		public string IssuedByDept { get; set; }
		// 'EmployeeDocuments.IssueDate'
		public System.DateTime? IssueDate { get; set; }
		// 'EmployeeDocuments.ValidUntilDate'
		public System.DateTime? ValidUntilDate { get; set; }
		// Links
		// 'EmployeeDocuments.EmployeeId' --> 'Jp.Employees'
		public JuridicalPerson.Employee Employee { get; set; }
		// 'EmployeeDocuments.EmployeeDocumentTypeId' --> 'Dicts.EmployeeDocumentTypes'
		public Dicts.EmployeeDocumentType EmployeeDocumentType { get; set; }
	}
}

