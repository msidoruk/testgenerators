using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.Contractors'
	public class Contractor
	{
		// Properties
		// 'Contractors.JuridicalPersonId'
		public int JuridicalPersonId { get; set; }
		// Links
		// 'Contractors.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
	}
}

