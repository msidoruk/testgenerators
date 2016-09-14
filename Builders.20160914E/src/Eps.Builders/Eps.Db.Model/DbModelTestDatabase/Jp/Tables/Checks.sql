CREATE TABLE [Jp].[Checks]
(
	[CheckId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	[JuridicalPersonCheckTypeId] INT NULL,
	[CheckResult] VARCHAR (1000) NULL,
	-- Constraints
	CONSTRAINT PK_Checks PRIMARY KEY(CheckId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_Checks_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
	CONSTRAINT FK_Dicts_JuridicalPersonCheckTypes_Jp_Checks_JuridicalPersonCheckTypeId FOREIGN KEY (JuridicalPersonCheckTypeId) REFERENCES Dicts.[JuridicalPersonCheckTypes](JuridicalPersonCheckTypeId),
)

