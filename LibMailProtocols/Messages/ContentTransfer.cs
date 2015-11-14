using System;

namespace Bau.Libraries.LibMailProtocols.Messages
{
	/// <summary>
	///		Clase con los datos de transferencia del contenido
	/// </summary>
	public class ContentTransfer
	{	// Constantes internas con la codificación de la transferencia
			internal const string cnstStrTransferEncodingBase64 = "base64";
			internal const string cnstStrTransferEncodingQuotedPrintable = "quoted-printable";
			internal const string cnstStrTransferEncoding8Bit = "8Bit";
			internal const string cnstStrTransferEncoding7Bit = "7Bit";
		// Codificación del contenido para transferirlo
			public enum ContentTransferEncoding
				{ Unknown,
					QuotedPrintable,
					Base64,
					Bit8,
					Bit7
				}
		// Variables privadas
			private string strTransferEncoding;
			
		
		/// <summary>
		///		Obtiene la descripción del tipo de transferencia
		/// </summary>
		internal static string GetTransferEncoding(ContentTransferEncoding intTransferEncoding)
		{ switch (intTransferEncoding)
				{ case ContentTransferEncoding.Base64:
						return cnstStrTransferEncodingBase64;
					case ContentTransferEncoding.QuotedPrintable:
						return cnstStrTransferEncodingQuotedPrintable;
					case ContentTransferEncoding.Bit7:
						return cnstStrTransferEncoding7Bit;
					case ContentTransferEncoding.Bit8:
						return cnstStrTransferEncoding8Bit;
					default:
						return "";
				}
		}
		
		/// <summary>
		///		Codificación de la transferenca
		/// </summary>
		public ContentTransferEncoding TransferEncoding { get; set; }
		
		/// <summary>
		///		Definición de la codificación del contenido
		/// </summary>
		public string TransferEncodingDefinition
		{ get
				{ string strTransfer = GetTransferEncoding(TransferEncoding);
				
						if (!string.IsNullOrEmpty(strTransfer))
							return strTransfer;
						else
							return strTransferEncoding;
				}
			set
				{ // Asigna el tipo de transferencia
						strTransferEncoding = value;
						TransferEncoding = ContentTransferEncoding.Unknown;
					// Comprueba si es alguno de los tipos de contenido identificados
						if (!string.IsNullOrEmpty(strTransferEncoding))
							{ // Quita los espacios
									strTransferEncoding = strTransferEncoding.Trim();
								// Asigna el tipo
									if (strTransferEncoding.Equals(cnstStrTransferEncodingBase64,
																								 StringComparison.CurrentCultureIgnoreCase))
										TransferEncoding = ContentTransferEncoding.Base64;
									else if (strTransferEncoding.Equals(cnstStrTransferEncodingQuotedPrintable,
																											StringComparison.CurrentCultureIgnoreCase))
										TransferEncoding = ContentTransferEncoding.QuotedPrintable;
									else if (strTransferEncoding.Equals(cnstStrTransferEncoding8Bit,
																											StringComparison.CurrentCultureIgnoreCase))
										TransferEncoding = ContentTransferEncoding.Bit8;
									else if (strTransferEncoding.Equals(cnstStrTransferEncoding8Bit,
																											StringComparison.CurrentCultureIgnoreCase))
										TransferEncoding = ContentTransferEncoding.Bit7;
							}
				}
		}
	}
}
