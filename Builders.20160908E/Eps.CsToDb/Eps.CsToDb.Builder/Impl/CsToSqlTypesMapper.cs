using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Builder.Base;

namespace Eps.CsToDb.Builder.Impl
{
    public class CsToSqlTypesMapper : ICsToSqlTypesMapper
    {
        private readonly Dictionary<string, string> _csToSqlTypesMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        private BuilderContext _context;

        public CsToSqlTypesMapper(BuilderContext context)
        {
            _context = context;
            //
            _csToSqlTypesMapping["Int64"] = SqlDbType.BigInt.ToString();
            // _csToSqlTypesMapping[""] = SqlDbType.Binary.ToString();
            _csToSqlTypesMapping["Boolean"] = SqlDbType.Bit.ToString();
            _csToSqlTypesMapping["Char"] = SqlDbType.Char.ToString();
            _csToSqlTypesMapping["DateTime"] = SqlDbType.DateTime.ToString();
            _csToSqlTypesMapping["Decimal"] = SqlDbType.Decimal.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Float.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Image.ToString();
            _csToSqlTypesMapping["Int32"] = SqlDbType.Int.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Money.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.NChar.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.NText.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.NVarChar.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Real.ToString();
            _csToSqlTypesMapping["Guid"] = SqlDbType.UniqueIdentifier.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.SmallDateTime.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.SmallInt.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.SmallMoney.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Text.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Timestamp.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.TinyInt.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.VarBinary.ToString();
            _csToSqlTypesMapping["String"] = SqlDbType.VarChar.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Variant.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Xml.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Udt.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Structured.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Date.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.Time.ToString();
            //_csToSqlTypesMapping[""] = SqlDbType.DateTime2.ToString();
            _csToSqlTypesMapping["DateTimeOffset"] = SqlDbType.DateTimeOffset.ToString();
        }

        public SqlDbType GetSqlType(Type fieldType)
        {
            try
            {
                SqlDbType result;
                SqlDbType.TryParse(_csToSqlTypesMapping[fieldType.Name], out result);
                return result;
            }
            catch (KeyNotFoundException exception)
            {
                _context.Logger.Error($"Cannot find SqlType for type name '{fieldType.Name}'", exception);
                //
                throw;
            }
        }
    }
}
