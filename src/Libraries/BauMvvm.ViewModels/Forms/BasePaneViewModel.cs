using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Forms
{
	/// <summary>
	///		Clase base para los ViewModel de un panel de herramientas
	/// </summary>
	public abstract class BasePaneViewModel : BaseFormViewModel, Interfaces.IPaneViewModel
	{ 
		public BasePaneViewModel(bool changeUpdated = true) : base(changeUpdated)
		{
			NewCommand = new BaseCommand(parameter => ExecuteAction(nameof(NewCommand), parameter),
										 parameter => CanExecuteAction(nameof(NewCommand), parameter));
			PropertiesCommand = new BaseCommand(parameter => ExecuteAction(nameof(PropertiesCommand), parameter),
												parameter => CanExecuteAction(nameof(PropertiesCommand), parameter));
		}

		/// <summary>
		///		Comando para un nuevo elemento
		/// </summary>
		public BaseCommand NewCommand { get; }

		/// <summary>
		///		Comando para mostrar las propiedades del elemento
		/// </summary>
		public BaseCommand PropertiesCommand { get; }
	}
}
