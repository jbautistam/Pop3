using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Bau.Libraries.LibEncoder.Encoders
{
	/// <summary>
	///		Implementación del codificador en formato QuotedPrintable
	/// </summary>
	public class QuotedPrintableEncoder :IEncoder
	{	// Constantes privadas
			private const int cnstIntCharsPerLine = 75; // Número máximo de caracteres por línea sin incluir los saltos de línea
			private const string cnstStrEndOfLine = "\r\n";
			private const string cnstStrEqual = "=";
			private const string cnstStrHexPattern = "(\\=([0-9A-F][0-9A-F]))";

		/// <summary>
		///		Codifica una cadena con un charset determinado
		/// </summary>
		public string Encode(string strCharset, string strSource, bool blnSubject)
		{ if (IsNonAscii(strSource))
				return EncodeQP(strSource, strCharset, false, blnSubject);
			else
				return strSource;
		}

		/// <summary>
		///		Codifica un array de bytes
		/// </summary>
		public string Encode(string strCharSet, byte[] arrBytBuffer, bool blnSubject)
		{ return Encode(strCharSet, UTF8Encoding.UTF8.GetString(arrBytBuffer, 0, arrBytBuffer.Length), blnSubject);
		}

		/// <summary>
		///		Decodifica una cadena
		/// </summary>
		public string Decode(string strCharSet, string strSource, bool blnSubject)
		{ return DecodeQP(strCharSet, strSource, blnSubject);
		}
		
		/// <summary>
		///		Decodifica una cadena desde un array de bytes
		/// </summary>
		public string Decode(string strCharSet, byte[] arrBytSource, bool blnSubject)
		{ return Decode(strCharSet, UTF8Encoding.UTF8.GetString(arrBytSource, 0, arrBytSource.Length),
										blnSubject);
		}
		
		/// <summary>
		///		Decodifica una cadena en un array de bytes
		/// </summary>
		public byte[] DecodeToBytes(string strCharSet, string strSource, bool blnSubject)
		{ return ASCIIEncoding.ASCII.GetBytes(Decode(strCharSet, strSource, blnSubject));
		}

		/// <summary>
		///		Decodifica un array de bytes en un array de bytes
		/// </summary>
		public byte[] DecodeToBytes(string strCharSet, byte[] arrBytSource, bool blnSubject)
		{ return DecodeToBytes(strCharSet, ASCIIEncoding.ASCII.GetString(arrBytSource), blnSubject);
		}
		
		/// <summary>
		///		Codifica una cadena en el formato QuotedPrintable
		/// </summary>
		private string EncodeQP(string strSource, string strCharSet, bool blnForceRFC2047, bool blnSubject) 
		{	StringBuilder sbTarget = new StringBuilder();
			StringReader stmReader = new StringReader(strSource);
			string strLine = null;
			bool blnForceEncoding = false;
			int intColumnPosition;
			
				// Inicializa la columna
					intColumnPosition = sbTarget.Length;
				// Codifica la cadena
					while ((strLine = stmReader.ReadLine()) != null) 
						{	Encoding objEncoding = GetEncoderText(strCharSet);
							byte[] arrBytLine = objEncoding.GetBytes(strLine); 
							StringBuilder sbBlankendChars = new StringBuilder();
							int intEndPos = arrBytLine.Length - 1;

								// Codifica los caracteres en blanco
									while (intEndPos >= 0 && (arrBytLine[intEndPos] == 0x20 || arrBytLine[intEndPos] == 0x09) ) 
										{	sbBlankendChars.Insert(sbBlankendChars.Length, EncodeByte(arrBytLine[intEndPos]));
											intEndPos--;
										}
								// Añade los caracteres codificados
									for (int intIndex = 0; intIndex <= intEndPos; intIndex++) 
										{	string strToWrite = "";
										
												// Añade el carácter (codificado o no)
													if (blnForceEncoding || NeedsEncoding(arrBytLine[intIndex], blnForceRFC2047))
														{	intColumnPosition += 3;
															strToWrite = EncodeByte(arrBytLine[intIndex]);
														} 
													else 
														{	intColumnPosition++;
															strToWrite = objEncoding.GetString(new byte[]{arrBytLine[intIndex]});
														}
												// Añade el salto de línea
													if (intColumnPosition > QuotedPrintableEncoder.cnstIntCharsPerLine ) 
														{	sbTarget.Append("=" + QuotedPrintableEncoder.cnstStrEndOfLine);
															intColumnPosition = strToWrite.Length;
														}
												// Añade la cadena al buffer total
													sbTarget.Append(strToWrite);					
										}
								// Añade los caracteres finales
									if (sbBlankendChars.Length > 0) 
										{	if (intColumnPosition + sbBlankendChars.Length > QuotedPrintableEncoder.cnstIntCharsPerLine) 
												sbTarget.Append("=" + QuotedPrintableEncoder.cnstStrEndOfLine);
											sbTarget.Append(sbBlankendChars);
										}
								// Añade un salto de línea
									if (stmReader.Peek() >= 0) 
										sbTarget.Append(cnstStrEndOfLine);
								// Inicializa la columna
									intColumnPosition = 0;
						}
				// Devuelve la cadena codificada
					return sbTarget.ToString();
		}

		/// <summary>
		///		Obtiene el codificador de texto
		/// </summary>
		private Encoding GetEncoderText(string strCharSet)
		{	if (string.IsNullOrEmpty(strCharSet))
				return Encoding.GetEncoding("utf-8");
			else
				return Encoding.GetEncoding(strCharSet);
		}

		/// <summary>
		///		Codifica una cadena como asunto de mensaje
		/// </summary>
		private string EncodeSubject(string strSubject, string strCharset)
		{ if (IsNonAscii(strSubject))
				{ string strContent = "";
				
						// Reemplaza los saltos de línea
							strSubject = strSubject.Replace("\r\n", null);
							strSubject = strSubject.Replace("\n", null);
						// Codifica el texto
							strSubject = Encode(strSubject, strCharset, false);
						// Lee por líneas y le añade los datos
							using (StringReader stmReader = new StringReader(strSubject))
								{ bool blnFirst = true;
									string strLine;
								
										// Añade la cabecera
											strContent = "=?" + strCharset + "?Q?";
										// Lee las líneas
											while ((strLine = stmReader.ReadLine()) != null)
												{ // Añade el salto de línea si es necesario
														if (!blnFirst)
															strContent += "\r\n =";
													// Añade la línea
														strContent += strLine;
													// Indica que ya no es la primera línea
														blnFirst = false;
												}
								}
						// Añade el fin de código y lo devuelve
							return strContent + "?=";
				}
			else
				return strSubject;
		}
		
		/// <summary>
		///		Codifica un carácter
		/// </summary>
		private string EncodeChar(char chrChar) 
		{ int intChar = (int) chrChar;
		
				// Devuelve el carácter codificado
					if (intChar > 255) 
						return string.Format("={0:X2}={1:X2}", intChar >> 8, intChar & 0xFF);
					else 
						return string.Format("={0:X2}", chrChar);
		}

		/// <summary>
		///		Codifica un byte
		/// </summary>
		private string EncodeByte(byte chrChar) 
		{	return string.Format("={0:X2}", (int) chrChar);
		}

		/// <summary>
		/// Return true if the char needs to be encoded.
		/// </summary>
		internal bool NeedsEncoding(char chrChar, bool blnForceRFC2047) 
		{ return (IsNonAscii(chrChar) || (blnForceRFC2047 && (chrChar == 0x20 || chrChar == 0x09 || chrChar == 0x3f)));
		}

		/// <summary>
		/// Return true if the byte needs to be encoded.
		/// </summary>
		internal bool NeedsEncoding(byte bytChar, bool blnForceRFC2047) 
		{ return NeedsEncoding((char) bytChar, blnForceRFC2047);
		}

		/// <summary>
		///		Comprueba si en una cadena hay algún carácter no ASCII
		/// </summary>
		private bool IsNonAscii(string strSource) 
		{ // Comprueba los caracteres de la cadena
				foreach (char chrChar in strSource)
					if (IsNonAscii(chrChar))
						return true;
			// Si ha llegado hasta aquí es porque todos los caracteres son ASCII
				return false;	
		}

		/// <summary>
		///		Se considera que un carácter no es ASCII si su código está por debajo del del espacio
		///	(excepto el tabulador) o es un signo igual o el código es mayor o igual a 0x7F
		/// </summary>
		private bool IsNonAscii(char chrChar) 
		{ return (chrChar <= 0x1F && chrChar != 0x09) || chrChar == 0x3D || chrChar >= 0x7F;
		}

		/// <summary>
		///		Decodifica una cadena
		/// </summary>
		private string DecodeQP(string strCharSet, string strText, bool blnSubject)
		{ StringBuilder sbWriter = new StringBuilder();
			string strTarget;

				// Decodifica el mensaje
					using (StringReader stmReader = new StringReader(SplitQuotedPrintable(strText, blnSubject)))
						{ string strLine;
						
								while ((strLine = stmReader.ReadLine()) != null)
									{ // Elimina los blancos finales
											strLine = strLine.TrimEnd();
										// Elimina el salto de línea final
											if (strLine.EndsWith("="))
												{ // Quita el fin de línea
														if (strLine.Length > 1)
															strLine = strLine.Substring(0, strLine.Length - 1);
														else
															strLine = "";
													// Añade la línea sin salto de línea
														sbWriter.Append(DecodeLine(strCharSet, strLine, blnSubject));
												}
											else
												sbWriter.AppendLine(DecodeLine(strCharSet, strLine, blnSubject));
										}
						}
				// Si es el asunto de un mensaje quita los saltos de línea y sustituye los subrayados
					strTarget = sbWriter.ToString();
					if (!string.IsNullOrEmpty(strTarget) && blnSubject)
						{ strTarget = strTarget.Replace("\r\n", " ");
							strTarget = strTarget.Replace("_", " ");
						}
				// Devuelve la cadena codificada
					return strTarget;
		}

		/// <summary>
		///		Decodifica una línea
		/// </summary>
    private string DecodeLine(string strCharSet, string strLine, bool blnSubject)
    {	Regex objHexRegex = new Regex(cnstStrHexPattern, RegexOptions.IgnoreCase);
    
				// Obtiene el charset del cabecera
					if (blnSubject && string.IsNullOrEmpty(strCharSet))
						strCharSet = GetCharSet(ref strLine);
				// Decodifica la línea
					if (!string.IsNullOrEmpty(strCharSet) && strCharSet.Equals("utf-8", StringComparison.CurrentCultureIgnoreCase))
						return DecodeQuotedPrintableUTF8(strLine);
					else
						return objHexRegex.Replace(strLine, new MatchEvaluator(HexMatchEvaluator));
    }

		/// <summary>
		///		Decodifica un carácter
		/// </summary>
    private string HexMatchEvaluator(Match objMatch)
    {	return Convert.ToChar(Convert.ToInt32(objMatch.Groups[2].Value, 16)).ToString();
    }
    
    /// <summary>
    ///		Obtiene el charSet de una línea y la elimina
    /// </summary>
    private string GetCharSet(ref string strLine)
    {	string strCharset = "";
			
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
				// Devuelve el charSet
					return strCharset;
		}

		/// <summary>
		///		Decodifica QuotedPrintable con UTF-8
		/// </summary>
		private string DecodeQuotedPrintableUTF8(string strSource) 
		{ string strOutput = "";
		
				// Decodifica los caracteres
					while (!string.IsNullOrEmpty(strSource))
						{ 
							//strSource = strSource.Replace("=\r\n", "");
							
							int intIndex = strSource.IndexOf("=");
						
						
								if (intIndex < 0)
									{ strOutput += strSource;
										strSource = "";
									}
								else
									{ strOutput += strSource.Substring(0, intIndex);
										strSource = strSource.Substring(intIndex);
										if (strSource.StartsWith("=?="))
											{ strOutput += " ";
												strSource = strSource.Substring(3);
											}
										else if (strSource.StartsWith("=") && strSource.Length > 3 && strSource.Substring(3, 1) == "=")
											{ System.Text.UTF8Encoding oUTF8Encoding = new System.Text.UTF8Encoding();
												string[] arrStrHex = strSource.Substring(0, 6).Split('=');
												string strHex2 = strSource.Substring(4, 2);
												byte [] arrByte = new byte[2];

														arrByte[0] = Convert.ToByte(int.Parse(arrStrHex[1], System.Globalization.NumberStyles.HexNumber));
														arrByte[1] = Convert.ToByte(int.Parse(arrStrHex[2], System.Globalization.NumberStyles.HexNumber));
														strOutput += oUTF8Encoding.GetString(arrByte);
													// Quita los cinco primeros caracteres
														strSource = strSource.Substring(6);
											}
										else if (strSource.Length >= 3)
											{ System.Text.UTF8Encoding oUTF8Encoding = new System.Text.UTF8Encoding();
												string strHex = strSource.Substring(1, 2);
												
													if (strHex != "\r\n")
														{	byte [] arrByte = new byte[1];
														
																arrByte[0] = Convert.ToByte(int.Parse(strHex, System.Globalization.NumberStyles.HexNumber));
													
													
																strOutput += oUTF8Encoding.GetString(arrByte);
														} 
													else 
														strOutput += "\r\n";
													// Quita los tres primeros caracteres
														strSource = strSource.Substring(3);
											}
										else
											{ strOutput += strSource;
												strSource = "";
											}
									}
						}

			return strOutput;
		}		
		
		/// <summary>
		///		Añade saltos de línea reemplazando los ?= que indican el final de una línea codificada como QuotedPrintable
		/// </summary>
		private string SplitQuotedPrintable(string strMessage, bool blnSubject)
		{ // En los asuntos, reemplaza los caracteres ?=
				if (blnSubject && !string.IsNullOrEmpty(strMessage))
					{ // Sustituye los ?= con un espacio final por = + salto de línea para que al interpretar el asunto
						// se ponga en la misma línea
							strMessage = strMessage.Replace("?= ", "=\r\n");
						// Reemplaza el ?= final
							if (strMessage.EndsWith("?="))
								strMessage = strMessage.Substring(0, strMessage.Length - 2);
					}
			// Devuelve el mensaje
				return strMessage;
		}
		
		/// <summary>
		///		Trim de una línea
		/// </summary>
		private string Trim(string strLine)
		{ if (string.IsNullOrEmpty(strLine))
				return "";
			else
				return strLine.Trim();
		}
	}
}