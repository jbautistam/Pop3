using System;

namespace Bau.Libraries.LibEncoder.Encoders
{
	/// <summary>
	///		Interface que deben cumplir las clases de codificación
	/// </summary>
	public interface IEncoder
	{
		/// <summary>
		///		Codifica una cadena 
		/// </summary>
		string Encode(string strCharSet, string strSource, bool blnSubject);
		
		/// <summary>
		///		Codifica un array de bytes
		/// </summary>
		string Encode(string strCharSet, byte [] arrBytSource, bool blnSuject);
		
		/// <summary>
		///		Decodifica una cadena
		/// </summary>
    string Decode(string strCharSet, string strSource, bool blnSubject);
		
		/// <summary>
		///		Decodifica un array de bytes en una cadena
		/// </summary>
    string Decode(string strCharSet, byte [] arrBytSource, bool blnSubject);
    
    /// <summary>
    ///		Decodifica en un array de bytes una cadena
    /// </summary>
		byte [] DecodeToBytes(string strCharSet, string strSource, bool blnSubject);
    
		/// <summary>
		///		Decodifica en un array de bytes otro array de bytes
		/// </summary>
		byte [] DecodeToBytes(string strCharSet, byte [] arrBytSource, bool blnSubject);
	}
}