using System;

namespace Bau.Libraries.LibMailProtocols.Messages.Parser 
{
	/// <summary>
	///		Intérprete de las cabeceras de un mensaje Mime
	/// </summary>
	internal static class MimeHeadersParser 
	{
    /// <summary>
    ///		Lee las cabeceras del mensaje
    /// </summary>
    internal static void ReadHeaders(ParserLines objParser, MimeMessage objMessage)
    { // Lee las cabeceras
				objMessage.Headers.AddRange(ReadHeaders(objParser));
			// Normaliza las cabeceras del mensaje
				NormalizeHeaders(objMessage);
    }

		/// <summary>
		///		Lee las cabeceras de una sección
		/// </summary>
		internal static HeadersCollection ReadHeaders(ParserLines objParser, Section objSection) 
		{ HeadersCollection objColHeaders = ReadHeaders(objParser);
		
				// Normaliza las cabeceras
					NormalizeHeaders(objSection, objColHeaders);
				// Devuelve la colección de cabeceras
					return objColHeaders;
		}
		
    /// <summary>
    ///		Lee las cabeceras
    /// </summary>
    private static HeadersCollection ReadHeaders(ParserLines objParser)
    { HeadersCollection objColHeaders = new HeadersCollection();
			string strLine = "----";
    
				// Interpreta las líneas de cabecera
					while (!objParser.IsEof && !string.IsNullOrEmpty(strLine))
						{ // Obtiene la línea
								strLine = objParser.ReadLineContinuous();
							// Interpreta la cabecera (si la hay)
								if (!string.IsNullOrEmpty(strLine))
									{ Header objHeader = ParseHeader(strLine);
									
											// Añade la cabecera
												objColHeaders.Add(objHeader);
									}
						}
				// Devuelve la colección de cabeceras
					return objColHeaders;
    }

    /// <summary>
    ///		Interpreta una cabecera a partir de la línea
    /// </summary>
    private static Header ParseHeader(string strLine)
    { Header objHeader = new Header();
			int intIndex = strLine.IndexOf(":");
			
				// Separa la clave del nombre en la cabecera
					if (intIndex >= 0)
						{ // Asigna los datos de la cabecera
								objHeader.Name = MimeParserHelper.Trim(strLine.Substring(0, intIndex));
								objHeader.Value = MimeParserHelper.Trim(strLine.Substring(intIndex + 1));
							// Interpreta el valor de la cabecera
								if (!string.IsNullOrEmpty(objHeader.Value) && 
										!objHeader.Name.Equals("Subject", StringComparison.CurrentCultureIgnoreCase) && 
										objHeader.Value.IndexOf(';') >= 0)
									{ string [] arrStrParameters = objHeader.Value.Split(';');
									
											// Asigna el nuevo valor
												if (arrStrParameters.Length > 0)
													{ // Asigna el nuevo valor a la cabecera
															objHeader.Value = arrStrParameters[0];
														// Asigna las subcabeceras
															for (int intSubHeader = 1; intSubHeader < arrStrParameters.Length; intSubHeader++)
																{ int intPosition = arrStrParameters[intSubHeader].IndexOf('=');
																
																		// Añade la subcabecera
																			if (intPosition <= 0)
																				objHeader.SubHeaders.Add(Header.cnstStrUndefined, arrStrParameters[intSubHeader]);
																			else
																				objHeader.SubHeaders.Add(arrStrParameters[intSubHeader].Substring(0, intPosition),
																																 arrStrParameters[intSubHeader].Substring(intPosition + 1));

																}
													}
									}
						} 
				// Quita las comillas de las subcabeceras
					foreach (Header objSubHeader in objHeader.SubHeaders)
						{ if (objSubHeader.Value.StartsWith("\""))
								objSubHeader.Value = objSubHeader.Value.Substring(1);
							if (objSubHeader.Value.EndsWith("\""))
								objSubHeader.Value = objSubHeader.Value.Substring(0, objSubHeader.Value.Length - 1);
						}
				// Imprime la cabecera
					System.Diagnostics.Debug.WriteLine("------------------------------------------------");						
					System.Diagnostics.Debug.WriteLine("   " + objHeader.Name + ": " + objHeader.Value);
					foreach (Header objSubHeader in objHeader.SubHeaders)	
						System.Diagnostics.Debug.WriteLine("                --> " + objSubHeader.Name + ": " + objSubHeader.Value);
				// Devuelve la cabecera
					return objHeader;
    }

