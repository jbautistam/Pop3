using System;

namespace Bau.Libraries.LibEncoder
{
	/// <summary>
	///		Codificación
	/// </summary>
	public static class Encoder
	{
		/// <summary>
		///		Tipo de codificación / decodificación
		/// </summary>
		public enum EncoderType
			{ Unknown,
				Base64,
				Bit7,
				Bit8,				
				Dummy,
				QuotedPrintable
			}
		
		/// <summary>
		///		Obtiene el codificador
		/// </summary>
		public static Encoders.IEncoder GetEncoder(EncoderType intEncoder)
		{ switch (intEncoder)
				{ case EncoderType.Base64:
						return new Encoders.Base64Encoder();
					case EncoderType.Bit7:
						return new Encoders.Bit7Encoder();
					case EncoderType.Bit8:
						return new Encoders.Bit8Encoder();
					case EncoderType.Dummy:
						return new Encoders.DummyEncoder();
					case EncoderType.QuotedPrintable:
						return new Encoders.QuotedPrintableEncoder();
					default:
						throw new NotImplementedException();
				}
		}
		
		/// <summary>
		///		Obtiene el tipo de codificación
		/// </summary>
		public static EncoderType GetEncoderType(string strTransferEncoding)
		{ if (string.IsNullOrEmpty(strTransferEncoding))
				return EncoderType.Dummy;
			else if (strTransferEncoding.Equals("8bit", StringComparison.CurrentCultureIgnoreCase))
				return EncoderType.Bit8;
			else if (strTransferEncoding.Equals("7bit", StringComparison.CurrentCultureIgnoreCase))
				return EncoderType.Bit7;
			else if (strTransferEncoding.Equals("QP", StringComparison.CurrentCultureIgnoreCase) ||
							 strTransferEncoding.Equals("quoted-printable", StringComparison.CurrentCultureIgnoreCase))
				return EncoderType.QuotedPrintable;
			else if (strTransferEncoding.Equals("Base64", StringComparison.CurrentCultureIgnoreCase))
				return EncoderType.Base64;
			else
				return EncoderType.Unknown;
		}
		
		/// <summary>
		///		Codifica una cadena
		/// </summary>
		public static string Encode(EncoderType intEncoder, string strSource) 
		{ return Encode(intEncoder, strSource, false);
		}
		
		/// <summary>
		///		Codifica una cadena
		/// </summary>
		public static string Encode(EncoderType intEncoder, string strSource, bool blnSubject)
		{ return GetEncoder(intEncoder).Encode(null, strSource, blnSubject);
		}
		
		/// <summary>
		///		Decodifica una cadena
		/// </summary>
		public static string Decode(EncoderType intEncoder, string strSource, bool blnSubject)
		{ return Decode(intEncoder, null, strSource, blnSubject);
		}
		
		/// <summary>
		///		Decodifica una cadena
		/// </summary>
		public static string Decode(EncoderType intEncoder, string strCharSet, string strSource, bool blnSubject)
		{ return GetEncoder(intEncoder).Decode(strCharSet, strSource, blnSubject);
		}
		
		/// <summary>
		///		Decodifica una cadena sobre un archivo
		/// </summary>
		public static void DecodeToFile(EncoderType intEncoder, string strSource, string strFileName)
		{ DecodeToFile(intEncoder, null, strSource, strFileName);
		}
		
		/// <summary>
		///		Decodifica una cadena sobre un archivo
		/// </summary>
		public static void DecodeToFile(EncoderType intEncoder, string strCharSet, string strSource, 
		                                string strFileName)
		{	Encoders.Tools.FileHelper.Save(GetEncoder(intEncoder).DecodeToBytes(strCharSet, strSource, false), 
			                               strFileName);
		}

		/// <summary>
		///		Códifica un archivo (en base 64)
		/// </summary>
		public static string EncodeFileToBase64(string strFileName)
		{	string strEncoded = null;
		
				// Codifica el contenido del archivo
					using (System.IO.FileStream fnFile = new System.IO.FileStream(strFileName, System.IO.FileMode.Open, 
																																				System.IO.FileAccess.Read))
						{ byte [] arrBytBuffer = new byte[fnFile.Length];
						
								// Lee el archivo
									fnFile.Read(arrBytBuffer, 0, arrBytBuffer.Length);
								// Obtiene la codificación del archivo
									strEncoded = System.Convert.ToBase64String(arrBytBuffer, 0, arrBytBuffer.Length);
								// Cierra el archivo
									fnFile.Close();
						}
				// Devuelve el contenido del archivo
					return strEncoded;
		}
	}
}