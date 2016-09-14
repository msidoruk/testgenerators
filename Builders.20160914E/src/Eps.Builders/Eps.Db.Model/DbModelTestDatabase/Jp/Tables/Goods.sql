CREATE TABLE [Jp].[Goods]
(
	[GoodId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	[GoodsCategoryId] INT NULL,
	-- Constraints
	CONSTRAINT PK_Goods PRIMARY KEY(GoodId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_Goods_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
	CONSTRAINT FK_Dicts_GoodsCategories_Jp_Goods_GoodsCategoryId FOREIGN KEY (GoodsCategoryId) REFERENCES Dicts.[GoodsCategories](GoodsCategoryId),
)