		/// <summary>
		///		Normaliza las cabeceras de un correo
		/// </summary>
		private static void NormalizeHeaders(MimeMessage objMail)
		{ foreach (Header objHeader in objMail.Headers)
				if (objHeader.Name.Equals(Header.cnstStrFrom, StringComparison.CurrentCultureIgnoreCase))
					objMail.From = new Address(DecodeQP(objHeader.Value, true));
				else if (objHeader.Name.Equals(Header.cnstStrTo, StringComparison.CurrentCultureIgnoreCase))
					objMail.To = new AddressesCollection(DecodeQP(objHeader.Value, true));
				else if (objHeader.Name.Equals(Header.cnstStrCC, StringComparison.CurrentCultureIgnoreCase))
					objMail.CC = new AddressesCollection(DecodeQP(objHeader.Value, true));
				else if (objHeader.Name.Equals(Header.cnstStrSubject, StringComparison.CurrentCultureIgnoreCase))
					objMail.Subject = DecodeQP(objHeader.Value, true);
				else if (objHeader.Name.Equals(Header.cnstStrDate, StringComparison.CurrentCultureIgnoreCase))
					objMail.Date = MimeParserHelper.GetDate(objHeader.Value);
				else if (objHeader.Name.Equals(Header.cnstStrMimeVersion, StringComparison.CurrentCultureIgnoreCase))
					objMail.MimeVersion = objHeader.Value;
				else if (objHeader.Name.Equals(Header.cnstStrID, StringComparison.CurrentCultureIgnoreCase))
					objMail.ID = objHeader.Value;
				else if (objHeader.Name.Equals(Header.cnstStrContentType, StringComparison.CurrentCultureIgnoreCase))
					objMail.ContentType = ParseContentType(objHeader);
				else if (objHeader.Name.Equals(Header.cnstStrContentTransferEncoding, StringComparison.CurrentCultureIgnoreCase))
					objMail.TransferEncoding = ParseTransferEncoding(objHeader);
		}
		
		/// <summary>
		///		Decodifica una cadena en QuotedPrintable
		/// </summary>
		private static string DecodeQP(string strSource, bool blnSubject)
		{ return LibEncoder.Encoder.Decode(LibEncoder.Encoder.EncoderType.QuotedPrintable,
																			 strSource, blnSubject);
		}
		
		/// <summary>
		///		Normaliza una serie de cabeceras estándar para el correo y las secciones
		/// </summary>
		internal static void NormalizeHeaders(Section objSection, HeadersCollection objColHeaders)
		{	foreach (Header objHeader in objColHeaders)
				if (objHeader.Name.Equals(Header.cnstStrContentType, StringComparison.CurrentCultureIgnoreCase))
					objSection.ContentType = ParseContentType(objHeader);
				else if (objHeader.Name.Equals(Header.cnstStrContentTransferEncoding, StringComparison.CurrentCultureIgnoreCase))
					objSection.TransferEncoding = ParseTransferEncoding(objHeader);
				else if (objHeader.Name.Equals(Header.cnstStrContentDisposition, StringComparison.CurrentCultureIgnoreCase))
					objSection.ContentDisposition = ParseContentDisposition(objHeader);
		}
    
		/// <summary>
		///		Interpreta el ContentType de un elemento
		/// </summary>
		private static ContentType ParseContentType(Header objHeader)
		{ ContentType objContentType = new ContentType();
		
				// Asigna la definición
					objContentType.ContentTypeDefinition = objHeader.SearchValue(Header.cnstStrContentType);
					objContentType.Boundary = objHeader.SearchValue(Header.cnstStrContentTypeBoundary);
					objContentType.CharSet = objHeader.SearchValue(Header.cnstStrCharSet);
				// Devuelve el tipo de contenido
					return objContentType;
		}

		/// <summary>
		///		Interpreta la forma de transferencia
		/// </summary>
		private static ContentTransfer ParseTransferEncoding(Header objHeader)
		{ ContentTransfer objTransfer = new ContentTransfer();
		
				// Asigna los datos
					objTransfer.TransferEncodingDefinition = objHeader.Value;
				// Devuelve el modo de transferencia
					return objTransfer;
		}

		/// <summary>
		///		Interpreta la disposición de la sección
		/// </summary>
		/// <example>
		///		Content-Disposition: attachment; filename="N35MQ55Z.pdf"
		/// </example>
		private static ContentDisposition ParseContentDisposition(Header objHeader)
		{ ContentDisposition objDisposition = new ContentDisposition();
		
				// Si tenemos una disposición en el valor ...
					objDisposition.ContentDispositionDefinition = objHeader.SearchValue(Header.cnstStrContentDisposition);
					objDisposition.FileName = objHeader.SearchValue(Header.cnstStrFileName);
				// Devuelve la disposición de la sección
					return objDisposition;		
		}
	}
}
