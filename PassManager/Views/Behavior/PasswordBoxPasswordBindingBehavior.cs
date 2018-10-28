using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PassManager.Views.Behavior
{
    public class PasswordBoxPasswordBindingBehavior
    {
        public static readonly DependencyProperty IsAttachedProperty =
            DependencyProperty.RegisterAttached(
                "IsAttached",
                typeof(bool),
                typeof(PasswordBoxPasswordBindingBehavior),
                new FrameworkPropertyMetadata(
                    false,
                    IsAttachecProperty_Changed));

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordBoxPasswordBindingBehavior),
                new FrameworkPropertyMetadata(
                    "",
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    PasswordProperty_Changed));

        public static bool GetIsAttached(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsAttachedProperty);
        }

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetIsAttached(DependencyObject dp, bool value)
        {
            dp.SetValue(IsAttachedProperty, value);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        public static void IsAttachecProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordProperty_Changed;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordProperty_Changed;
            }
        }

        public static void PasswordProperty_Changed(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            SetPassword(passwordBox, passwordBox.Password);
        }

        private static void PasswordProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var newPassword = (string)e.NewValue;

            if (!GetIsAttached(passwordBox))
            {
                SetIsAttached(passwordBox, true);
            }

            if((string.IsNullOrEmpty(passwordBox.Password) && string.IsNullOrEmpty(newPassword)) ||
                passwordBox.Password == newPassword)
            {
                return;
            }

            passwordBox.PasswordChanged -= PasswordProperty_Changed;
            passwordBox.Password = newPassword;
            passwordBox.PasswordChanged += PasswordProperty_Changed;
        }
    }
}
