using System;
using System.ComponentModel;

namespace Bau.Controls.Notifications
{
	/// <summary>
	///		Clase con los datos de una notificación
	/// </summary>
	public class NotificationModel : INotifyPropertyChanged
	{   // Eventos públicos
		public event PropertyChangedEventHandler PropertyChanged;
		// Enumerados públicos
		/// <summary>
		///		Tipo de notificación
		/// </summary>
		public enum NotificationType
		{
			Error,
			Warning,
			Info,
			Other
		}
		// Variables privadas
		private NotificationType _type;
		private string _id, _message, _imageUrl, _title;

		/// <summary>
		///		Tratamiento del evento PropertyChanged
		/// </summary>
		protected virtual void OnPropertyChanged(string property)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}

		/// <summary>
		///		ID de la notificación
		/// </summary>
		public string ID
		{
			get
			{	// Obtiene el ID
				if (string.IsNullOrEmpty(_id))
					_id = Guid.NewGuid().ToString();
				// Devuelve el ID
				return _id;
			}
			set
			{
				if (_id != value)
				{
					_id = value;
					OnPropertyChanged(nameof(ID));
				}
			}
		}

		/// <summary>
		///		Tipo de notificación
		/// </summary>
		public NotificationType IDType
		{
			get { return _type; }
			set
			{
				if (_type != value)
				{
					_type = value;
					OnPropertyChanged(nameof(IDType));
				}
			}
		}

		/// <summary>
		///		Título de la notificación
		/// </summary>
		public string Title
		{
			get { return _title; }
			set
			{
				if (_title != value)
				{
					_title = value;
					OnPropertyChanged(nameof(Title));
				}
			}
		}

		/// <summary>
		///		Mensaje de la notificación
		/// </summary>
		public string Message
		{
			get { return _message; }
			set
			{
				if (_message != value)
				{
					_message = value;
					OnPropertyChanged(nameof(Message));
				}
			}
		}

		/// <summary>
		///		Url de la imagen
		/// </summary>
		public string ImageUrl
		{
			get { return _imageUrl; }
			set
			{
				if (_imageUrl != value)
				{
					_imageUrl = value;
					OnPropertyChanged(nameof(ImageUrl));
				}
			}
		}
	}
}