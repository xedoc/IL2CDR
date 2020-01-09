﻿using System.Windows;
using System.Windows.Controls;

namespace IL2CDR.Interactivity
{
	public static class PasswordBoxAssistant
	{
		public static readonly DependencyProperty BoundPassword =
			DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxAssistant),
				new PropertyMetadata(null, OnBoundPasswordChanged));

		private static readonly DependencyProperty UpdatingPassword =
			DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxAssistant),
				new PropertyMetadata(false));

		private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var box = d as PasswordBox;

			// only handle this event when the property is attached to a PasswordBox
			// and when the BindPassword attached property has been set to true
			if (d == null) {
				return;
			}

			// avoid recursive updating by ignoring the box's changed event
			box.PasswordChanged -= HandlePasswordChanged;

			var newPassword = (string) e.NewValue;

			if (!GetUpdatingPassword(box)) {
				box.Password = newPassword;
			}

			box.PasswordChanged += HandlePasswordChanged;
		}

		private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
		{
			var box = sender as PasswordBox;

			// set a flag to indicate that we're updating the password
			SetUpdatingPassword(box, true);
			// push the new password into the BoundPassword property
			if (box != null) {
				SetBoundPassword(box, box.Password);
				SetUpdatingPassword(box, false);
			}
		}

		public static string GetBoundPassword(DependencyObject dp)
		{
			return (string) dp.GetValue(BoundPassword);
		}

		public static void SetBoundPassword(DependencyObject dp, string value)
		{
			dp.SetValue(BoundPassword, value);
		}

		private static bool GetUpdatingPassword(DependencyObject dp)
		{
			return (bool) dp.GetValue(UpdatingPassword);
		}

		private static void SetUpdatingPassword(DependencyObject dp, bool value)
		{
			dp.SetValue(UpdatingPassword, value);
		}
	}
}