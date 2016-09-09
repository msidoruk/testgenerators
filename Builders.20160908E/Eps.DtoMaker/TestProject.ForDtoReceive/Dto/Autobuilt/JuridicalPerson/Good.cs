using System;
using TestProject.ForDtoReceive.Dto.Autobuilt.Dicts;
using TestProject.ForDtoReceive.Dto.Autobuilt.Eps;
namespace TestProject.ForDtoReceive.Dto.Autobuilt.JuridicalPerson
{
	// Automatically built for 'jp.Goods'
	public class Good
	{
		// Properties
		// 'Goods.GoodId'
		public int GoodId { get; set; }
		// 'Goods.JuridicalPersonId'
		public int? JuridicalPersonId { get; set; }
		// 'Goods.GoodsCategoryId'
		public int? GoodsCategoryId { get; set; }
		// Links
		// 'Goods.JuridicalPersonId' --> 'Eps.JuridicalPersons'
		public Eps.JuridicalPerson JuridicalPerson { get; set; }
		// 'Goods.GoodsCategoryId' --> 'Dicts.GoodsCategories'
		public Dicts.GoodsCategory GoodsCategory { get; set; }
	}
}

