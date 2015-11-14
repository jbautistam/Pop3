using System;
using System.Text;

using BauEncoder = Bau.Libraries.LibEncoder;

namespace Bau.Libraries.LibMailProtocols.Messages.Creator
{
	/// <summary>
	///		Creador de mensajes
	/// </summary>
	public static class MimeMessageCreator
	{ // Constantes privadas
			private const int cnstStrMaxCharsPerLine = 76;
			private const string cnstStrEndLine = "\r\n";
		
		/// <summary>
		///		Crea el mensaje a enviar
		/// </summary>
		public static string CreateMessage(MimeMessage objMessage)
		{ return CreateMessage(objMessage, "iso-8859-15");
		}
		
		/// <summary>
		///		Crea la cabecera del mensaje
		/// </summary>
		public static string CreateMessage(MimeMessage objMessage, string strCharSet)
		{	StringBuilder sbWriter = new StringBuilder();

				// Responder a ...
					if (!string.IsNullOrEmpty(objMessage.ReplyTo.EMail))
						sbWriter.Append("Reply-To: " + GetEMail(objMessage.ReplyTo) + cnstStrEndLine);
				// Remitente
					if (!string.IsNullOrEmpty(objMessage.From.EMail))
						sbWriter.Append("From: " + GetEMail(objMessage.From) + cnstStrEndLine);
				// Destinatarios
					sbWriter.Append("To: " + CreateAddressList(objMessage.To) + cnstStrEndLine);
					if (objMessage.CC.Count > 0)
						sbWriter.Append("CC: " + CreateAddressList(objMessage.CC) + cnstStrEndLine);
					if (objMessage.BCC.Count > 0)
						sbWriter.Append("BCC: " + CreateAddressList(objMessage.BCC) + cnstStrEndLine);
				// Asunto
					if (!string.IsNullOrEmpty(objMessage.Subject))
						sbWriter.Append("Subject: " + LibEncoder.Encoder.Encode(LibEncoder.Encoder.EncoderType.QuotedPrintable,
																																		objMessage.Subject, true) + cnstStrEndLine);
				// Añade la fecha
					sbWriter.Append("Date: " + DateTime.Now.ToUniversalTime().ToString("R") + cnstStrEndLine);
				// Añade la cabecera de emisión
					sbWriter.Append("X-Mailer: LibMailProtocols.net" + cnstStrEndLine);
				// Añade los datos de notificación
					//if (notification)
					//{
					//  if (ReplyTo.name != null && ReplyTo.name.Length != 0)
					//           {
					//               sb.Append("Disposition-Notification-To: " + MailEncoder.ConvertHeaderToQP(ReplyTo.name, charset) + " <" + ReplyTo.address + ">\r\n");
					//           }
					//  else
					//  {
					//               sb.Append("Disposition-Notification-To: <" + ReplyTo.address + ">\r\n");
					//           }
					//}

					//if (Priority != MailPriority.Unknown)
					//  sb.Append("X-Priority: " + ((int) Priority).ToString() + "\r\n");
				// Añade las cabeceras
					sbWriter.Append(GetHeaders(objMessage));
				// Añade la versión MIME
					sbWriter.Append("MIME-Version: 1.0" + cnstStrEndLine);
					sbWriter.Append(GetMessageBody(objMessage, strCharSet));
				// Devuelve la cadena a enviar
					return sbWriter.ToString();
		}
		
		/// <summary>
		///		Obtiene una dirección de correo
		/// </summary>
		private static string GetEMail(Address objAddress)
		{ string strEMail = "<" + objAddress.EMail + ">";
		
				// Añade el nombre
					if (!string.IsNullOrEmpty(objAddress.Name))
						strEMail = "\"" + EncodeQP(objAddress.Name, false) + "\" " + strEMail;
				// Devuelve la cadena
					return strEMail;
		}
		
