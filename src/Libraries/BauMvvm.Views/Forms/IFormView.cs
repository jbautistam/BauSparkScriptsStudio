using System;

namespace Bau.Libraries.BauMvvm.Views.Forms
{
	/// <summary>
	///		Interface para las vistas de formulario
	/// </summary>
	public interface IFormView
	{
		/// <summary>
		///		ViewModel del formulario
		/// </summary>
		BaseFormView FormView { get; }
	}
}
