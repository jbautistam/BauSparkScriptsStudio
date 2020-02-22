using System;

namespace Bau.Libraries.BauMvvm.ViewModels.Controllers
{
	/// <summary>
	///		Controlador para mensajería
	/// </summary>
    public interface IHostMessengerController
    {
        /// <summary>
        ///     Suscribe una acción a un evento
        /// </summary>
        void Subscribe<TSender, TArgs>(object subscriber, string message, 
                                        Action<TSender, TArgs> callback, TSender source = null) where TSender : class;

        /// <summary>
        ///     Envía un evento
        /// </summary>
        void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class;
    }
}
