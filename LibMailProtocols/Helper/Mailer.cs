using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;

namespace Bau.Libraries.LibMailProtocols.Helper
{
	/// <summary>
	///		Clase de ayuda para envío de correos
	/// </summary>
	public class Mailer
	{
		public Mailer(string strServer, int intPort, string strUser, string strPassword, bool blnUseSSL)
		{ Server = strServer;
			Port = intPort;
			User = strUser;
			Password = strPassword; 
			UseSSL = blnUseSSL;
		}
		
		/// <summary>
		///		Concatena varias direcciones de correo sin repetición
		/// </summary>
		public static string ConcatEMail(string strEMailSource, string strNewEMail)
		{ return ConcatEMail(strEMailSource, strNewEMail, false, null);
		}
		
		/// <summary>
		///		Concatena varias direcciones de correo sin repetición
		/// </summary>
		public static string ConcatEMail(string strEMailSource, string strNewEMail, bool blnDebug, 
																		 string strEMailDebug)
		{ // Va añadiendo los correos a la cadena de correos
				if (!string.IsNullOrEmpty(strNewEMail))
					{ string [] arrStrEMail = strNewEMail.Split(';');
					
							foreach (string strEMailTo in arrStrEMail)
								if (!string.IsNullOrEmpty(strEMailTo) && 
										strEMailSource.IndexOf(strEMailTo, StringComparison.CurrentCultureIgnoreCase) < 0)
									{ // Añade el separador
											if (!string.IsNullOrEmpty(strEMailSource))
												strEMailSource += ";";
										// Añade el correo
											if (blnDebug)
												strEMailSource += strEMailDebug;
											else
												strEMailSource += strEMailTo;
									}
					}
			// Devuelve la cadena con los correos
				return strEMailSource;
		}

		/// <summary>
		///		Envía un correo
		/// </summary>
		public bool Send(string strEMailTo, string strSubject, string strBody)
		{ return Send(strEMailTo, strSubject, strBody, null, new List<string>());
		}

		/// <summary>
		///		Envía un correo
		/// </summary>
		public bool Send(string strEMailTo, string strSubject, string strBody, string strBodyHTML, 
										 string strFileAttachment)
		{ return Send(strEMailTo, strSubject, strBody, strBodyHTML, new List<string> { strFileAttachment });
		}

		/// <summary>
		///		Envía un correo con una colección de adjuntos
		/// </summary>
		public bool Send(string strEMailTo, string strSubject, string strBody, string strBodyHTML,
										 List<string> objColFileNames)
		{ return Send(GetMessage(strEMailTo, strSubject, strBody, strBodyHTML, objColFileNames));
		}
		
		/// <summary>
		///		Envía un correo con la excepción
		/// </summary>
		public bool Send(string strAddresTo, string strSubject, Exception objException)
		{ return Send(strAddresTo, strSubject, 
									"Excepción: " + objException.Message + "\r\nTraza: " + objException.StackTrace,
									null, new List<string>());
		}
		
		/// <summary>
		///		Envía un correo
		/// </summary>
		private bool Send(MailMessage objMessage)
		{	bool blnSent = false;
		
				// Envía el correo
					try
						{ if (objMessage.To != null && objMessage.To.Count > 0)
								{ SmtpClient objSMTP = new SmtpClient(Server, Port);
								
										// Asigna las credenciales
											objSMTP.Credentials = new System.Net.NetworkCredential(User, Password);
											objSMTP.EnableSsl = UseSSL;
										// Envía el correo
											objSMTP.Send(objMessage);
										// Indica que se ha enviado correctamente
											blnSent = true;
								}
						}
					catch (Exception objException)
						{	System.Diagnostics.Debug.WriteLine(objException.Message);
						}
				// Devuelve el valor que indica si se ha enviado el correo
					return blnSent;
		}
		
		/// <summary>
		///		Obtiene un mensaje MIME para enviarlo por eMail
		/// </summary>
		private MailMessage GetMessage(string strAddressTo, string strSubject, string strBody, string strBodyHTML, 
																	 List<string> objColFileNames)
		{ MailMessage objMessage = new MailMessage();
			string [] arrStrAddress = strAddressTo.Split(';');

			  // Añade los destinatarios
			    foreach (string strAddress in arrStrAddress)
			      if (Messages.Address.CheckEMail(strAddress))
							// if (objMessage.To.Count == 0)
								objMessage.To.Add(new MailAddress(strAddress.Trim()));
							// else
							//	objMessage.Bcc.Add(new MailAddress(strAddress.Trim()));
			  // Remitente del mensaje
			    objMessage.From = new MailAddress(User);
			  // Asigna los datos del mensaje
					objMessage.BodyEncoding = System.Text.Encoding.UTF8;
					objMessage.SubjectEncoding = System.Text.Encoding.UTF8;
			    objMessage.Subject = strSubject;
			    objMessage.Body = strBody; 
					if (!string.IsNullOrEmpty(strBodyHTML))
						{ objMessage.Body = strBodyHTML;
							objMessage.IsBodyHtml = true;
						}
			  // Añade los adjuntos
					if (objColFileNames != null)
						foreach (string strFileAttachment in objColFileNames)
							if (!string.IsNullOrEmpty(strFileAttachment))
								objMessage.Attachments.Add(new Attachment(strFileAttachment));
			  // Devuelve el mensaje
			    return objMessage;		
		}

		/// <summary>
		///		Servidor SMTP
		/// </summary>
		public string Server { get; set; }
		
		/// <summary>
		///		Puerto del servidor SMTP
		/// </summary>
		public int Port { get; set; }
		
		/// <summary>
		///		Usuario del servidor SMTP
		/// </summary>
		public string User { get; set; }
		
		/// <summary>
		///		Contraseña del servidor SMTP
		/// </summary>
		public string Password { get; set; }
		
		/// <summary>
		///		Indica si se debe utilizar SSL en las comunicaciones con el servidor SMTP
		/// </summary>
		public bool UseSSL { get; set; }
	}
}