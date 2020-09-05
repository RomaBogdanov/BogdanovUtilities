//#define Local

//using SintezLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using LogsWrapper;

namespace SintezWpfUiLib.Model
{

#if !Local
    /// <summary>
    /// Базовый класс для обеспечения взаимодействия контрола ChatView с
    /// внешним миром.
    /// </summary>
    public class ChatModel
    {

        /// <summary>
        /// Последний запрос
        /// </summary>
        protected string lastQuery = "";
        
        /// <summary>
        /// Указывает на статус открытости левой части чата, где список аккаунтов
        /// и задач, возможно, макетов.
        /// </summary>
        public System.Windows.Visibility IsVisibleLeftPart { get; set; } 
            = System.Windows.Visibility.Visible;

        /// <summary>
        /// Список сообщений
        /// </summary>
        public virtual ObservableCollection<ChatMsgRec> Messages { get; set; }

        /// <summary>
        /// Список аккаунтов
        /// </summary>
        public virtual ObservableCollection<SintezUserRec> Accounts { get; set; }

        /// <summary>
        /// Список задач
        /// </summary>
        public virtual ObservableCollection<GoalRec> Issues { get; set; }

        /// <summary>
        /// Список макетов
        /// </summary>
        public virtual ObservableCollection<DocumentRec> Designes { get; set; }

        /// <summary>
        /// Тот, от имени кого заходят в чат
        /// </summary>
        public virtual SintezUserRec Manager { get; set; }

        /// <summary>
        /// Собеседники, с которым идёт диалог
        /// </summary>
        public virtual SintezUserRec Talker { get; set; }

        /// <summary>
        /// Текущий макет, к которому привязано сообщение.
        /// </summary>
        public virtual DocumentRec CurrentDoc { get; set; }

        public ChatModel()
        {
            Logger.Debug("Создаём ChatModel");
            Messages = new ObservableCollection<ChatMsgRec>();
            Accounts = new ObservableCollection<SintezUserRec>();
            Issues = new ObservableCollection<GoalRec>();
            Designes = new ObservableCollection<DocumentRec>();

            Manager = new SintezUserRec
            {
                idx = 1,
                CountNotReadedMessages = 0,
                login = "Вася"
            };
            //List<SintezUserRec> users = dbrec.loadRange<SintezUserRec>(
            //    "select * from sintezuser", DBConnector.Sintez_Connection);
            //Accounts = new ObservableCollection<SintezUserRec>(users);
            //Accounts.Remove(Manager);
            /*List<GoalRec> goals = dbrec.loadRange<GoalRec>(
                "select * from goals where closetype=0", DBConnector.Seller_Connection);
            Issues = new ObservableCollection<GoalRec>(goals);*/

            //ChatFerm.onUnreadMessagesCount += ChatFerm_onUnreadMessagesCount;
            Logger.Debug("Создали ChatModel");
        }

        /// <summary>
        /// Процедура отправления сообщения.
        /// TODO: ключевой метод. Попробовать порефакторить его.
        /// </summary>
        /// <param name="message">Сообщение для отправки</param>
        /// <returns>true - сообщение отправлено успешно, 
        /// false - сообщение отправлено неуспешно.</returns>
        public virtual bool SendMessage(SintezUserRec toAccount,
            string message, bool isAttentionMessage, string file = "")
        {
            /*Находим в сообщении пометки, указывающие на аккаунт назначения, 
             задачу или макет*/

            string design = Regex.Match(message, @"(?<=\%\"").*?(?=\"")").Value;
            message = message.Replace($@"%""{design}""", "");
            string pointedAccount = Regex.Match(message, @"(?<=\$\"").*?(?=\"")").Value;
            message = message.Replace($@"$""{pointedAccount}""", "");
            string issue = Regex.Match(message, @"(?<=\#\"").*?(?=\"")").Value;
            message = message.Replace($@"#""{issue}""", "");

            // Формируем и сохраняем сообщение
            ChatMsgRec sendedMessage = new ChatMsgRec()
            {
                //from_id = DBConnector.CurrentSintezUser.idx,
                //message = message
            };
            if (pointedAccount != "")
            {
                sendedMessage.to_id = Convert.ToInt64(Regex.Match(pointedAccount, 
                    @"(?<=\()\d*(?=\)$)").Value);
            }
            if (toAccount != null)
            {
                sendedMessage.to_id = toAccount.idx;
            }
            if (issue != "")
            {
                sendedMessage.goal_id = Convert.ToInt64(Regex.Match(issue, 
                    @"(?<=\()\d*(?=\)$)").Value);
            }
            // TODO: сдесь потенциальная ошибка, т.к. мы может задать несколько 
            // макетов и будут проблемы.
            if (design != "")
            {
                // TODO: - добавить привязку к базе данных, в которой находится 
                // таблица с макетом
                string s = Regex.Match(design, @"(?<=\()\d*\|\d*(?=\))").Value;
                string[] s1 = s.Split('|');
                sendedMessage.doc_id = Convert.ToInt64(s1[0]);
                sendedMessage.doc_connection_id = Convert.ToInt64(s1[1]);
            }
            else if (CurrentDoc != null)
            {
                sendedMessage.doc_id = CurrentDoc.baseid;
                sendedMessage.doc_connection_id = CurrentDoc.connection.connector_id;//.ConnectionId;
            }
            if (!string.IsNullOrEmpty(file))
            {
                sendedMessage.filename = (new System.IO.FileInfo(file)).Name;
            }
            if (sendedMessage.to_id == 0)
            {
                Logger.MsgBox("Введите аккаунт, которому будет адресовано сообщение");
                return false;
            }
            sendedMessage.attention = isAttentionMessage;
            Messages.Add(sendedMessage);
            //sendedMessage.save();
            ChatFerm.sendMessage(sendedMessage);

            // загружаем файл
            if (!string.IsNullOrEmpty(file))
            {
                sendedMessage.LoadContent(file);
            }
            return true;
        }

