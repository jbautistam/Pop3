using System;
using System.Text;

namespace Bau.Libraries.LibEncoder.Encoders
{
	/// <summary>
	///		Codificador / decodificador en 8Bit
	/// </summary>
	public class Bit8Encoder : IEncoder
	{
		/// <summary>
		///		Codifica una cadena (a base 64)
		/// </summary>
		public string Encode(string strCharSet, string strSource, bool blnSubject)
    { return Encode(strCharSet, Encoding.ASCII.GetBytes(strSource), blnSubject);
    }
		
		/// <summary>
		///		Codifica un array de bytes
		/// </summary>
		public string Encode(string strCharSet, byte [] arrBytSource, bool blnSubject)
		{	byte[] arrBytAscii = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, arrBytSource);
      char[] arrChrAscii = new char[Encoding.ASCII.GetCharCount(arrBytAscii, 0, arrBytAscii.Length)];
     
				// Obtiene los caracteres de la codificación
					Encoding.ASCII.GetChars(arrBytAscii, 0, arrBytAscii.Length, arrChrAscii, 0);
				// Devuelve la cadena a partir del array de caracteres
					return new string(arrChrAscii);
		}
		
		/// <summary>
		///		Decodifica una cadena desde Base64 a otra cadena
		/// </summary>
    public string Decode(string strCharSet, string strSource, bool blnSubject)
    { return UTF8Encoding.UTF8.GetString(ASCIIEncoding.ASCII.GetBytes(strSource));
    }
    
    /// <summary>
    ///		Decodificar una cadena desde un array de bytes
    /// </summary>
    public string Decode(string strCharSet, byte [] arrBytSource, bool blnSubject)
    { return UTF8Encoding.UTF8.GetString(arrBytSource);
    }
    
    /// <summary>
    ///		Decodifica una cadena en base 64 a un array de bytes
    /// </summary>
    public byte[] DecodeToBytes(string strCharSet, string strSource, bool blnSubject)
    { return DecodeToBytes(strCharSet, ASCIIEncoding.ASCII.GetBytes(strSource), blnSubject);
    }
    
    /// <summary>
    ///		Decodifica un array de bytes en un array de bytes
    /// </summary>
    public byte[] DecodeToBytes(string strCharSet, byte [] arrBytSource, bool blnSubject)
    { return ASCIIEncoding.Convert(Encoding.UTF8, Encoding.UTF8, arrBytSource);
    }
	}
}