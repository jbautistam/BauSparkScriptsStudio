using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms
{
	/// <summary>
	///		ViewModel para las clases de ventana
	/// </summary>
	public abstract class BaseFormViewModel : BaseObservableObject, Interfaces.IFormViewModel
	{
		// Eventos públicos
		public event EventHandler<EventArguments.EventCloseArgs> RequestClose;

		public BaseFormViewModel(bool changeUpdated = true) : base(changeUpdated)
		{
			//ViewModelData = viewModel;
			CloseCommand = new BaseCommand("Cerrar", parameter => Close(SystemControllerEnums.ResultType.No));
			RefreshCommand = new BaseCommand("Actualizar",
											 parameter => ExecuteAction(nameof(RefreshCommand), parameter),
											 parameter => CanExecuteAction(nameof(RefreshCommand), parameter));
			SaveCommand = new BaseCommand("Guardar",
										  parameter => ExecuteAction(nameof(SaveCommand), parameter),
										  parameter => CanExecuteAction(nameof(SaveCommand), parameter));
			DeleteCommand = new BaseCommand("Borrar",
											parameter => ExecuteAction(nameof(DeleteCommand), parameter),
											parameter => CanExecuteAction(nameof(DeleteCommand), parameter));
		}

		/// <summary>
		///		Ejecuta una acción
		/// </summary>
		protected abstract void ExecuteAction(string action, object parameter);

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected abstract bool CanExecuteAction(string action, object parameter);

		/// <summary>
		///		Llama al evento de cierre de ventana
		/// </summary>
		public virtual void Close(SystemControllerEnums.ResultType result)
		{
			RequestClose?.Invoke(this, new EventArguments.EventCloseArgs(result == SystemControllerEnums.ResultType.Yes));
		}

		/// <summary>
		///		Menús
		/// </summary>
		public ControlItems.Menus.MenuComposition MenuCompositionData { get; } = new ControlItems.Menus.MenuComposition();

		/// <summary>
		///		Comando de cierre de la ventana
		/// </summary>
		public BaseCommand CloseCommand { get; }

		/// <summary>
		///		Comando para actualización
		/// </summary>
		public BaseCommand RefreshCommand { get; }

		/// <summary>
		///		Comando para grabación
		/// </summary>
		public BaseCommand SaveCommand { get; }

		/// <summary>
		///		Comando para borrado
		/// </summary>
		public BaseCommand DeleteCommand { get; }
	}
}
