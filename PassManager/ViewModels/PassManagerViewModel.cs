using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using PassManager.Models;

namespace PassManager.ViewModels
{
    public class PassManagerViewModel : BindableBase
    {
        public string[] MenuButtonItems
        {
            get { return MenuContent.Instance.MenuButtonItems; }
        }
    }
}
