using System.Xml.Serialization;

namespace Eps.DbSharedModel.DbModel.Parts
{
    [XmlRoot("DbColumn")]
    public class DbColumnDef
    {
        public DbColumnDef()
        {
        }

        [XmlAttribute("name")]
        public string ColumnName
        {
            get; set; }
    }
}
