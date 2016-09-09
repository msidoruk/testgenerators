CREATE TABLE [Jp].[Contacts]
(
	[ContactId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	[JuridicalPersonContactTypeId] INT NULL,
	[ContactValue] VARCHAR (500) NULL,
	-- Constraints
	CONSTRAINT PK_Contacts PRIMARY KEY(ContactId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_Contacts_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
	CONSTRAINT FK_Dicts_JuridicalPersonContactTypes_Jp_Contacts_JuridicalPersonContactTypeId FOREIGN KEY (JuridicalPersonContactTypeId) REFERENCES Dicts.[JuridicalPersonContactTypes](JuridicalPersonContactTypeId),
)

