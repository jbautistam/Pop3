using System;
using System.Text;

namespace Bau.Libraries.LibEncoder.Encoders
{
	/// <summary>
	///		Codificador / decodificador en 8Bit
	/// </summary>
	public class DummyEncoder : IEncoder
	{
		/// <summary>
		///		Codifica una cadena (a base 64)
		/// </summary>
		public string Encode(string strCharSet, string strSource, bool blnSubject)
    { return strSource;
    }
		
		/// <summary>
		///		Codifica un array de bytes
		/// </summary>
		public string Encode(string strCharSet, byte [] arrBytBuffer, bool blnSubject)
		{ return Convert.ToString(arrBytBuffer);
		}
		
		/// <summary>
		///		Decodifica una cadena desde Base64 a otra cadena
		/// </summary>
    public string Decode(string strCharSet, string strSource, bool blnSubject)
    { return strSource;
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
    { return ASCIIEncoding.ASCII.GetBytes(strSource);
    }
    
    /// <summary>
    ///		Decodifica un array de bytes en un array de bytes
    /// </summary>
    public byte[] DecodeToBytes(string strCharSet, byte [] arrBytSource, bool blnSubject)
    { return arrBytSource;
    }
	}
}