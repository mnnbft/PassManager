using System;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Runtime.InteropServices;
using PassManager.Models;

namespace PassManager.Views.Behavior
{
    public class SecurePasswordBindingBehavior : Behavior<PasswordBox>
    {
        public SecureString SecurePassword
        {
            get { return (SecureString)GetValue(SecurePasswordProperty); }
            set { SetValue(SecurePasswordProperty, value); }
        }

        //public static readonly DependencyProperty SecurePasswordProperty =
        //    DependencyProperty.Register(
        //        nameof(SecurePassword),
        //        typeof(SecureString),
        //        typeof(SecurePasswordBindingBehavior),
        //        new PropertyMetadata(new SecureString(), SecurePasswordPropertyChanged));
        public static readonly DependencyProperty SecurePasswordProperty =
            DependencyProperty.Register(
                nameof(SecurePassword),
                typeof(SecureString),
                typeof(SecurePasswordBindingBehavior),
                new FrameworkPropertyMetadata(new SecureString(),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    SecurePasswordPropertyChanged));

        private static void SecurePasswordPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as SecurePasswordBindingBehavior;
            if (d == null) return;

            var passwordBox = behavior.AssociatedObject as PasswordBox;
            if (passwordBox == null) return;

            var newPassword = e.NewValue as SecureString;
            if (newPassword == null) return;

            var oldPassword = e.OldValue as SecureString;
            if (Functions.SecureStringEquals(newPassword, oldPassword)) return;

            var bstr = Marshal.SecureStringToBSTR(newPassword);
            var password = Marshal.PtrToStringUni(bstr);

            try
            {
                passwordBox.SecurePassword.Clear();
                foreach(var passChar in password)
                {
                    passwordBox.SecurePassword.AppendChar(passChar);
                }
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstr);
            }
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
