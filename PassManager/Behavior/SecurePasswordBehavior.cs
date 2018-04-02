using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Behavior
{
    using System.Security;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    public class SecurePasswordBehavior : Behavior<PasswordBox>
    {
        public static readonly DependencyProperty SecurePasswordProperty =
            DependencyProperty.Register("SecurePassword",
                typeof(SecureString),
                typeof(SecurePasswordBehavior),
                new PropertyMetadata(new SecureString(), null));

        public SecureString SecurePassword
        {
            get { return (SecureString)GetValue(SecurePasswordProperty); }
            set { SetValue(SecurePasswordProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += PasswordBox_PasswordChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= PasswordBox_PasswordChanged;
            base.OnDetaching();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            SecurePassword = AssociatedObject.SecurePassword.Copy();
        }
    }

}
