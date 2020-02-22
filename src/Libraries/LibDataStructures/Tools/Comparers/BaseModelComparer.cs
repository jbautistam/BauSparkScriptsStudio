using System;

namespace Bau.Libraries.LibDataStructures.Tools.Comparers
{
	/// <summary>
	///		Comparador de <see cref="CallModel"/>
	/// </summary>
	internal class BaseModelComparer : AbstractBaseComparer<Base.BaseModel>
	{
		internal BaseModelComparer(bool ascending) : base(ascending) { }

		/// <summary>
		///		Compara dos elementos por ID
		/// </summary>
		protected override int CompareData(Base.BaseModel first, Base.BaseModel second)
		{
			return (first.ID ?? 0).CompareTo(second.ID ?? 0);
		}
	}
}