CREATE TABLE [Jp].[BankProducts]
(
	[BankProductId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	-- Constraints
	CONSTRAINT PK_BankProducts PRIMARY KEY(BankProductId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_BankProducts_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
	CONSTRAINT FK_Dicts_BankProducts_Jp_BankProducts_BankProductId FOREIGN KEY (BankProductId) REFERENCES Dicts.[BankProducts](BankProductId),
)

