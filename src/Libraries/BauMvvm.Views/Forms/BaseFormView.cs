using System;

namespace Bau.Libraries.BauMvvm.Views.Forms
{
	/// <summary>
	///		Base para las vistas
	/// </summary>
	public class BaseFormView
	{ 
		public BaseFormView(ViewModels.BaseObservableObject viewModel)
		{
			ViewModel = viewModel;
		}

		/// <summary>
		///		Rutina que avisa del cierre del ViewModel por si hay que hacer alguna rutina posterior (simplemente implementa
		///	el interface y deja a los ViewModel hijo que implementen sus propias rutinas de cierre)
		/// </summary>
		public virtual void CloseViewModel()
		{
		}

		/// <summary>
		///		Datos del viewModel
		/// </summary>
		public ViewModels.BaseObservableObject ViewModel { get; }

		/// <summary>
		///		Indica si se han modificado los datos del viewModel
		/// </summary>
		public bool IsUpdated
		{
			get { return ViewModel?.IsUpdated ?? false; }
			set 
			{ 
				if (ViewModel != null)
					ViewModel.IsUpdated = value; 
			}
		}
	}
}