using System;
using System.Windows;
using System.Windows.Controls;

namespace Bau.Controls.Notifications
{
	/// <summary>
	///		Ventana para mostrar notificaciones
	/// </summary>
	public partial class WindowNotifications
	{   
		// Constantes privadas
		private const int Maximum = 4;
		// Variables privadas
		private readonly NotificationsModelCollection _notifications = new NotificationsModelCollection();
		private readonly NotificationsModelCollection _buffer = new NotificationsModelCollection();

		public WindowNotifications()
		{   // Inicializa los componentes
			InitializeComponent();
			// Asigna el dataContext
			NotificationsControl.DataContext = _notifications;
		}

		/// <summary>
		///		Añade una notificación
		/// </summary>
		public NotificationModel AddNotification(NotificationModel.NotificationType type, string title, string message,
												 string urlImage = null)
		{ // Asigna la Url de la imagen
			if (string.IsNullOrWhiteSpace(urlImage))
				switch (type)
				{
					case NotificationModel.NotificationType.Error:
							urlImage = "pack://application:,,,/BauControls;component/Themes/Images/Notifications/facebook-button.png";
						break;
					case NotificationModel.NotificationType.Warning:
							urlImage = "pack://application:,,,/BauControls;component/Themes/Images/Notifications/Radiation_warning_symbol.png";
						break;
					default:
							urlImage = "pack://application:,,,/BauControls;component/Themes/Images/Notifications/notification-icon.png";
						break;
				}
			// Añade la notificación
			return AddNotification(title, message, urlImage);
		}

		/// <summary>
		///		Añade una notificación
		/// </summary>
		public NotificationModel AddNotification(string title, string message, string url)
		{
			NotificationModel notification;

				// Añade la notificación
				notification = new NotificationModel { Title = title, Message = message, ImageUrl = url };
				// Añade la notificación al buffer o a la colección de notificaciones a mostrar
				if (_notifications.Count + 1 > Maximum)
					_buffer.Add(notification);
				else
					_notifications.Add(notification);
				// Muestra la ventana si hay notificaciones
				if (_notifications.Count > 0 && !IsActive)
					Show();
				// Devuelve la notificación
				return notification;
		}

		/// <summary>
		///		Elimina una notificación
		/// </summary>
		public void RemoveNotification(NotificationModel notification)
		{
			RemoveNotification(notification.ID);
		}

		/// <summary>
		///		Elimina una notificación
		/// </summary>
		public void RemoveNotification(string id)
		{   
			// Elimina la notificación
			_notifications.RemoveByID(id);
			// Si queda algo en el buffer, lo añade a la colección a mostrar
			if (_buffer.Count > 0)
			{
				_notifications.Add(_buffer [0]);
				_buffer.RemoveAt(0);
			}
			// Oculta la ventana si no queda nada para mostrar
			if (_notifications.Count < 1)
				Hide();
		}

		/// <summary>
		///		Trata el evento de cambio de tamaño
		/// </summary>
		private void NotificationWindowSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Height == 0.0)
			{
				Grid grdNotifications = sender as Grid;

					if (grdNotifications != null && grdNotifications.Tag != null)
						RemoveNotification(grdNotifications.Tag.ToString());
			}
		}
	}
}
