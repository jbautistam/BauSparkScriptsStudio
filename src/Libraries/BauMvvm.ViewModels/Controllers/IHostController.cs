using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Controllers
{
    /// <summary>
    ///     Controlador de vistas principal
    /// </summary>
    public interface IHostController
    {
		/// <summary>
		///		Controlador de diálogos de sistema
		/// </summary>
		IHostDialogsController DialogsController { get; }

        /// <summary>
        ///     Controlador con funciones del sistema
        /// </summary>
        IHostSystemController SystemController { get; }

        /// <summary>
        ///     Controlador con funciones del sistema asíncrono
        /// </summary>
        IHostSystemControllerAsync SystemControllerAsync { get; }

        /// <summary>
        ///     Controlador de mensajes
        /// </summary>
        IHostMessengerController MessengerController { get; }
    }
}
