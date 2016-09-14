using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.Eps
{
	// Automatically built for 'eps.JuridicalPersons'
	public class JuridicalPerson
	{
		// Properties
		// 'JuridicalPersons.JuridicalPersonId'
		public int JuridicalPersonId { get; set; }
		// 'JuridicalPersons.JuridicalPersonType'
		public int? JuridicalPersonType { get; set; }
		// 'JuridicalPersons.BankDepartmentId'
		public int? BankDepartmentId { get; set; }
		// 'JuridicalPersons.BusinessAreaId'
		public int? BusinessAreaId { get; set; }
		// 'JuridicalPersons.JuridicalPersonContractStateTypeId'
		public int? JuridicalPersonContractStateTypeId { get; set; }
		// 'JuridicalPersons.UseBankRko'
		public bool? UseBankRko { get; set; }
		// 'JuridicalPersons.PsbAccount'
		public string PsbAccount { get; set; }
		// 'JuridicalPersons.Comments'
		public string Comments { get; set; }
		// 'JuridicalPersons.AbonentsNumber'
		public int? AbonentsNumber { get; set; }
		// 'JuridicalPersons.AlternativePaymentChannels'
		public string AlternativePaymentChannels { get; set; }
		// 'JuridicalPersons.JuridicalPersonAbonentsNotificationWayTypeId'
		public int? JuridicalPersonAbonentsNotificationWayTypeId { get; set; }
		// 'JuridicalPersons.ApprovementStatusId'
		public int? ApprovementStatusId { get; set; }
		// 'JuridicalPersons.ActivationDate'
		public System.DateTime? ActivationDate { get; set; }
		// Links
		// 'JuridicalPersons.BankDepartmentId' --> 'Dicts.BankDepartments'
		public Dicts.BankDepartment BankDepartment { get; set; }
		// 'JuridicalPersons.BusinessAreaId' --> 'Dicts.BusinessAreas'
		public Dicts.BusinessArea BusinessArea { get; set; }
		// 'JuridicalPersons.JuridicalPersonContractStateTypeId' --> 'Dicts.JuridicalPersonContractStateTypes'
		public Dicts.JuridicalPersonContractStateType JuridicalPersonContractStateType { get; set; }
		// 'JuridicalPersons.JuridicalPersonAbonentsNotificationWayTypeId' --> 'Dicts.JuridicalPersonAbonentsNotificationWayTypes'
		public Dicts.JuridicalPersonAbonentsNotificationWayType JuridicalPersonAbonentsNotificationWayType { get; set; }
		// 'JuridicalPersons.ApprovementStatusId' --> 'Dicts.ApprovementStatuses'
		public Dicts.ApprovementStatus ApprovementStatus { get; set; }
	}
}

