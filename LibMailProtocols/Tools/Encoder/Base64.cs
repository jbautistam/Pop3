using System;

namespace Bau.Libraries.LibMailProtocols.Tools.Encoder
{
	/// <summary>
	///		Codificador / decodificador de Base64
	/// </summary>
	internal static class Base64
	{
		/// <summary>
		///		Codifica una cadena
		/// </summary>
    public static byte[] ConvertFromBase64String(string strBase64)
    { // Quita los saltos de línea
        strBase64 = strBase64.Trim(new char[] {'\r','\n'});
        strBase64 = strBase64.Replace("\r\n", "");
      // Devuelve el array de bytes codificado
        return Convert.FromBase64String(strBase64);
    }
	}
}