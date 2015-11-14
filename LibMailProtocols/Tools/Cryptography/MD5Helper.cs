using System;
using System.Security.Cryptography;
using System.Text;

namespace Bau.Libraries.LibMailProtocols.Tools.Cryptography
{
	/// <summary>
	///		Clase para codificación MD5
	/// </summary>
	internal static class MD5Helper
	{
		/// <summary>
		///		Calcula una cadena MD5 a partir de una cadena de entrada
		/// </summary>
		internal static string Compute(string strSource)
		{ byte [] arrBytTarget;
			MD5 objMD5 = new MD5CryptoServiceProvider();
			
				// Codifica la cadena
					arrBytTarget = objMD5.ComputeHash(ASCIIEncoding.Default.GetBytes(strSource));
				// Convierte los bytes codificados en una cadena legible
					return BitConverter.ToString(arrBytTarget).Replace("-", "");
		}
	}
}
