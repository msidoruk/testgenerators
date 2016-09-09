CREATE TABLE [Dicts].[JuridicalPersonFinancialStates]
(
	[JuridicalPersonFinancialStateId] INT IDENTITY(1,1) NOT NULL,
	[Code] VARCHAR (100) NULL,
	[Description] VARCHAR (200) NULL,
	[IsDefault] BIT NULL,
	[ActivationDate] DATE NULL,
	[EndDate] DATE NULL,
	-- Constraints
	CONSTRAINT PK_JuridicalPersonFinancialStates PRIMARY KEY(JuridicalPersonFinancialStateId),
)

