CREATE TABLE [Eps].[JuridicalPersons]
(
	[JuridicalPersonId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonType] INT NULL,
	[BankDepartmentId] INT NULL,
	[BusinessAreaId] INT NULL,
	[JuridicalPersonContractStateTypeId] INT NULL,
	[UseBankRko] BIT NULL,
	[PsbAccount] VARCHAR (50) NULL,
	[Comments] VARCHAR (1000) NULL,
	[AbonentsNumber] INT NULL,
	[AlternativePaymentChannels] VARCHAR (1000) NULL,
	[JuridicalPersonAbonentsNotificationWayTypeId] INT NULL,
	[ApprovementStatusId] INT NULL,
	[ActivationDate] DATETIME NULL,
	-- Constraints
	CONSTRAINT PK_JuridicalPersons PRIMARY KEY(JuridicalPersonId),
	CONSTRAINT FK_Dicts_BankDepartments_Eps_JuridicalPersons_BankDepartmentId FOREIGN KEY (BankDepartmentId) REFERENCES Dicts.[BankDepartments](BankDepartmentId),
	CONSTRAINT FK_Dicts_BusinessAreas_Eps_JuridicalPersons_BusinessAreaId FOREIGN KEY (BusinessAreaId) REFERENCES Dicts.[BusinessAreas](BusinessAreaId),
	CONSTRAINT FK_Dicts_JuridicalPersonContractStateTypes_Eps_JuridicalPersons_JuridicalPersonContractStateTypeId FOREIGN KEY (JuridicalPersonContractStateTypeId) REFERENCES Dicts.[JuridicalPersonContractStateTypes](JuridicalPersonContractStateTypeId),
	CONSTRAINT FK_Dicts_JuridicalPersonAbonentsNotificationWayTypes_Eps_JuridicalPersons_JuridicalPersonAbonentsNotificationWayTypeId FOREIGN KEY (JuridicalPersonAbonentsNotificationWayTypeId) REFERENCES Dicts.[JuridicalPersonAbonentsNotificationWayTypes](JuridicalPersonAbonentsNotificationWayTypeId),
	CONSTRAINT FK_Dicts_ApprovementStatuses_Eps_JuridicalPersons_ApprovementStatusId FOREIGN KEY (ApprovementStatusId) REFERENCES Dicts.[ApprovementStatuses](ApprovementStatusId),
)

