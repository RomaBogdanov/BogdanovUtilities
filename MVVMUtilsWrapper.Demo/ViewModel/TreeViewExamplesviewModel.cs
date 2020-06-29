using BogdanovUtilitisLib.MVVMUtilsWrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMUtilsWrapper.Demo.ViewModel
{
    class TreeViewExamplesviewModel : NotifyPropertyChanged
    {
        private ObservableCollection<Node> nodes;

        public ObservableCollection<Node> Nodes
        {
            get => nodes; set
            {
                nodes = value;
                OnPropertyChanged();
            }
        }

        public TreeViewExamplesviewModel()
        {
            var nodes = new ObservableCollection<Node>();
            nodes.Add(new Node
            {
                Name = "First",
                Children = new ObservableCollection<Node>
                {
                    new Node{ Name="FirstSec", Children = new ObservableCollection<Node>()},
                    new Node
                    {
                        Name="HAHA",
                        Children = new ObservableCollection<Node>{
                            new Node{ Name="TRRR"}
                    } }
                }
            });
            Nodes = nodes;
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public ObservableCollection<Node> Children { get; set; }
    }
}
