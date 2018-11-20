using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using System.Windows.Threading;

namespace PassManager.Models
{
    public sealed class ClipBoard 
    {
        public static ClipBoard Instance { get; } = new ClipBoard();

        private ClipBoard() { }

        public void PasswordCopy()
        {
            if(ItemOperation.Instance.SelectedPassword == null)
            { return; }

            Clipboard.SetDataObject(ItemOperation.Instance.SelectedPassword.Password);
            MenuContent.Instance.ShowSnackMessage("クリップボードにコピーしました");

            var disposeTimer = new DispatcherTimer();
            disposeTimer.Interval = new TimeSpan(0, 0, 10);
            disposeTimer.Tick += PasswordDispose;
            disposeTimer.Start();
        }

        private void PasswordDispose(object sender, EventArgs e)
        {
            var timer = (DispatcherTimer)sender;
            timer.Stop();

            Clipboard.Clear();
            MenuContent.Instance.ShowSnackMessage("クリップボードをクリアしました");
        }
    }
}
