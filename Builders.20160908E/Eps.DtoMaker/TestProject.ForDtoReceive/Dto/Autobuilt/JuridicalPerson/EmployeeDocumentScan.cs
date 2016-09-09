using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
using TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.EmployeeDocumentScans'
	public class EmployeeDocumentScan
	{
		// Properties
		// 'EmployeeDocumentScans.EmployeeDocumentScanId'
		public int EmployeeDocumentScanId { get; set; }
		// 'EmployeeDocumentScans.EmployeeDocumentId'
		public int? EmployeeDocumentId { get; set; }
		// 'EmployeeDocumentScans.EmployeeDocumentScanType'
		public int? EmployeeDocumentScanType { get; set; }
		// 'EmployeeDocumentScans.DocumentScanId'
		public int? DocumentScanId { get; set; }
		// Links
		// 'EmployeeDocumentScans.EmployeeDocumentId' --> 'Jp.EmployeeDocuments'
		public JuridicalPerson.EmployeeDocument EmployeeDocument { get; set; }
		// 'EmployeeDocumentScans.DocumentScanId' --> 'Eps.DocumentScans'
		public Eps.DocumentScan DocumentScan { get; set; }
	}
}

