using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.StandardDictionary;

namespace Eps.CsToDb.Model.Tables.Dicts
{
    // Вид контакта юридического лица - перефразировано в "Тип контактного сотрудника ЮЛ", так как "контакты у ЮЛ" уже есть.
    [DbModelPart]
    [Trait(typeof(StandardDictionary))]
    public class ContactEmployeeTypes
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;
    }
}
