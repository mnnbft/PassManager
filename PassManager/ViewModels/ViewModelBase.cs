using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Reactive.Disposables;

namespace PassManager.ViewModels
{
    public class ViewModelBase : BindableBase, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public void Dispose()
        {
            this.Disposables.Dispose();
        }
    }
}
