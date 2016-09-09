CREATE TABLE [Jp].[BankRequisites]
(
	[BankRequisiteId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	[BankBik] VARCHAR (20) NULL,
	[BankName] VARCHAR (200) NULL,
	[CorrespondentAccount] VARCHAR (50) NULL,
	[JuridicalPersonAccount] VARCHAR (50) NULL,
	[CreateAccountDocumentScanId] INT NULL,
	[ActivationDate] DATE NULL,
	[EndDate] DATE NULL,
	[Description] VARCHAR (200) NULL,
	-- Constraints
	CONSTRAINT PK_BankRequisites PRIMARY KEY(BankRequisiteId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_BankRequisites_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
	CONSTRAINT FK_Eps_DocumentScans_Jp_BankRequisites_CreateAccountDocumentScanId FOREIGN KEY (CreateAccountDocumentScanId) REFERENCES Eps.[DocumentScans](DocumentScanId),
)