        /// <summary>
        /// Процедура поиска сообщений привязанных к аккаунту.
        /// </summary>
        /// <param name="userRec"></param>
        public virtual void MessagesRelatedWithAccount(SintezUserRec userRec)
        {
            lastQuery = $"select * from chatmsg where " +
                $"(from_id = {userRec.idx} and to_id = {DBConnector.CurrentSintezUser.idx}) or " +
                $"(to_id = {userRec.idx} and from_id = {DBConnector.CurrentSintezUser.idx})";
            //List<ChatMsgRec> chats = dbrec.loadRange<ChatMsgRec>(
            //    lastQuery, DBConnector.Sintez_Connection);
            Messages = new ObservableCollection<ChatMsgRec>();//(chats);
            foreach (var item in Messages)
            {
                if (item.readed == false && item.to_id == Manager.idx)
                {
                    item.readed = true;
                    item.save();
                }
            }
        }

        /// <summary>
        /// Процедура поиска сообщений привязанных к задаче.
        /// </summary>
        /// <param name="goalRec"></param>
        public virtual void MessagesRelatedWithIssue(GoalRec goalRec)
        {
            //lastQuery = $"select * from chatmsg where " +
            //            $"goal_id = {goalRec.idx}";
            //List<ChatMsgRec> chats = dbrec.loadRange<ChatMsgRec>(
            //    lastQuery, DBConnector.Sintez_Connection);
            Messages = new ObservableCollection<ChatMsgRec>();//(chats);
        }

        /// <summary>
        /// Процедура поиска сообщений привязанных к макету.
        /// </summary>
        /// <param name="documentRec"></param>
        public virtual void MessagesRelatedWithDoc(DocumentRec documentRec)
        {
            MessagesRelatedWithDoc(documentRec.idx, documentRec.connection.connector_id);
        }

        public virtual void MessagesRelatedWithDoc(long idDocRec, long idConnect)
        {
            lastQuery = $"select * from chatmsg where " +
                    $"doc_id = {idDocRec} and doc_connection_id = {idConnect}";
            //List<ChatMsgRec> chats = dbrec.loadRange<ChatMsgRec>(
            //    lastQuery, DBConnector.Sintez_Connection);
            Messages = new ObservableCollection<ChatMsgRec>();// (chats);
        }

        /// <summary>
        /// Поиск ещё не прочитанных от всех аккаунтов сообщений.
        /// </summary>
        /// <param name="unreadedMsgsInfo">Краткая информация по 
        /// непрочитанным сообщениям.</param>
        protected virtual void ChatFerm_onUnreadMessagesCount(List<idxLong> unreadedMsgsInfo)
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
            // Здесь мы находим сообщения связанные с перепиской по конкретному аккаунту.
            if (Talker == null) return;
            System.Threading.Tasks.Task.Run(() =>
            {
                System.Threading.Thread.Sleep(2000);
                if (Talker!=null)
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
            Logger.Debug("Закончили обработку информации о ещё " +
                "не прочитанных сообщениях");
        }

    }
#endif
}