		/// <summary>
		///		Codifica un mensaje en QuotedPrintable
		/// </summary>
		private static string EncodeQP(string strValue, bool blnSubject)
		{ return BauEncoder.Encoder.Encode(BauEncoder.Encoder.EncoderType.QuotedPrintable, strValue, blnSubject);
		}
		
		/// <summary>
		///		Crea una lista de direcciones
		/// </summary>
		private static string CreateAddressList(AddressesCollection objColAddress)
		{ StringBuilder sb = new StringBuilder();

				// Añade las direcciones
					foreach (Address objAddress in objColAddress)
						if (!string.IsNullOrEmpty(objAddress.EMail))
							{// Añade una coma si es necesario
									if (sb.Length > 0)
										sb.Append(",");
								// Añade la dirección de correo
									sb.Append(GetEMail(objAddress));
							}
				// Devuelve la lista de direcciones
					return sb.ToString().Trim();
		}
		
		/// <summary>
		///		Obtiene las cabeceras de un mensaje
		/// </summary>
		private static string GetHeaders(MimeMessage objMessage)
		{ StringBuilder sbHeaders = new StringBuilder();
		
				// Añade las cabeceras
					foreach (Header objHeader in objMessage.Headers)
						if (!string.IsNullOrEmpty(objHeader.Name))
							sbHeaders.Append(objHeader.Name + ":" + EncodeQP(objHeader.Value, false) + cnstStrEndLine);
				// Devuelve la cadena
					return sbHeaders.ToString().Trim();
		}

		/// <summary>
		///		Obtiene una cadena con el cuerpo del mensaje (incluyendo los adjuntos
		/// </summary>
		private static string GetMessageBody(MimeMessage objMessage, string strCharSet) 
		{ StringBuilder sb = new StringBuilder();
			string strMixedBoundary = GenerateMixedMimeBoundary();

				// Si hay adjuntos añade la cabecera
					if (objMessage.Attachments.Count > 0) 
						{	sb.Append("Content-Type: multipart/mixed;" + cnstStrEndLine);
							sb.Append(" boundary=\"" + strMixedBoundary + "\"");
							sb.Append("\r\n\r\nThis is a multi-part message in MIME format.");
							sb.Append("\r\n\r\n--" + strMixedBoundary + cnstStrEndLine);
						}
				// Añade el cuerpo del mensaje
					sb.Append(GetInnerMessageBody(objMessage, strCharSet));
				// Añade los adjuntos
					if (objMessage.Attachments.Count > 0) 
					  {	//Añade el adjunto
					      foreach (Section objAttachment in objMessage.Attachments) 
					        {	// Separador
											sb.Append("\r\n--" + strMixedBoundary + cnstStrEndLine);
										// Texto del adjunto
											sb.Append(GetAttachmentText(objAttachment, strCharSet));
					        }
					    // Añade el fin de mensaje
					      sb.Append("\r\n--" + strMixedBoundary + "--\r\n");
					  }
				// Devuelve la cadena
					return sb.ToString().Trim();
		}

		/// <summary>
		///		Obtiene el HTML y/o texto
		/// </summary>
		private static string GetInnerMessageBody(MimeMessage objMessage, string strCharSet)
		{ string strRelatedBoundary = GenerateRelatedMimeBoundary();
			StringBuilder sb = new StringBuilder();
			
				// Añade la cabecera de las imágenes
					//if (images.Count > 0)
					//  {	sb.Append("Content-Type: multipart/related;");
					//    sb.Append("boundary=\"" + strRelatedBoundary + "\"");
					//    sb.Append("\r\n\r\n--" + strRelatedBoundary + "\r\n");
					//  }
				// Añade el cuerpo de mensaje (legible)
					sb.Append(GetReadableMessageBody(objMessage, strCharSet));
				// Añade las imágenes
					//if (images.Count > 0)
					//  { // Añade las imágenes 
					//      foreach (Attachment image in images) 
					//        {	sb.Append("\r\n\r\n--" + strRelatedBoundary + "\r\n");
					//          sb.Append(image.ToMime());
					//        }
					//    // Añade el final de la sección
					//      sb.Append("\r\n\r\n--" + strRelatedBoundary + "--\r\n");
					//  }
				// Devuelve la cadena
					return sb.ToString().Trim();
		}

