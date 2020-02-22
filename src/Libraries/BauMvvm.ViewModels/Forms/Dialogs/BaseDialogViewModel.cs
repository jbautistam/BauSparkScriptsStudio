using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms.Dialogs
{
	/// <summary>
	///		Clase base para los ViewModel para ventanas de diálogo básicas
	/// </summary>
	public abstract class BaseDialogViewModel : BaseObservableObject
	{ 
		// Eventos públicos
		public event EventHandler<EventArguments.EventCloseArgs> Close;

		protected BaseDialogViewModel()
		{
			SaveCommand = new BaseCommand(parameter => Save());
		}

		/// <summary>
		///		Graba los datos
		/// </summary>
		protected abstract void Save();

		/// <summary>
		///		Lanza el evento <see cref="Close"/>
		/// </summary>
		protected void RaiseEventClose(bool isAccepted)
		{
			Close?.Invoke(this, new EventArguments.EventCloseArgs(isAccepted));
		}

		/// <summary>
		///		Comando de grabación
		/// </summary>
		public BaseCommand SaveCommand { get; }
	}
}
