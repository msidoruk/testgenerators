CREATE TABLE [Jp].[Contracts]
(
	[ContractId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	[JuridicalPersonContactTypeId] INT NULL,
	[ContractNumber] VARCHAR (200) NULL,
	[ContractDate] VARCHAR (100) NULL,
	[ContractDocumentScanId] INT NULL,
	[SignedEmployeeId] INT NULL,
	[TerminationDate] DATETIME NULL,
	[TerminationReason] VARCHAR (200) NULL,
	[TerminationDocumentScanId] INT NULL,
	[Description] VARCHAR (200) NULL,
	[ActivationDate] DATETIME NULL,
	-- Constraints
	CONSTRAINT PK_Contracts PRIMARY KEY(ContractId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_Contracts_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
	CONSTRAINT FK_Dicts_JuridicalPersonContactTypes_Jp_Contracts_JuridicalPersonContactTypeId FOREIGN KEY (JuridicalPersonContactTypeId) REFERENCES Dicts.[JuridicalPersonContactTypes](JuridicalPersonContactTypeId),
	CONSTRAINT FK_Eps_DocumentScans_Jp_Contracts_ContractDocumentScanId FOREIGN KEY (ContractDocumentScanId) REFERENCES Eps.[DocumentScans](DocumentScanId),
	CONSTRAINT FK_Jp_Employees_Jp_Contracts_SignedEmployeeId FOREIGN KEY (SignedEmployeeId) REFERENCES Jp.[Employees](EmployeeId),
	CONSTRAINT FK_Eps_DocumentScans_Jp_Contracts_TerminationDocumentScanId FOREIGN KEY (TerminationDocumentScanId) REFERENCES Eps.[DocumentScans](DocumentScanId),
)

