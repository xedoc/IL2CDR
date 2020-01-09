using System.Windows;
using IL2CDR.ViewModel;
using MahApps.Metro.Controls;

namespace IL2CDR
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		/// <summary>
		/// Initializes a new instance of the MainWindow class.
		/// </summary>
		public MainWindow()
		{
			this.InitializeComponent();
			this.Closing += (s, e) => ViewModelLocator.Cleanup();
		}
	}
}