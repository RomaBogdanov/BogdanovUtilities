#define Local

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
//using SintezLibrary;

namespace SintezWpfUiLib.Model
{
#if !Local
    public class TestChatModel : ChatModel
    {
        public TestChatModel()
        {
            Messages = new ObservableCollection<ChatMsgRec>();
            string server = "***************";
            string dbname = "***************";
            string dblogin = "***************";
            string dbpassword = "***************";
            var connok = DBConnector.Initialize(server, dbname, dblogin, dbpassword);

            DBConnector.setCurrentManager(new ManagerUserRec(1));
            Manager = DBConnector.CurrentSintezUser;

            List<ChatMsgRec> chats = dbrec.loadRange<ChatMsgRec>(
                "select * from chatmsg", DBConnector.Sintez_Connection);

            string a = chats[0].MaketOS;
            Messages = new ObservableCollection<ChatMsgRec>(chats);

            List<SintezUserRec> users = dbrec.loadRange<SintezUserRec>(
                "select * from sintezuser", DBConnector.Sintez_Connection);
            Accounts = new ObservableCollection<SintezUserRec>(users);

            List<GoalRec> goals = dbrec.loadRange<GoalRec>(
                "select * from goals", DBConnector.Seller_Connection);
            Issues = new ObservableCollection<GoalRec>(goals);
            List<DocumentRec> rcs = new List<DocumentRec>();
            foreach (dbConnection item in DBConnector.OSPconnectionsList())
            {

                List<DocumentRec> recs = dbrec.loadRange<DocumentRec>(
                    "select top 10 * from card", item);
                /*foreach (var rec in recs)
                {
                    rec.ConnectionId = item.idx;
                }*/
                rcs.AddRange(recs);
            }
            Designes = new ObservableCollection<DocumentRec>(rcs);
        }
    }

#endif
}
