//using SintezLibrary;
using System;
using System.Windows;

namespace SintezWpfUiLib.Model
{
    public class ChatMsgRec
    {
        public long from_id { get; internal set; }
        public long doc_id { get; internal set; }
        public long to_id { get; internal set; }
        public long goal_id { get; internal set; }
        public long doc_connection_id { get; internal set; }
        public string filename { get; internal set; }
        public bool attention { get; internal set; }
        public bool readed { get; internal set; }

        public void saveToFile()
        {
            MessageBox.Show("Заглушка для процедуры сохранения в файл");
        }

        public void openFile()
        {
            MessageBox.Show("Заглушка для процедуры открытия файлов");
        }

        internal void LoadContent(string file)
        {
            MessageBox.Show("Заглушка для процедуры загрузки");
        }

        internal void save()
        {
            MessageBox.Show("Заглушка для процедуры сохранения");
        }
    }
}