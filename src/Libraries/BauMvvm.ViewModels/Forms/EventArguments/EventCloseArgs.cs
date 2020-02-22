using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.EventArguments
{
	/// <summary>
	///		Argumentos del evento de cierre de una ventana
	/// </summary>
	public class EventCloseArgs : EventArgs
	{
		public EventCloseArgs(bool isAccepted)
		{ 
			IsAccepted = isAccepted;
		}

		/// <summary>
		///		Indica si se han aceptado los datos al cerrar la ventana
		/// </summary>
		public bool IsAccepted { get; }
	}
}
