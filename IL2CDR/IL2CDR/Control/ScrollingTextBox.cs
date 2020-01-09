using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IL2CDR.Control
{
	public class ScrollingTextBox : TextBox
	{
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
		}

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			base.OnTextChanged(e);
			this.CaretIndex = this.Text.Length;
			this.ScrollToEnd();
		}
	}
}