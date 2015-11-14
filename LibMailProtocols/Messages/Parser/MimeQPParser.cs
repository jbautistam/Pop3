using System;

using Bau.Libraries.LibEncoder;

namespace Bau.Libraries.LibMailProtocols.Messages.Parser 
{
	/// <summary>
	///		Parser para los datos codificados como QP para los mensajes MIME
	/// </summary>
	internal static class MimeQPParser 
	{
/*
		/// <summary>
		///		Decodifica una cadena en el formato QuotedPrintable
		/// </summary>
		internal static string DecodeQuotedPrintable(string strCharSet, string strMessage)
		{ return Encoder.Decode(Encoder.EncoderType.QuotedPrintable, strMessage, false);
		}

		/// <summary>
		///		Decodifica una cabecera codificada como QuotedPrintable
		///		Los asuntos en un correo son del tipo: =?ISO-8859-1?Q?Env=EDo_de_un_correo_con_acentos_y_e=F1es?=
		///	donde la primera parte es el charSet. Si la cadena es muy larga, se manda el asunto partido por
		///	líneas con cada línea un encabezado del charset,es decir:
		///	Subject: =?ISO-8859-1?Q?Informaci=F3n_registral_=2D_Registradores_de_Espa=F1a_=2D_?=
		///		=?ISO-8859-1?Q?Ref=3A_1433290_=28Denegaci=F3n=29?=
		/// </summary>
		internal static string DecodeHeaderQuotedPrintable(string strMessage)
		{ System.Collections.Generic.List<string> objColLines = SplitQuotedPrintable(strMessage);
			string strResult = "";
				
				// Lee las líneas del asunto separadas por líneas
					foreach (string strLineIndex in objColLines)
						{	string strLine = strLineIndex; // ... no se puede utilizar directamente strLineIndex porque es el índice del bucle
							string strCharset = "";
							
								// Quita la cabecera del charSet
									if (strLine.StartsWith("=?"))
										{ int intIndex;
											
											// Quita el inicio
												strLine = strLine.Substring(2);
											// Quita el charSet
												intIndex = strLine.IndexOf("?");
												if (intIndex >= 0)
													{ // Obtiene la codificación
															strCharset = strLine.Substring(0, intIndex);														
														// Quita el charSet
															strLine = strLine.Substring(intIndex);
														// Quita los caracteres ?Q? que identifican la cabecera
															strLine = strLine.Substring(3);
													}
										}
									// Quita el final de la cadena
										if (strLine.EndsWith("?="))
											strLine = strLine.Substring(0, strLine.Length - 2);
									// Decodifica la cadena con QuotedPrintable
										strResult += DecodeQuotedPrintable(strCharset, strLine);
									// Quita los saltos de línea finales
										if (strResult.EndsWith("\r\n"))
											strResult = strResult.Substring(0, strResult.Length - 2);
						}
			// Quita los espacios iniciales y finales y sustituye los caracteres de subrayado por espacios
				strResult = strResult.Trim().Replace('_', ' ');
			// Devuelve la cadena decodificada
				return strResult;
		}

		/// <summary>
		///		Decodifica QuotedPrintable con UTF-8
		/// </summary>
		private static string DecodeQuotedPrintableUTF8(string sInput) 
		{ string strOutput = "";
		
				// Decodifica los caracteres
					while (!string.IsNullOrEmpty(sInput))
						{ 
							sInput = sInput.Replace("=\r\n", "");
							
							int intIndex = sInput.IndexOf("=");
						
						
								if (intIndex < 0)
									{ strOutput += sInput;
										sInput = "";
									}
								else
									{ strOutput += sInput.Substring(0, intIndex);
										sInput = sInput.Substring(intIndex);
										if (sInput.StartsWith("=") && sInput.Length >3 && sInput.Substring(3, 1) == "=")
											{ System.Text.UTF8Encoding oUTF8Encoding = new System.Text.UTF8Encoding();
												string[] arrStrHex = sInput.Substring(0, 6).Split('=');
												string strHex2 = sInput.Substring(4, 2);
												byte [] arrByte = new byte[2];

														arrByte[0] = Convert.ToByte(int.Parse(arrStrHex[1], System.Globalization.NumberStyles.HexNumber));
														arrByte[1] = Convert.ToByte(int.Parse(arrStrHex[2], System.Globalization.NumberStyles.HexNumber));
														strOutput += oUTF8Encoding.GetString(arrByte);
													// Quita los cinco primeros caracteres
														sInput = sInput.Substring(6);
											}
										else if (sInput.Length >= 3)
											{ System.Text.UTF8Encoding oUTF8Encoding = new System.Text.UTF8Encoding();
												string strHex = sInput.Substring(1, 2);
													if (strHex != "\r\n")
														{	byte [] arrByte = new byte[1];
														
																arrByte[0] = Convert.ToByte(int.Parse(strHex, System.Globalization.NumberStyles.HexNumber));
													
													
																strOutput += oUTF8Encoding.GetString(arrByte);
														} 
													else 
														strOutput += "\r\n";
													// Quita los tres primeros caracteres
														sInput = sInput.Substring(3);
											}
										else
											{ strOutput += sInput;
												sInput = "";
											}
									}
						}

			return strOutput;
		}		
		
		/// <summary>
		///		Parte una cadena por los caracteres ?= que indican el final de una línea codificada como QuotedPrintable
		/// </summary>
		private static System.Collections.Generic.List<string> SplitQuotedPrintable(string strMessage)
		{ System.Collections.Generic.List<string> objColLines = new System.Collections.Generic.List<string>();
		
				// Separa la cadena por los caracteres ?=
					while (!string.IsNullOrEmpty(strMessage))
						{ int intIndex = strMessage.IndexOf("?= ");
							string strLine = "";
						
								// Quita la parte de la cadena
									if (intIndex >= 0)
										strLine = strMessage.Substring(0, intIndex);
									else
										strLine = strMessage;
								// Quita la parte encontrada del mensaje
									if (strLine.Length < strMessage.Length)
										strMessage = strMessage.Substring(strLine.Length + 2);
									else
										strMessage = "";
								// Añade la línea a la colección
									objColLines.Add(MimeParserHelper.Trim(strLine));
						}
				// Devuelve la colección de líneas localizadas
					return objColLines;
		}
*/
	}
}
