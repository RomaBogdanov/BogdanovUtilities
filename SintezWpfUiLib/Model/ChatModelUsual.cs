#define Local

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SintezWpfUiLib.Model
{

#if !Local
    public class ChatModelUsual : ChatModel
    {

        public ChatModelUsual()
        {
            Messages = new ObservableCollection<ChatMsgRec>();
            Accounts = new ObservableCollection<SintezUserRec>();
            Issues = new ObservableCollection<GoalRec>();
            Designes = new ObservableCollection<DocumentRec>();

            Manager = DBConnector.CurrentSintezUser;
            List<SintezUserRec> users = dbrec.loadRange<SintezUserRec>(
                "select * from sintezuser", DBConnector.Sintez_Connection);
            Accounts = new ObservableCollection<SintezUserRec>(users);
            Accounts.Remove(Manager);
            List<GoalRec> goals = dbrec.loadRange<GoalRec>(
                "select * from goals where closetype=0", DBConnector.Seller_Connection);
            Issues = new ObservableCollection<GoalRec>(goals);

            ChatFerm.onUnreadMessagesCount += ChatFerm_onUnreadMessagesCount;
        }



        /// <summary>
        /// Поиск ещё не прочитанных от всех аккаунтов сообщений.
        /// </summary>
        /// <param name="unreadedMsgsInfo">Краткая информация по 
        /// непрочитанным сообщениям.</param>
        private void ChatFerm_onUnreadMessagesCount(List<idxLong> unreadedMsgsInfo)
        {
            // Здесь мы сортируем по аккаунтам непрочитанные сообщения.
            foreach (var msgInfo in unreadedMsgsInfo)
            {
                foreach (var acc in Accounts)
                {
                    if (msgInfo.idx == acc.idx)
                    {
                        acc.CountNotReadedMessages = msgInfo.value;
                    }
                }
            }
            // Здесь мы находим сообщения связанные с перепиской по конкретному аккаунту.
            if (Talker == null) return;
            System.Threading.Tasks.Task.Run(() =>
            {
                System.Threading.Thread.Sleep(2000);
                if (Talker != null)
                {
                    Talker.CountNotReadedMessages = 0;
                }
            });
            string query = $"select * from chatmsg where " +
                $"(from_id = {Talker.idx} and to_id = " +
                $"{DBConnector.CurrentSintezUser.idx} and readed = 'False')";
            List<ChatMsgRec> chats = ChatFerm.getMessages(query); //dbrec.loadRange<ChatMsgRec>(
                                                                  //query, DBConnector.Sintez_Connection);
            foreach (var item in chats)
            {
                Messages.Add(item);
                item.readed = true;
                item.save();
            }
        }
    }
#endif
}
