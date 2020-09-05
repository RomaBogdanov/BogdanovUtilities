using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
//using SintezLibrary;

namespace SintezWpfUiLib.Model
{
    public class TestWithoutDataChatModel : ChatModel
    {
        public TestWithoutDataChatModel()
        {
            Messages = new ObservableCollection<ChatMsgRec>();
            Accounts = new ObservableCollection<SintezUserRec>();
            Issues = new ObservableCollection<GoalRec>();
            Designes = new ObservableCollection<DocumentRec>();
        }
    }
}
