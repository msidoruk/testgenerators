CREATE TABLE [Jp].[StateRegAddresses]
(
	[StateRegAddressId] INT IDENTITY(1,1) NOT NULL,
	[StateRegistrationId] INT NULL,
	[JuridicalPersonAddressTypeId] INT NULL,
	[AddressValue] VARCHAR (500) NULL,
	-- Constraints
	CONSTRAINT PK_StateRegAddresses PRIMARY KEY(StateRegAddressId),
	CONSTRAINT FK_Jp_StateRegistrations_Jp_StateRegAddresses_StateRegistrationId FOREIGN KEY (StateRegistrationId) REFERENCES Jp.[StateRegistrations](StateRegistrationId),
	CONSTRAINT FK_Dicts_JuridicalPersonAddressTypes_Jp_StateRegAddresses_JuridicalPersonAddressTypeId FOREIGN KEY (JuridicalPersonAddressTypeId) REFERENCES Dicts.[JuridicalPersonAddressTypes](JuridicalPersonAddressTypeId),
)