		/// <summary>
		///		Obtiene la cadena con el cuerpo del mensaje
		/// </summary>
		private static string GetReadableMessageBody(MimeMessage objMessage, string strCharSet) 
		{ StringBuilder sb = new StringBuilder();

				// Añade el cuerpo del mensaje
					if (string.IsNullOrEmpty(objMessage.BodyHTML.Content))
						sb.Append(GetTextMessageBody(objMessage.Body.Content, "text/plain", strCharSet));
					else if (string.IsNullOrEmpty(objMessage.Body.Content))
						sb.Append(GetTextMessageBody(objMessage.BodyHTML.Content, "text/html", strCharSet));
					else
						sb.Append(GetAltMessageBody(GetTextMessageBody(objMessage.Body.Content, "text/plain", strCharSet),
						                            GetTextMessageBody(objMessage.BodyHTML.Content, "text/html", strCharSet)));
				// Devuelve la cadena
					return sb.ToString().Trim();
		}

		/// <summary>
		///		Obtiene el texto de un cuerpo de mensaje
		/// </summary>
		private static string GetTextMessageBody(string strMessage, string strTextType, string strCharSet)
		{ StringBuilder sbMessage = new StringBuilder();

				// Cabecera
					sbMessage.Append("Content-Type: " + strTextType + ";\r\n");
					sbMessage.Append(" charset=" + strCharSet + "\r\n");
					sbMessage.Append("Content-Transfer-Encoding: quoted-printable\r\n\r\n");
				// Mensaje
					sbMessage.Append(EncodeQP(strMessage, false));
				// Devuelve la cadena
					return sbMessage.ToString().Trim();
		}

		/// <summary>
		///		Obtiene el cuerpo alternativo del mensaje
		/// </summary>
		private static string GetAltMessageBody(string strBody, string strHTML)
		{ StringBuilder sb = new StringBuilder();
			string strAltBoundary = GenerateAltMimeBoundary();
			string strRelatedBoundary = GenerateRelatedMimeBoundary();
			
				// Añade la cabecera de las imágenes
					//if (images.Count > 0)
					  {	sb.Append("Content-Type: multipart/related;");
					    sb.Append("boundary=\"" + strRelatedBoundary + "\"");
					    sb.Append("\r\n\r\n--" + strRelatedBoundary + "\r\n");
					  }
				// Cabecera
					sb.Append("Content-Type: multipart/alternative;" + cnstStrEndLine);
					sb.Append(" boundary=\"" + strAltBoundary + "\"");
					// sb.Append("\r\n\r\nThis is a multi-part message in MIME format.");
					sb.Append("\r\n\r\n--" + strAltBoundary + "\r\n");
				// Cuerpo
					sb.Append(strBody);
					sb.Append("\r\n\r\n--" + strAltBoundary + "\r\n");
				// Cuerpo HTML
					sb.Append(strHTML);
					sb.Append("\r\n\r\n--" + strAltBoundary + "--\r\n");
					
				// Añade las imágenes
					//if (images.Count > 0)
					//  { // Añade las imágenes 
					//      foreach (Attachment image in images) 
					//        {	sb.Append("\r\n\r\n--" + strRelatedBoundary + "\r\n");
					//          sb.Append(image.ToMime());
					//        }
					    // Añade el final de la sección
					      sb.Append("\r\n\r\n--" + strRelatedBoundary + "--\r\n");
					//  }

				// Devuelve la cadena
					return sb.ToString().Trim();
		}

