using System;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;

namespace IL2CDR.Model
{
	public static class UI
	{
		public static void Dispatch(Action action)
		{
			DispatcherHelper.CheckBeginInvokeOnUI(action);
		}

		public static T DispatchFunc<T>(Func<T> func) where T : class
		{
			if (func == null) {
				return null;
			}

			return DispatcherHelper.UIDispatcher.Invoke(func);
		}

		public static Dispatcher UIDispatcher => DispatcherHelper.UIDispatcher;
	}
}