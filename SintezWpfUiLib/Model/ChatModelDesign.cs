#define Local

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SintezWpfUiLib.Model
{
#if !Local
    /// <summary>
    /// Модель для открытия из макета
    /// </summary>
    public class ChatModelDesign : ChatModel
    {

        public ChatModelDesign(long idDocument, long idConnectedDB) : base()
        {
            Logger.Debug("Создаём ChatModelDesign");

            IsVisibleLeftPart = System.Windows.Visibility.Collapsed;
            CurrentDoc = new DocumentRec(idDocument,
                DBConnector.DBConnectionsDictionary[idConnectedDB]);
            MessagesRelatedWithDoc(idDocument, idConnectedDB);

            Logger.Debug("Создали ChatModelDesign");
        }

        /// <summary>
        /// Поиск ещё не прочитанных от всех аккаунтов сообщений.
        /// </summary>
        /// <param name="unreadedMsgsInfo">Краткая информация по 
        /// непрочитанным сообщениям.</param>
        protected override void ChatFerm_onUnreadMessagesCount(List<idxLong> unreadedMsgsInfo)
        {
            Logger.Debug("Начинаем получать и обрабатывать информацию о ещё " +
                "не прочитанных сообщениях");
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
            // Здесь мы находим сообщения связанные с перепиской по конкретному макету.
            
            string query = $"select * from chatmsg where " +
                $"(doc_id = {CurrentDoc.idx} and doc_connection_id = " +
                $"{CurrentDoc.connection.idx} and from_id <> " +
                $"{DBConnector.CurrentSintezUser.idx} and readed = 'False')";
            List<ChatMsgRec> chats = ChatFerm.getMessages(query); //dbrec.loadRange<ChatMsgRec>(
                                                                  //query, DBConnector.Sintez_Connection);
            foreach (var item in chats)
            {
                Messages.Add(item);
                item.readed = true;
                item.save();
            }
            Logger.Debug("Закончили обработку информации о ещё " +
                "не прочитанных сообщениях");
        }
    }
#endif
}
