using System;

namespace Bau.Libraries.LibMailProtocols.Messages.Parser 
{
	/// <summary>
	///		Helper para interpretación
	/// </summary>
	internal static class MimeParserHelper 
	{
    /// <summary>
    ///		Trim de una cadena
    /// </summary>
    internal static string Trim(string strValue)
    { if (!string.IsNullOrEmpty(strValue))
				return strValue.Trim();
			else
				return strValue;
    }
    
		/// <summary>
		///		Obtiene la fecha a partir de una cadena
		/// </summary>
		internal static DateTime GetDate(string strDate)
		{ DateTime dtmValue;

				// Interpreta la fecha, si no es una fecha correcta devuelve la fecha mínima
					if (!DateTime.TryParse(strDate, out dtmValue))
						dtmValue = DateTime.MinValue;
				// Devuelve la fecha
					return dtmValue;
		}
	}
}
