using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.Info'
	public class Info
	{
		// Properties
		// 'Info.InfoId'
		public int InfoId { get; set; }
		// 'Info.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// 'Info.OkopfId'
		public int? OkopfId { get; set; }
		// 'Info.Name'
		public string Name { get; set; }
		// 'Info.ShortName'
		public string ShortName { get; set; }
		// 'Info.ForeignName'
		public string ForeignName { get; set; }
		// 'Info.IsRegisteredInRussia'
		public bool? IsRegisteredInRussia { get; set; }
		// 'Info.MustBeLicensed'
		public bool? MustBeLicensed { get; set; }
		// 'Info.HaveOwner'
		public bool? HaveOwner { get; set; }
		// 'Info.ActsForOwnerInterest'
		public bool? ActsForOwnerInterest { get; set; }
		// 'Info.JuridicalPersonFinancialStateId'
		public int? JuridicalPersonFinancialStateId { get; set; }
		// 'Info.BusinessAreaId'
		public int? BusinessAreaId { get; set; }
		// 'Info.ActivationDate'
		public System.DateTime? ActivationDate { get; set; }
		// 'Info.EndDate'
		public System.DateTime? EndDate { get; set; }
		// 'Info.Description'
		public string Description { get; set; }
		// Links
		// 'Info.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
		// 'Info.OkopfId' --> 'Dicts.Okopfs'
		public Dicts.Okopf Okopf { get; set; }
		// 'Info.JuridicalPersonFinancialStateId' --> 'Dicts.JuridicalPersonFinancialStates'
		public Dicts.JuridicalPersonFinancialState JuridicalPersonFinancialState { get; set; }
		// 'Info.BusinessAreaId' --> 'Dicts.BusinessAreas'
		public Dicts.BusinessArea BusinessArea { get; set; }
	}
}

