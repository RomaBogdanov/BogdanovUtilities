//using SintezLibrary;
namespace SintezWpfUiLib.Model
{
    public class DocumentRec
    {
        public long idx;

        public DocumentRec(long id_doc)
        { }

        public string Production { get; internal set; }
        public long baseid { get; internal set; }
        public Connection connection { get; internal set; }
    }

    public class Connection
    {
        public long connector_id { get; internal set; }
    }
}