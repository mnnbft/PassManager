using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security;

namespace PassManager.Views
{
    /// <summary>
    /// PasswordGenerateDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class PasswordGenerateDialog : UserControl
    {
        public PasswordGenerateDialog()
        {
            InitializeComponent();
        }
        
        public SecureString GeneratedPassword
        {
            get { return (SecureString)GetValue(GeneratedPasswordProperty); }
            set { SetValue(GeneratedPasswordProperty, value); }
        }
        public static readonly DependencyProperty GeneratedPasswordProperty =
            DependencyProperty.Register(
                nameof(GeneratedPassword),
                typeof(SecureString),
                typeof(PasswordGenerateDialog),
                new PropertyMetadata(new SecureString()));
    }
}
