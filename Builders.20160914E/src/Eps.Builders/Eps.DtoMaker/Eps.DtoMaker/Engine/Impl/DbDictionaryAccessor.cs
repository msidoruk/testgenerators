using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Actions;
using EPS.DtoMaker.Engine.Base;
using EPS.DtoMaker.Engine.Helpers;

namespace EPS.DtoMaker.Engine.Impl
{
    public class DbDictionaryAccessor : IDbDictionaryAccessor
    {
        private readonly IDbConnection _dbConnection;

        public DbDictionaryAccessor(System.Data.IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Tuple<bool, string> GetTableInfo(string schemaName, string tableName, out TableInfo tableInfo)
        {
            //
            DataTable tableColumns;
            if (!DbHelpers.GetTableInfo(_dbConnection, schemaName, tableName, out tableInfo).Item1)
            {
                return Tuple.Create(false, $"Cannot get info for table {tableName}");
            }
            //
            return Tuple.Create(true, string.Empty);
        }
    }

    
}
