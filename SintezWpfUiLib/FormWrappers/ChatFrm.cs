using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SintezWpfUiLib.ViewModel;
using SintezWpfUiLib.Model;

namespace SintezWpfUiLib.FormWrappers
{
    public partial class ChatFrm : Form
    {
        public bool opened = false;

        public ChatFrm()
        {
            InitializeComponent();
        }

        public ChatModel Model
        {
            set
            {
                ((ChatViewModel)chatView.DataContext).Model = value;
            }
        }
    }
}
