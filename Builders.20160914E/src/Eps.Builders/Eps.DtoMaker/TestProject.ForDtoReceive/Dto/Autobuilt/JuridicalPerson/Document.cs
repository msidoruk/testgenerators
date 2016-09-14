using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.Documents'
	public class Document
	{
		// Properties
		// 'Documents.DocumentId'
		public int DocumentId { get; set; }
		// 'Documents.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// 'Documents.JuridicalPersonDocumentTypeId'
		public int? JuridicalPersonDocumentTypeId { get; set; }
		// 'Documents.DocumentScanId'
		public int? DocumentScanId { get; set; }
		// 'Documents.DocumentFileLink'
		public string DocumentFileLink { get; set; }
		// 'Documents.CopyDate'
		public System.DateTime? CopyDate { get; set; }
		// 'Documents.OriginalDate'
		public System.DateTime? OriginalDate { get; set; }
		// 'Documents.DocumentsStoringPlaceId'
		public int? DocumentsStoringPlaceId { get; set; }
		// 'Documents.Description'
		public string Description { get; set; }
		// 'Documents.ActivationDate'
		public System.DateTime? ActivationDate { get; set; }
		// Links
		// 'Documents.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
		// 'Documents.JuridicalPersonDocumentTypeId' --> 'Dicts.JuridicalPersonDocumentTypes'
		public Dicts.JuridicalPersonDocumentType JuridicalPersonDocumentType { get; set; }
		// 'Documents.DocumentScanId' --> 'Eps.DocumentScans'
		public Eps.DocumentScan DocumentScan { get; set; }
		// 'Documents.DocumentsStoringPlaceId' --> 'Dicts.DocumentsStoringPlaces'
		public Dicts.DocumentsStoringPlace DocumentsStoringPlace { get; set; }
	}
}

