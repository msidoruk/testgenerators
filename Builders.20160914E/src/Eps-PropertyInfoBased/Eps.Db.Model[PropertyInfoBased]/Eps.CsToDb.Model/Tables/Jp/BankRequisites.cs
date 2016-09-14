using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Model.Tables.Jp
{
    [DbModelPart]
    public class BankRequisites
    {
        [DbPrimaryKey(IsIdentity = true)]
        [DbColumn(SqlDbType.Int)]
        public int Key;

        public JuridicalPerson REFKEY;

[JuridicalPersonId] INT NOT NULL,
	[Description] VARCHAR(200) NULL,
	[BankBik] VARCHAR(50) NULL,
	[BankName] VARCHAR(200) NULL,
	[CorrespondentAccount] VARCHAR(50) NULL,
	[JuridicalPersonAccount] VARCHAR(50) NULL,
	[CreateAccountDocumentScanId]
        INT NULL,

    [ActivationDate] DATE NULL,
    [EndDate] DATE NULL
    }
}
