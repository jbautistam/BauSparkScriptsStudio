using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Controllers
{
	/// <summary>
	///		Enumerados para el controlador principal del sistema
	/// </summary>
    public static class SystemControllerEnums
    {
		/// <summary>
		///		Tipo de resultado del cuadro de diálogo
		/// </summary>
		public enum ResultType
		{
			Yes,
			No,
			Cancel
		}

		/// <summary>
		///		Tipo de notificación
		/// </summary>
		public enum NotificationType
		{
			/// <summary>Información</summary>
			Information,
			/// <summary>Advertencia</summary>
			Warning,
			/// <summary>Error</summary>
			Error,
			/// <summary>Otras notificaciones</summary>
			Other
		}

		/// <summary>
		///		Tipo de pregunta 
		/// </summary>
		public enum QuestionType
		{
			/// <summary>Aceptar o cancelar</summary>
			AcceptCancel,
			/// <summary>Sí o no</summary>
			YesNo
		}
    }
}
