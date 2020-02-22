using System;
using System.Collections.ObjectModel;

namespace Bau.Controls.Notifications
{
	/// <summary>
	///		Colección de <see cref="NotificationModel"/>
	/// </summary>
	public class NotificationsModelCollection : ObservableCollection<NotificationModel>
	{
		/// <summary>
		///		Elimina un elemento por su ID
		/// </summary>
		public void RemoveByID(string id)
		{
			for (int index = Count - 1; index >= 0; index--)
				if (this [index].ID == id)
					RemoveAt(index);
		}
	}
}
