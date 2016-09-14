CREATE TABLE [Jp].[Contractors]
(
	[JuridicalPersonId] INT NOT NULL,
	-- Constraints
	CONSTRAINT PK_Contractors PRIMARY KEY(JuridicalPersonId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_Contractors_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
)

