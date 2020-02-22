using System;

namespace Bau.Controls.Tabs
{
	/// <summary>
	///		Argumentos del evento de cierre de una ficha en un control MDI
	/// </summary>
	public class CloseTabEvntArgs : EventArgs
	{
		public CloseTabEvntArgs(System.Windows.Controls.TabItem tab, object tag)
		{
			Tab = tab;
			Tag = tag;
		}

		/// <summary>
		///		Ficha que se va a cerrar
		/// </summary>
		public System.Windows.Controls.TabItem Tab { get; }

		/// <summary>
		///		Tag de la ficha que se va a cerrar
		/// </summary>
		public object Tag { get; }

		/// <summary>
		///		Indica si se cancela el cierre de la ficha
		/// </summary>
		public bool Cancel { get; set; }
	}
}
