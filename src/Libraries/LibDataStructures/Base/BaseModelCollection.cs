using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Bau.Libraries.LibDataStructures.Base
{
	/// <summary>
	///		Colección de <see cref="BaseModel"/>
	/// </summary>
	public class BaseModelCollection<TypeData> : List<TypeData> where TypeData : BaseModel
	{
		/// <summary>
		///		Busca un elemento por su clave numérica
		/// </summary>
		public virtual TypeData Search(int? id)
		{ 
			return this.FirstOrDefault<TypeData>(item => item.ID == id);
		}

		/// <summary>
		///		Busca un elemento
		/// </summary>
		public virtual TypeData Search(string id)
		{
			return this.FirstOrDefault(item => (item.GlobalId ?? "").Equals(id, StringComparison.CurrentCultureIgnoreCase));
		}

		/// <summary>
		///		Busca un elemento
		/// </summary>
		public TypeData Search(TypeData item)
		{ 
			return this.FirstOrDefault(data => data.ID == item.ID);
		}

		/// <summary>
		///		Comprueba si existe un dato
		/// </summary>
		public virtual bool Exists(int? id)
		{ 
			return Search(id) != null;
		}

		/// <summary>
		///		Comprueba si existe un dato
		/// </summary>
		public virtual bool Exists(string id)
		{ 
			return Search(id) != null;
		}
		
		/// <summary>
		///		Elimina un elemento de la colección
		/// </summary>
		public void RemoveByID(int? id)
		{ 
			for (int index = Count - 1; index >= 0; index--)
				if (this[index].ID == id)
					RemoveAt(index);
		}

		/// <summary>
		///		Elimina un elemento por su ID
		/// </summary>
		public void RemoveByID(string id)
		{
			TypeData item = Search(id);

				// Borra el elemento
				if (item != null)
					Remove(item);
		}

		/// <summary>
		///		Convierte la lista a un <see cref="ObservableCollection"/>
		/// </summary>
		public ObservableCollection<TypeData> ToObservableCollection()
		{ 
			ObservableCollection<TypeData> observable = new ObservableCollection<TypeData>();

				// Traspasa la colección
				foreach (TypeData item in this)
					observable.Add(item);
				// Devuelve la colección
				return observable;
		}
	}
}
