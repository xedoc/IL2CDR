using System;

namespace IL2CDR.Model
{
	public class WindowWrapper : System.Windows.Forms.IWin32Window
	{
		public WindowWrapper(IntPtr handle)
		{
			this.Handle = handle;
		}

		public IntPtr Handle { get; }
	}
}