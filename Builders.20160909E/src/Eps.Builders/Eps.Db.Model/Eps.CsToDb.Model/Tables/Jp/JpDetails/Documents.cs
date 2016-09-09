using System;
using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;
using Eps.CsToDb.Model.DbModelTemplates.RequireConfirmation;
using Eps.CsToDb.Model.Tables.Dicts;
using Eps.CsToDb.Model.Tables.Eps;
using Eps.CsToDb.Model.Tables.Jp.Dicts;

namespace Eps.CsToDb.Model.Tables.Jp.JpDetails
{
    /* Таблица 7. Элемент списка документов Контрагента */
    [DbModelPart]
    [DbSchema("Jp")]
    [Trait(typeof(WithDescription_200))]
    [Trait(typeof(HasActivityRange))]
    [Trait(typeof(ConfirmationRequired))]
    public class Documents
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        JuridicalPersons REFKEY;

        JuridicalPersonDocumentTypes REFKEY_ContactType;

        DocumentScans REFKEY_DocScan;

        string DocumentFileLink;

        DateTime CopyDate;

        DateTime OriginalDate;

        DocumentsStoringPlaces REFKEY_DocStoringPlaces;
    }
}
