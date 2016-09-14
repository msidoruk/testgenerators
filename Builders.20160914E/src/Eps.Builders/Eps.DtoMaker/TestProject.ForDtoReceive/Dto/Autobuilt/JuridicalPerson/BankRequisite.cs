using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.BankRequisites'
	public class BankRequisite
	{
		// Properties
		// 'BankRequisites.BankRequisiteId'
		public int BankRequisiteId { get; set; }
		// 'BankRequisites.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// 'BankRequisites.BankBik'
		public string BankBik { get; set; }
		// 'BankRequisites.BankName'
		public string BankName { get; set; }
		// 'BankRequisites.CorrespondentAccount'
		public string CorrespondentAccount { get; set; }
		// 'BankRequisites.JuridicalPersonAccount'
		public string JuridicalPersonAccount { get; set; }
		// 'BankRequisites.CreateAccountDocumentScanId'
		public int? CreateAccountDocumentScanId { get; set; }
		// 'BankRequisites.ActivationDate'
		public System.DateTime? ActivationDate { get; set; }
		// 'BankRequisites.EndDate'
		public System.DateTime? EndDate { get; set; }
		// 'BankRequisites.Description'
		public string Description { get; set; }
		// Links
		// 'BankRequisites.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
		// 'BankRequisites.CreateAccountDocumentScanId' --> 'Eps.DocumentScans'
		public Eps.DocumentScan CreateAccountDocumentScan { get; set; }
	}
}

