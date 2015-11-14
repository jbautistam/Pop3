﻿using System;
using System.Text;

namespace Bau.Libraries.LibEncoder.Encoders
{
	/// <summary>
	///		Codificador / decodificador en Base64
	/// </summary>
	public class Base64Encoder : IEncoder
	{
		/// <summary>
		///		Codifica una cadena (a base 64)
		/// </summary>
		public string Encode(string strCharSet, string strSource, bool blnSubject)
    { return Encode(strCharSet, UTF8Encoding.UTF8.GetBytes(strSource), blnSubject);
    }
		
		/// <summary>
		///		Codifica un array de bytes
		/// </summary>
		public string Encode(string strCharSet, byte [] arrBytBuffer, bool blnSubject)
		{ return Convert.ToBase64String(arrBytBuffer, Base64FormattingOptions.InsertLineBreaks);
		}
		
		/// <summary>
		///		Decodifica una cadena desde Base64 a otra cadena
		/// </summary>
    public string Decode(string strCharSet, string strSource, bool blnSubject)
    { return Decode(strCharSet, DecodeToBytes(strCharSet, strSource, blnSubject), blnSubject);
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
    { return Convert.FromBase64String(strSource);
    }
    
    /// <summary>
    ///		Decodifica un array de bytes en un array de bytes
    /// </summary>
    public byte[] DecodeToBytes(string strCharSet, byte [] arrBytSource, bool blnSubject)
    { return DecodeToBytes(strCharSet, UTF8Encoding.UTF8.GetString(arrBytSource), blnSubject);
    }
	}
}