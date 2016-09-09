using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Base;

namespace EPS.DtoMaker.Engine.Impl
{
    public class SqlServerDbPropertyTypeMapper : IDbPropertyTypeMapper
    {
        Dictionary<string, Type> _typesMap = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
        public SqlServerDbPropertyTypeMapper()
        {
            _typesMap["image"] = typeof(byte[]);
            _typesMap["text"] = typeof(char[]);
            _typesMap["uniqueidentifier"] = typeof(Guid);
            _typesMap["date"] = typeof(DateTime);
            _typesMap["time"] = typeof(TimeSpan);
            _typesMap["datetime2"] = typeof(DateTime);
            _typesMap["datetimeoffset"] = typeof(DateTimeOffset);
            _typesMap["tinyint"] = typeof(int);
            _typesMap["smallint"] = typeof(short);
            _typesMap["int"] = typeof(int);
            _typesMap["smalldatetime"] = typeof(DateTime);
            _typesMap["real"] = typeof(decimal);
            _typesMap["money"] = typeof(decimal);
            _typesMap["datetime"] = typeof(DateTime);
            _typesMap["float"] = typeof(decimal);
            _typesMap["sql_variant"] = typeof(UnmappedType);
            _typesMap["ntext"] = typeof(char[]);
            _typesMap["bit"] = typeof(bool);
            _typesMap["decimal"] = typeof(decimal);
            _typesMap["numeric"] = typeof(decimal);
            _typesMap["smallmoney"] = typeof(decimal);
            _typesMap["bigint"] = typeof(long);
            _typesMap["hierarchyid"] = typeof(int);
            _typesMap["geometry"] = typeof(UnmappedType);
            _typesMap["geography"] = typeof(UnmappedType);
            _typesMap["varbinary"] = typeof(byte []);
            _typesMap["varchar"] = typeof(string);
            _typesMap["binary"] = typeof(byte []);
            _typesMap["char"] = typeof(string);
            _typesMap["timestamp"] = typeof(TimeSpan);
            _typesMap["nvarchar"] = typeof(string);
            _typesMap["nchar"] = typeof(string);
            _typesMap["xml"] = typeof(UnmappedType);
            _typesMap["sysname"] = typeof(UnmappedType);
        }

        public Type Map<TK, TV>(Dictionary<TK, TV> dbTypes, TK dbTypeId) where TV : DbTypeInfoParser
        {
            TV dbTypeName = dbTypes[dbTypeId];
            Type mappedType = _typesMap[dbTypeName.TypeName];
            return mappedType;
        }
    }

    public class UnmappedType
    {
    }
}
