using System;
using System.Windows.Data;

namespace Bau.Libraries.BauMvvm.Views.Converters
{
	/// <summary>
	///		Conversor inverso para valores lógicos
	/// </summary>
	[ValueConversion(typeof(bool?), typeof(bool))]
	public class InverseBooleanConverter : IValueConverter
	{
		/// <summary>
		///		Convierte un valor lógico en su inverso
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			return !((value as bool?) ?? false);
		}

		/// <summary>
		///		Convierte un valor al tipo inicial
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{	
			return !(value as bool?);
		}
	}
}
