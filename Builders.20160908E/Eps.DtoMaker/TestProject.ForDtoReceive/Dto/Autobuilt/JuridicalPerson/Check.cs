using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.Checks'
	public class Check
	{
		// Properties
		// 'Checks.CheckId'
		public int CheckId { get; set; }
		// 'Checks.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// 'Checks.JuridicalPersonCheckTypeId'
		public int? JuridicalPersonCheckTypeId { get; set; }
		// 'Checks.CheckResult'
		public string CheckResult { get; set; }
		// Links
		// 'Checks.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
		// 'Checks.JuridicalPersonCheckTypeId' --> 'Dicts.JuridicalPersonCheckTypes'
		public Dicts.JuridicalPersonCheckType JuridicalPersonCheckType { get; set; }
	}
}

