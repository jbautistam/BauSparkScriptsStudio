using System;

namespace Bau.Libraries.LibDataStructures.Tools.Comparers
{
	/// <summary>
	///		Comparador de <see cref="BaseExtendelModel"/>
	/// </summary>
	internal class BaseExtendedModelComparerByName : AbstractBaseComparer<Base.BaseExtendedModel>
	{
		internal BaseExtendedModelComparerByName(bool ascending) : base(ascending) { }

		/// <summary>
		///		Compara dos elementos por nombre
		/// </summary>
		protected override int CompareData(Base.BaseExtendedModel first, Base.BaseExtendedModel second)
		{
			return (first.Name ?? "").CompareTo(second.Name ?? "");
		}
	}
}