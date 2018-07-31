using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Commands;
using PassManager.Models;

namespace PassManager.ViewModels
{
    public class PassManagerViewModel : BindableBase
    {
        public IEnumerable<string> MenuButtonItems
        {
            get { return MenuContent.Instance.MenuButtonItems; }
        }
        public IEnumerable<string> ContextMenuItems
        {
            get { return MenuContent.Instance.ContextMenuItems; }
        }
        public ObservableCollection<ItemOperation.RecursionItem> RecursionItems
        {
            get { return ItemOperation.Instance.RecursionItems; }
        }

        public DelegateCommand ItemAddCommand
        {
            get { return new DelegateCommand(ItemAddFunction); }
        }

        private void ItemAddFunction()
        {
        }
    }
}
