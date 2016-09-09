CREATE TABLE [Jp].[Accounts]
(
	[AccountId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	[JuridicalPersonAccountTypeId] INT NULL,
	[Bik] VARCHAR (20) NULL,
	[AccountNumber] VARCHAR (50) NULL,
	[Description] VARCHAR (200) NULL,
	-- Constraints
	CONSTRAINT PK_Accounts PRIMARY KEY(AccountId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_Accounts_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
	CONSTRAINT FK_Dicts_JuridicalPersonAccountTypes_Jp_Accounts_JuridicalPersonAccountTypeId FOREIGN KEY (JuridicalPersonAccountTypeId) REFERENCES Dicts.[JuridicalPersonAccountTypes](JuridicalPersonAccountTypeId),
)

