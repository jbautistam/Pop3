using System;

using Bau.Libraries.LibEncoder;

namespace Bau.Libraries.LibMailProtocols.Messages.Parser
{
	/// <summary>
	///		Interpreta los datos de un mensaje de correo
	/// </summary>
	public static class MimeMessageParser
	{			
		/// <summary>
		///		Interpreta los datos de un mensaje
		/// </summary>
		public static MimeMessage Parse(string strMessage)
    { MimeMessage objMail = new MimeMessage();
			ParserLines objParser = new ParserLines(strMessage);
			Section objSection;
			
				// Interpreta las líneas recibidas
					MimeHeadersParser.ReadHeaders(objParser, objMail);
				// Interpreta las secciones
					objSection = ParseSections(objMail.ContentType, objParser);
				// Normaliza el correo
					NormalizeMail(objMail, objSection);
				// Devuelve el mensaje recibido
					return objMail;
    }

		/// <summary>
		///		Interpreta las secciones
		/// </summary>
		private static Section ParseSections(ContentType objContentType, ParserLines objParser)
		{ Section objSection = new Section();
		
				// Asigna los parámetros a la sección
					objSection.ContentType = objContentType;
				// Interpreta la sección
					ParseSection(objSection, objParser);
				// Devuelve la sección interpretada
					return objSection;
		}
		
		/// <summary>
		///		Interpreta los datos de una sección
		/// </summary>
		private static void ParseSection(Section objSection, ParserLines objParser)
		{ string strLastLine = null;
		
				if (objSection.ContentType.IsMultipart)
					objSection.Sections.AddRange(ParseSectionsChild(objSection.ContentType.Boundary, objParser, ref strLastLine));
				else if (objSection.ContentType.Type == ContentType.ContentTypeEnum.MultipartReport)
				 	objSection.Sections.AddRange(ParseSectionsMultipartReport(objSection, objParser, ref strLastLine));
				else
					objSection.Content = ParseContentSection(objParser, null, null, ref strLastLine);									
		}

		/// <summary>
		///		Interpreta las secciones con tipo de contenido "multipart/report"
		/// </summary>
		private static SectionsCollection ParseSectionsMultipartReport(Section objSection, ParserLines objParser, 
																																	ref string strLastLine)
		{ return ParseSectionsChild(objSection.ContentType.Boundary, objParser, ref strLastLine);
		}
		
		/// <summary>
		///		Interpreta las secciones hijo
		/// </summary>
		private static SectionsCollection ParseSectionsChild(string strBoundaryParent, ParserLines objParser,
																												 ref string strLastLine)
		{ SectionsCollection objColSections = new SectionsCollection();
			string strLine;
				
				// Lee la primera línea (o recupera la anterior
					if (!string.IsNullOrEmpty(strLastLine))
						strLine = strLastLine;
					else
						strLine = objParser.ReadLine();
				// Quita las líneas anteriores al boundary si es necesario
					if (!string.IsNullOrEmpty(strBoundaryParent) && !IsStartBoundary(strLine, strBoundaryParent))
						{ // Lee la siguiente línea
								while (!objParser.IsEof && !IsStartBoundary(strLine, strBoundaryParent))
									strLine = objParser.ReadLine();
						}
				// Si es la línea de boundary
					if (IsStartBoundary(strLine, strBoundaryParent))
						{ bool blnEnd = false;
						
								// Recorre las secciones hija
									while (!objParser.IsEof && !blnEnd)
										if (!string.IsNullOrEmpty(strLastLine) && IsEndBoundary(strLastLine, strBoundaryParent))
											blnEnd = true;
										else
											{ Section objSection = ParseSection(objParser, strBoundaryParent, ref strLastLine);
											
													// Añade la sección a la colección
														objColSections.Add(objSection);
													// Comprueba la última línea leída
														if (IsEndBoundary(strLastLine, strBoundaryParent))
															{ // Vacía la última línea al ser un final de sección para pasar a la siguiente lectura
																// sin una cadena de lectura atrasada (al fin y al cabo, la hemos tratado en el
																// IsEndBoundary
																	if (!objParser.IsEof)
																		{ strLastLine = objParser.ReadLine();
																			if (strLastLine == "")
																				strLastLine = objParser.ReadLine();
																		}
																	else
																		strLastLine = "";
																// Indica que es el final de sección
																	blnEnd = true;
															}
														else if (objSection.ContentType.IsMultipart)
															objSection.Sections.AddRange(ParseSectionsChild(objSection.ContentType.Boundary,
																																							objParser, ref strLastLine));
											}
						}
					else
					  throw new Exception("Boundary mal formado");
				// Devuelve las secciones
					return objColSections;
		}
		
