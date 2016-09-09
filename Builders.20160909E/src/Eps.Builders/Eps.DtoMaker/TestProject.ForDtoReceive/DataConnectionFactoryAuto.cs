
using LinqToDB.Mapping;
using TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson;

namespace Eps.Dal.DataConnection
{
    public partial class DataConnectionFactory
    {
        protected void AppendAutoMappings(FluentMappingBuilder mappingBuilder)
        {
            // $AUTO_CODE_BEGIN (!!!Don't edit anything between '$AUTO_CODE_BEGIN' and '$AUTO_CODE_END' these changes will be lost after next generation with DtoMaker).
            // !!!Don't edit anything between $+'AUTO_CODE_BEGIN' and $+'AUTO_CODE_END' these changes will be lost after next generation with DtoMaker.
            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.Contractor>()
            	.HasSchemaName("jp")
            	.Property(s=>s.JuridicalPersonId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Eps.JuridicalPerson>()
            	.HasSchemaName("eps")
            	.Property(s=>s.JuridicalPersonId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonType)
            	.Property(s=>s.BankDepartmentId)
            	.Property(s=>s.BusinessAreaId)
            	.Property(s=>s.JuridicalPersonContractStateTypeId)
            	.Property(s=>s.UseBankRko)
            	.Property(s=>s.PsbAccount)
            	.Property(s=>s.Comments)
            	.Property(s=>s.AbonentsNumber)
            	.Property(s=>s.AlternativePaymentChannels)
            	.Property(s=>s.JuridicalPersonAbonentsNotificationWayTypeId)
            	.Property(s=>s.ApprovementStatusId)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.BankDepartment).HasAttribute(new AssociationAttribute{ThisKey="BankDepartmentId", OtherKey="BankDepartmentId"})
            	.Property(s=>s.BusinessArea).HasAttribute(new AssociationAttribute{ThisKey="BusinessAreaId", OtherKey="BusinessAreaId"})
            	.Property(s=>s.JuridicalPersonContractStateType).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonContractStateTypeId", OtherKey="JuridicalPersonContractStateTypeId"})
            	.Property(s=>s.JuridicalPersonAbonentsNotificationWayType).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonAbonentsNotificationWayTypeId", OtherKey="JuridicalPersonAbonentsNotificationWayTypeId"})
            	.Property(s=>s.ApprovementStatus).HasAttribute(new AssociationAttribute{ThisKey="ApprovementStatusId", OtherKey="ApprovementStatusId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Eps.DocumentScan>()
            	.HasSchemaName("eps")
            	.Property(s=>s.DocumentScanId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Image)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.Account>()
            	.HasSchemaName("jp")
            	.Property(s=>s.AccountId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.JuridicalPersonAccountTypeId)
            	.Property(s=>s.Bik)
            	.Property(s=>s.AccountNumber)
            	.Property(s=>s.Description)
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	.Property(s=>s.JuridicalPersonAccountType).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonAccountTypeId", OtherKey="JuridicalPersonAccountTypeId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.BankRequisite>()
            	.HasSchemaName("jp")
            	.Property(s=>s.BankRequisiteId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.BankBik)
            	.Property(s=>s.BankName)
            	.Property(s=>s.CorrespondentAccount)
            	.Property(s=>s.JuridicalPersonAccount)
            	.Property(s=>s.CreateAccountDocumentScanId)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	.Property(s=>s.Description)
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	.Property(s=>s.CreateAccountDocumentScan).HasAttribute(new AssociationAttribute{ThisKey="DocumentScanId", OtherKey="DocumentScanId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.Check>()
            	.HasSchemaName("jp")
            	.Property(s=>s.CheckId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.JuridicalPersonCheckTypeId)
            	.Property(s=>s.CheckResult)
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	.Property(s=>s.JuridicalPersonCheckType).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonCheckTypeId", OtherKey="JuridicalPersonCheckTypeId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.Contact>()
            	.HasSchemaName("jp")
            	.Property(s=>s.ContactId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.JuridicalPersonContactTypeId)
            	.Property(s=>s.ContactValue)
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	.Property(s=>s.JuridicalPersonContactType).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonContactTypeId", OtherKey="JuridicalPersonContactTypeId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.Document>()
            	.HasSchemaName("jp")
            	.Property(s=>s.DocumentId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.JuridicalPersonDocumentTypeId)
            	.Property(s=>s.DocumentScanId)
            	.Property(s=>s.DocumentFileLink)
            	.Property(s=>s.CopyDate)
            	.Property(s=>s.OriginalDate)
            	.Property(s=>s.DocumentsStoringPlaceId)
            	.Property(s=>s.Description)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	.Property(s=>s.JuridicalPersonDocumentType).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonDocumentTypeId", OtherKey="JuridicalPersonDocumentTypeId"})
            	.Property(s=>s.DocumentScan).HasAttribute(new AssociationAttribute{ThisKey="DocumentScanId", OtherKey="DocumentScanId"})
            	.Property(s=>s.DocumentsStoringPlace).HasAttribute(new AssociationAttribute{ThisKey="DocumentsStoringPlaceId", OtherKey="DocumentsStoringPlaceId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.EmployeeAddress>()
            	.HasSchemaName("jp")
            	.Property(s=>s.EmployeeAddressId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.EmployeeId)
            	.Property(s=>s.AddressType)
            	.Property(s=>s.Address)
            	.Property(s=>s.Employee).HasAttribute(new AssociationAttribute{ThisKey="EmployeeId", OtherKey="EmployeeId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.EmployeeContact>()
            	.HasSchemaName("jp")
            	.Property(s=>s.EmployeeContactId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.EmployeeId)
            	.Property(s=>s.EmployeeContactTypeId)
            	.Property(s=>s.ContactValue)
            	.Property(s=>s.Employee).HasAttribute(new AssociationAttribute{ThisKey="EmployeeId", OtherKey="EmployeeId"})
            	.Property(s=>s.EmployeeContactType).HasAttribute(new AssociationAttribute{ThisKey="EmployeeContactTypeId", OtherKey="EmployeeContactTypeId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.EmployeeDocument>()
            	.HasSchemaName("jp")
            	.Property(s=>s.EmployeeDocumentId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.EmployeeId)
            	.Property(s=>s.EmployeeDocumentTypeId)
            	.Property(s=>s.DocumentType)
            	.Property(s=>s.Number)
            	.Property(s=>s.IssueLocation)
            	.Property(s=>s.IssuedByDept)
            	.Property(s=>s.IssueDate)
            	.Property(s=>s.ValidUntilDate)
            	.Property(s=>s.Employee).HasAttribute(new AssociationAttribute{ThisKey="EmployeeId", OtherKey="EmployeeId"})
            	.Property(s=>s.EmployeeDocumentType).HasAttribute(new AssociationAttribute{ThisKey="EmployeeDocumentTypeId", OtherKey="EmployeeDocumentTypeId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.EmployeeDocumentScan>()
            	.HasSchemaName("jp")
            	.Property(s=>s.EmployeeDocumentScanId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.EmployeeDocumentId)
            	.Property(s=>s.EmployeeDocumentScanType)
            	.Property(s=>s.DocumentScanId)
            	.Property(s=>s.EmployeeDocument).HasAttribute(new AssociationAttribute{ThisKey="EmployeeDocumentId", OtherKey="EmployeeDocumentId"})
            	.Property(s=>s.DocumentScan).HasAttribute(new AssociationAttribute{ThisKey="DocumentScanId", OtherKey="DocumentScanId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.Employee>()
            	.HasSchemaName("jp")
            	.Property(s=>s.EmployeeId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.Position)
            	.Property(s=>s.FirstName)
            	.Property(s=>s.LastName)
            	.Property(s=>s.MiddleName)
            	.Property(s=>s.Citizenship)
            	.Property(s=>s.BirthDate)
            	.Property(s=>s.BirthPlace)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	.Property(s=>s.Description)
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.Info>()
            	.HasSchemaName("jp")
            	.Property(s=>s.InfoId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.OkopfId)
            	.Property(s=>s.Name)
            	.Property(s=>s.ShortName)
            	.Property(s=>s.ForeignName)
            	.Property(s=>s.IsRegisteredInRussia)
            	.Property(s=>s.MustBeLicensed)
            	.Property(s=>s.HaveOwner)
            	.Property(s=>s.ActsForOwnerInterest)
            	.Property(s=>s.JuridicalPersonFinancialStateId)
            	.Property(s=>s.BusinessAreaId)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	.Property(s=>s.Description)
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	.Property(s=>s.Okopf).HasAttribute(new AssociationAttribute{ThisKey="OkopfId", OtherKey="OkopfId"})
            	.Property(s=>s.JuridicalPersonFinancialState).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonFinancialStateId", OtherKey="JuridicalPersonFinancialStateId"})
            	.Property(s=>s.BusinessArea).HasAttribute(new AssociationAttribute{ThisKey="BusinessAreaId", OtherKey="BusinessAreaId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.BankProduct>()
            	.HasSchemaName("jp")
            	.Property(s=>s.BankProductId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	.Property(s=>s.BankProduct_).HasAttribute(new AssociationAttribute{ThisKey="BankProductId", OtherKey="BankProductId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.Contract>()
            	.HasSchemaName("jp")
            	.Property(s=>s.ContractId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.JuridicalPersonContactTypeId)
            	.Property(s=>s.ContractNumber)
            	.Property(s=>s.ContractDate)
            	.Property(s=>s.ContractDocumentScanId)
            	.Property(s=>s.SignedEmployeeId)
            	.Property(s=>s.TerminationDate)
            	.Property(s=>s.TerminationReason)
            	.Property(s=>s.TerminationDocumentScanId)
            	.Property(s=>s.Description)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.SignedEmployee).HasAttribute(new AssociationAttribute{ThisKey="EmployeeId", OtherKey="EmployeeId"})
            	.Property(s=>s.TerminationDocumentScan).HasAttribute(new AssociationAttribute{ThisKey="DocumentScanId", OtherKey="DocumentScanId"})
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	.Property(s=>s.JuridicalPersonContactType).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonContactTypeId", OtherKey="JuridicalPersonContactTypeId"})
            	.Property(s=>s.ContractDocumentScan).HasAttribute(new AssociationAttribute{ThisKey="DocumentScanId", OtherKey="DocumentScanId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.Good>()
            	.HasSchemaName("jp")
            	.Property(s=>s.GoodId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.GoodsCategoryId)
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	.Property(s=>s.GoodsCategory).HasAttribute(new AssociationAttribute{ThisKey="GoodsCategoryId", OtherKey="GoodsCategoryId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.StateRegistration>()
            	.HasSchemaName("jp")
            	.Property(s=>s.StateRegistrationId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.JuridicalPersonId)
            	.Property(s=>s.RegistrationDate)
            	.Property(s=>s.RegistrationPlace)
            	.Property(s=>s.RegisteredByDept)
            	.Property(s=>s.OGRN)
            	.Property(s=>s.INN)
            	.Property(s=>s.KPP)
            	.Property(s=>s.OKATO)
            	.Property(s=>s.OKTMO)
            	.Property(s=>s.OKFS)
            	.Property(s=>s.OKPO)
            	.Property(s=>s.OKOPF)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	.Property(s=>s.Description)
            	.Property(s=>s.JuridicalPerson).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonId", OtherKey="JuridicalPersonId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.StateRegAddress>()
            	.HasSchemaName("jp")
            	.Property(s=>s.StateRegAddressId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.StateRegistrationId)
            	.Property(s=>s.JuridicalPersonAddressTypeId)
            	.Property(s=>s.AddressValue)
            	.Property(s=>s.StateRegistration).HasAttribute(new AssociationAttribute{ThisKey="StateRegistrationId", OtherKey="StateRegistrationId"})
            	.Property(s=>s.JuridicalPersonAddressType).HasAttribute(new AssociationAttribute{ThisKey="JuridicalPersonAddressTypeId", OtherKey="JuridicalPersonAddressTypeId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson.StateRegOkved>()
            	.HasSchemaName("jp")
            	.Property(s=>s.StateRegOkvedId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.StateRegistrationId)
            	.Property(s=>s.Okved)
            	.Property(s=>s.StateRegistration).HasAttribute(new AssociationAttribute{ThisKey="StateRegistrationId", OtherKey="StateRegistrationId"})
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.ApprovementStatus>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.ApprovementStatusId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.BankDepartment>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.BankDepartmentId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.BankProduct>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.BankProductId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.BusinessArea>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.BusinessAreaId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.DocumentsStoringPlace>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.DocumentsStoringPlaceId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.GoodsCategory>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.GoodsCategoryId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.JuridicalPersonAbonentsNotificationWayType>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.JuridicalPersonAbonentsNotificationWayTypeId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.JuridicalPersonAccountType>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.JuridicalPersonAccountTypeId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.JuridicalPersonAddressType>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.JuridicalPersonAddressTypeId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.JuridicalPersonCheckType>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.JuridicalPersonCheckTypeId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.JuridicalPersonContactType>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.JuridicalPersonContactTypeId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.JuridicalPersonContractStateType>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.JuridicalPersonContractStateTypeId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.JuridicalPersonDocumentType>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.JuridicalPersonDocumentTypeId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.EmployeeContactType>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.EmployeeContactTypeId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.EmployeeDocumentType>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.EmployeeDocumentTypeId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.JuridicalPersonFinancialState>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.JuridicalPersonFinancialStateId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.Okopf>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.OkopfId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            mappingBuilder.Entity<TestProject.ForDtoReceive.Dto.Autobuilt.Dicts.Region>()
            	.HasSchemaName("dicts")
            	.Property(s=>s.RegionId).IsPrimaryKey().IsIdentity()
            	.Property(s=>s.Code)
            	.Property(s=>s.Description)
            	.Property(s=>s.IsDefault)
            	.Property(s=>s.ActivationDate)
            	.Property(s=>s.EndDate)
            	; /*End*/

            // $AUTO_CODE_END
        }
    }
}
