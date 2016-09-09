using System;
using System.Data;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;

namespace Eps.CsToDb.Model.DbModelTemplates.StandardDictionary
{
    // Таблица 9. Элемент набора записей стандартизованных справочников
    [Trait(typeof(HasActivityRange))]
    [Trait(typeof(RequireConfirmation.ConfirmationRequired))]
    public class StandardDictionary
    {
        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.DictionaryCodeSize)]
        string Code;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.DescriptionSize)]
        string Description;

        [DbColumn(SqlDbType.Bit)]
        bool IsDefault;
    }
}
