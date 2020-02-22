using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibDataStructures.Tools.Comparers
{
	/// <summary>
	///		Clase base para los comparadores
	/// </summary>
	public abstract class AbstractBaseComparer<TypeData> : IComparer<TypeData> where TypeData : Base.BaseModel
	{
		public AbstractBaseComparer(bool ascending)
		{
			IsAscending = ascending;
		}

		/// <summary>
		///		Compara dos elementos
		/// </summary>
		public int Compare(TypeData x, TypeData y)
		{
			if (x == null && y == null)
				return 0;
			else
				return GetSortFactor() * CompareData(x, y);
		}

		/// <summary>
		///		Compara dos elementos
		/// </summary>
		protected abstract int CompareData(TypeData first, TypeData second);

		/// <summary>
		///		Factor para la ordenación ascendente / descendente
		/// </summary>
		private int GetSortFactor()
		{
			if (IsAscending)
				return 1;
			else
				return -1;
		}

		/// <summary>
		///		Indica si la ordenación es ascendente / descendente
		/// </summary>
		private bool IsAscending { get; set; }
	}
}
