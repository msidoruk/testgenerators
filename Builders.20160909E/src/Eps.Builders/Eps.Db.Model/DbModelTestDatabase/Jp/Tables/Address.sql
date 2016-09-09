CREATE TABLE [Jp].[Address]
(
	[AddressId] INT IDENTITY(1,1) NOT NULL,
	[StateRegId] INT NULL,
	[JuridicalPersonAddressTypeId] INT NULL,
	[AddressValue] VARCHAR (500) NULL,
	-- Constraints
	CONSTRAINT PK_Address PRIMARY KEY(AddressId),
	CONSTRAINT FK_Jp_StateReg_Jp_Address_StateRegId FOREIGN KEY (StateRegId) REFERENCES Jp.[StateReg](StateRegId),
	CONSTRAINT FK_Dicts_JuridicalPersonAddressTypes_Jp_Address_JuridicalPersonAddressTypeId FOREIGN KEY (JuridicalPersonAddressTypeId) REFERENCES Dicts.[JuridicalPersonAddressTypes](JuridicalPersonAddressTypeId),
)

