using System;
using System.Windows.Data;

namespace Bau.Libraries.BauMvvm.Views.Converters
{
	/// <summary>
	///		Conversor para transformar un valor booleano en un enumerado "FontStyle" (para las cursivas)
	/// </summary>
	public class BoolToFontStyleConverter : IValueConverter
	{
		/// <summary>
		///		Convierte un valor boolean en un valor de FontWeight
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			if ((value is bool) && (bool) value)
				return System.Windows.FontStyles.Italic;
			else
				return System.Windows.FontStyles.Normal;
		}

		/// <summary>
		///		Convierte un valor de vuelta
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			throw new NotImplementedException();
		}
	}
}