		/// <summary>
		///		Interpreta la sección
		/// </summary>
		private static Section ParseSection(ParserLines objParser, string strBoundaryParent, ref string strLastLine)
		{ Section objSection = new Section();
		
				// Lee las cabeceras
					objSection.Headers = MimeHeadersParser.ReadHeaders(objParser, objSection);
				// Obtiene el contenido de la sección
					objSection.Content = ParseContentSection(objParser, strBoundaryParent, objSection.ContentType.Boundary, 
																									 ref strLastLine);
				// Devuelve la sección
					return objSection;
		}
		
		/// <summary>
		///		Interpreta el contenido de una sección
		/// </summary>
		private static string ParseContentSection(ParserLines objParser, string strBoundaryParent, string strBoundary, 
																							ref string strLastLine)
		{ bool blnEnd = false;
			string strLine, strContent = "";
			
				// Lee el contenido
					while (!blnEnd && !objParser.IsEof)
					  { // Lee la línea
					      strLine = objParser.ReadLine();
					    // Dependiendo de si estamos en una línea de boundary o en una línea de contenido
					      if (IsStartBoundary(strLine, strBoundary) || IsStartBoundary(strLine, strBoundaryParent))
									{ // Guarda la última línea
											strLastLine = strLine;
										// Indica que ha terminado
											blnEnd = true;
									}
								else
									{ // Añade el salto de línea si es necesario
											if (!string.IsNullOrEmpty(strContent))
												strContent += "\r\n";
										// Añade la línea al contenido
											strContent += strLine;
									}
					  }
				// Devuelve el contenido de la sección
					return strContent;
		}
		
		/// <summary>
		///		Comprueba si la línea es un limitador de inicio
		/// </summary>
		private static bool IsStartBoundary(string strLine, string strBoundary)
		{ if (string.IsNullOrEmpty(strBoundary))
				return false;
			else if (string.IsNullOrEmpty(strLine))
				return false;
			else 
				return strLine.StartsWith("--") && strLine.IndexOf(strBoundary) >= 0;
		}
		
		/// <summary>
		///		Comprueba si la línea es un limitador de fin
		/// </summary>
		private static bool IsEndBoundary(string strLine, string strBoundary)
		{ return IsStartBoundary(strLine, strBoundary) && strLine.EndsWith("--");
		}
		
		/// <summary>
		///		Normaliza el correo
		/// </summary>
		private static void NormalizeMail(MimeMessage objMail, Section objSection)
		{	// Normaliza la sección
				if (objSection.ContentType.Type == ContentType.ContentTypeEnum.Text)
					{ objMail.Body = objSection;
						objMail.Body.Content = Encoder.Decode(Encoder.GetEncoderType(objSection.TransferEncoding.TransferEncodingDefinition),
																									objSection.ContentType.CharSet,
																									objSection.Content, false);
					}
				else if (objSection.ContentType.Type == ContentType.ContentTypeEnum.HTML)
					{ objMail.BodyHTML = objSection;
						objMail.BodyHTML.Content = Encoder.Decode(Encoder.GetEncoderType(objSection.TransferEncoding.TransferEncodingDefinition),
																											objSection.ContentType.CharSet,
																											objSection.Content, false);
					}
				else if (objSection.ContentDisposition.IsAttachment)
					objMail.Attachments.Add(objSection);
			// Normaliza las secciones hija
				foreach (Section objSectionChild in objSection.Sections)
					NormalizeMail(objMail, objSectionChild);
		}
	}
}