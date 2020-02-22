﻿using System;
using System.Windows.Data;

namespace Bau.Libraries.BauMvvm.Views.Converters
{
	/// <summary>
	///		Conversor para transformar un valor booleano en un enumerado "Visibility"
	/// </summary>
	public class BoolToVisibilityConverter : IValueConverter
	{
		/// <summary>
		///		Convierte un valor boolean en un valor de FontWeight
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			if ((value is bool) && (bool) value)
				return System.Windows.Visibility.Visible;
			else
				return System.Windows.Visibility.Collapsed;
		}

		/// <summary>
		///		Convierte un valor de FontWeight en un boolean
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			throw new NotImplementedException();
		}
	}
}
