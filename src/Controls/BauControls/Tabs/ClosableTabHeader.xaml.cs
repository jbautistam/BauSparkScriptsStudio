using System;
using System.Windows.Controls;

namespace Bau.Controls.Tabs
{
	/// <summary>
	///		Cabecera para un elemento de un control tab para que se puede cerrar
	/// </summary>
	public partial class ClosableTabHeader : UserControl
	{
		// Eventos
		internal event EventHandler Close;

		public ClosableTabHeader()
		{
			InitializeComponent();
		}

		/// <summary>
		///		Lanza el evento de cerrar
		/// </summary>
		private void RaiseEventClose()
		{
			Close?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header 
		{
			get { return lblHeader.Content?.ToString(); }
			set { lblHeader.Content = value; }
		}

		private void cmdClose_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			RaiseEventClose();
		}
	}
}