		/// <summary>
		///		Añade el texto del adjunto
		/// </summary>
		private static string GetAttachmentText(Section objAttachment, string strCharSet) 
		{ StringBuilder sb = new StringBuilder();

//Content-Type: application/pdf; charset=iso-8859-1; name=N43QN93M.pdf
//Content-Transfer-Encoding: base64
//Content-Disposition: attachment; filename=N43QN93M.pdf
				// Cabecera		
					if (!string.IsNullOrEmpty(objAttachment.ID)) 
						sb.Append("Content-ID: <" + objAttachment.ID + ">\r\n");
					if (objAttachment.ContentDisposition.IsAttachment)
						{	sb.Append("Content-Type: " + objAttachment.ContentDisposition.MediaType + ";");
							sb.Append("charset=" + strCharSet + ";\r\n");
							sb.Append(" name=\"" + EncodeQP(System.IO.Path.GetFileName(objAttachment.ContentDisposition.FileName), false) + "\"\r\n");
						}
					else
						{ sb.Append("Content-Type: " + ContentType.GetContentType(ContentType.ContentTypeEnum.Base64) + ";\r\n");
							sb.Append(" name=\"" + EncodeQP(System.IO.Path.GetFileName(objAttachment.ContentDisposition.FileName), false) + "\"\r\n");
						}
					sb.Append("Content-Transfer-Encoding: " + ContentTransfer.GetTransferEncoding(ContentTransfer.ContentTransferEncoding.Base64) + "\r\n");
					sb.Append("Content-Disposition: attachment;\r\n");
					sb.Append(" filename=\"" + EncodeQP(System.IO.Path.GetFileName(objAttachment.ContentDisposition.FileName), false) + 
												"\"\r\n\r\n");
				// Añade el archivo
					using (System.IO.FileStream fnFile = new System.IO.FileStream(objAttachment.ContentDisposition.FileName, 
																																				System.IO.FileMode.Open, 
																																				System.IO.FileAccess.Read))
						{ byte [] arrBytBuffer = new byte[fnFile.Length];
						
								// Lee el archivo
									fnFile.Read(arrBytBuffer, 0, arrBytBuffer.Length);
								// Añade el archivo codificado
									sb.Append(MakeLines(System.Convert.ToBase64String(arrBytBuffer, 0, arrBytBuffer.Length)));
								// Cierra el archivo
									fnFile.Close();
						}
				// Devuelve la cadena
					return sb.ToString().Trim();
		}

		/// <summary>
		///		Separa el contenido en bloques de <see cref="cnstStrMaxCharsPerLine"/> caracteres
		/// </summary>
		private static string MakeLines(string strSource) 
		{ StringBuilder sb = new StringBuilder();
			int intPosition = 0;

				// Recorre la cadena insertando los saltos de línea			
					for (int intIndex = 0; intIndex < strSource.Length; intIndex++) 
						{	// Añade el carácter
								sb.Append(strSource[intIndex]);
							// Incrementa el contador de caracteres de la línea
								intPosition++;
							// Añade un salto de línea si es necesario
								if (intPosition >= cnstStrMaxCharsPerLine) 
									{ // Añade el salto de línea
											sb.Append(cnstStrEndLine);
										// Indica que es el primer carácter de la línea
											intPosition = 0;
									}				
						}
				// Devuelve la cadena con los saltos de línea
					return sb.ToString().Trim();
		}

		private static string GenerateMixedMimeBoundary()
		{	return "Part." + Convert.ToString(new Random(unchecked((int)DateTime.Now.Ticks)).Next()) + "." + Convert.ToString(new Random(~unchecked((int)DateTime.Now.Ticks)).Next());
		}

		private static string GenerateAltMimeBoundary()
		{	return "Part." + Convert.ToString(new Random(~unchecked((int)DateTime.Now.AddDays(1).Ticks)).Next()) + "." + Convert.ToString(new Random(unchecked((int)DateTime.Now.AddDays(1).Ticks)).Next());
		}

		private static string GenerateRelatedMimeBoundary()
		{	return "Part." + Convert.ToString(new Random(~unchecked((int)DateTime.Now.AddDays(2).Ticks)).Next()) + "." + Convert.ToString(new Random(unchecked((int)DateTime.Now.AddDays(1).Ticks)).Next());
		}

	}
}
