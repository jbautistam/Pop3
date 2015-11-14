using System;

using Bau.Libraries.LibEncoder;
namespace Bau.Libraries.LibMailProtocols.SMTP
{
	/// <summary>
	///		Clase cliente para un protocolo SMTP de recepción de correos electrónicos
	/// </summary>
  public class SMTPClient
  {	// Constantes privadas
			private const string cnstStrOk = "+ok ";
			private const string cnstStrNewLine = "\r\n";
			private const string cnstStrEndTrasmission = cnstStrNewLine + "." + cnstStrNewLine;
		// Eventos
			public event EventHandler<SMTPEventArgs> SMTPAction;
		// Variables privadas
			private Tools.Network.Connection objConnection;

		public SMTPClient(string strServer, int intPort, string strUser, string strPassword)
							: this(strServer, intPort, strUser, strPassword, false) { }

		public SMTPClient(string strServer, int intPort, string strUser, string strPassword,
											bool blnUseSSL)
							: this(new MailServer(strServer, intPort, strUser, strPassword, blnUseSSL)) { }

    public SMTPClient(MailServer objServer)
    {	Server = objServer;
    }

    /// <summary>
    ///		Conecta al servidor de correo
    /// </summary>
    public bool Connect()
    {	SMTPResponse objResponse = null;
			bool blnConnected = false;
        
        // Desconecta
					Disconnect();
				// Lanza el evento de comienzo de conexión
					RaiseEvent(SMTPEventArgs.ActionType.ConnectionEstablishing);
        // Conecta al servidor
					objConnection = new Tools.Network.Connection(Server);
					objConnection.Connect();
				// Comprueba la respuesta del servidor
					objResponse = new SMTPResponse(objConnection.Receive());
					if (!objResponse.IsOk)
						TreatError("Server " + Server.Address + " did not send welcome message.");
					else
						{ // Manda un comando EHLO para comprobar si el servidor responde a comandos extendidos
								objResponse = Send("EHLO " + Tools.Network.Connection.GetLocalHostName(), false);
							// Manda un comando HELO 
								if (!objResponse.IsOk)
									objResponse = Send("HELO " + Tools.Network.Connection.GetLocalHostName(), false);
							// Si es correcto, se autentifica
								if (objResponse.IsOk)
									{ // Lanza el evento de conexión establecida
											RaiseEvent(SMTPEventArgs.ActionType.ConnectionEstablished);
										// Lanza el comando de autentificación
											if (Authenticate(Server.User, Server.Password))
												blnConnected = true;
											else
												TreatError("Not aunthentified");
									}
						}
				// Devuelve el valor que indica si se ha conectado
					return blnConnected;
    }

		/// <summary>
		///		Autentifica un usuario
		/// </summary>
		private bool Authenticate(string strUser, string strPassword)
		{ string strEncoded = Encoder.Encode(Encoder.EncoderType.Base64, ((char) 0) + strUser + ((char) 0) + strPassword);
			SMTPResponse objResponse = Send("AUTH PLAIN " + strEncoded, false);
		
				// Comprueba si se ha autentificado
					return objResponse.IsOk;
		}
		
		/// <summary>
		///		Envía un mensaje
		/// </summary>
		public void Send(Messages.MimeMessage objMessage)
		{ SMTPResponse objResponse;
		
				// Envía el FROM
					objResponse = Send("MAIL FROM: " + GetEMailAddress(objMessage.From), false);
				// Envía el destinatario
					if (objResponse.IsOk)
						{ // Envía los destinatarios
								SendRecipients(objMessage.To);
								SendRecipients(objMessage.CC);
								SendRecipients(objMessage.BCC);
							// Envía el mensaje
								SendMessage(objMessage);
						}
					else
						TreatError("Error en el envío (comando MAIL FROM)");
		}
		
		/// <summary>
		///		Envía los destinatarios
		/// </summary>
		private void SendRecipients(Messages.AddressesCollection objColAddress)
		{ foreach (Messages.Address objAddress in objColAddress)
				{ SMTPResponse objResponse = Send("RCPT TO: " + GetEMailAddress(objAddress), false);
				
						if (!objResponse.IsOk)
							TreatError("Error al enviar el destinatario " + objAddress.EMail);
				}
		}
		
		/// <summary>
		///		Obtiene la dirección de correo con formato SMTP
		/// </summary>
		private string GetEMailAddress(Messages.Address objAddress)
		{ return "<" + objAddress.EMail + ">";
		}
		
		/// <summary>
		///		Envía el mensaje
		/// </summary>
		private void SendMessage(Messages.MimeMessage objMessage)
		{ SMTPResponse objResponse;
		
				// Comienza el envío de datos
					objResponse = Send("DATA", false);
				// Envía los datos
					if (objResponse.Code == 354)
						{ // Envía todos los datos del mensaje (MIME)
								objResponse = Send(Messages.Creator.MimeMessageCreator.CreateMessage(objMessage) + 
																			cnstStrEndTrasmission, false);
							// Comprueba si todo es correcto
								if (!objResponse.IsOk)
									TreatError("Error en el envío de datos");
						}
					else
						TreatError("Error en el envío (comando DATA)");
		}
		
		/// <summary>
		///		Desconecta del servidor de correo
		/// </summary>
    public void Disconnect()
    {	if (objConnection != null)
        {	// Envía el comando
						Send("QUIT", false);
					// Desconecta del servidor
						ShutDown();
          // Lanza el evento de desconexión
						RaiseEvent(SMTPEventArgs.ActionType.Disconnected);
        }
    }

		/// <summary>
		///		Cierra la conexión física
		/// </summary>
		private void ShutDown()
		{	objConnection.Disconnect(); 
			objConnection = null;
		}
        
    /// <summary>
    ///		Envía un mensaje y recibe la respuesta
    /// </summary>
    private SMTPResponse Send(string strMessage, bool blnResponseMultiline)
    {	string strResponse = "";
			bool blnWaitDataAvailable = true;

				// Lanza el evento de envío de un comando
					RaiseEvent(SMTPEventArgs.ActionType.SendedCommand, strMessage);
				// Envía el comando
					objConnection.Send(strMessage + cnstStrNewLine);
				// Envía el comando y obtiene la respuesta
					do
						{ // Obtiene la respuesta
								strResponse += objConnection.Receive(blnWaitDataAvailable);
							// Indica que es (al menos) la segunda vez que se recibe parte de un mensaje
								blnWaitDataAvailable = false;
						}
					while (!IsEndResponse(blnResponseMultiline, strResponse));
				// Lanza el evento de fin de un envío
					RaiseEvent(SMTPEventArgs.ActionType.ServerResponse, strResponse);
				// Comprueba la respuesta del servidor
					return new SMTPResponse(strResponse);
		}

		/// <summary>
		///		Comprueba si se ha finalizado una respuesta
		/// </summary>
		private bool IsEndResponse(bool blnResponseMultiline, string strResponse)
		{ return !blnResponseMultiline ||
						 (blnResponseMultiline && 
								(strResponse.EndsWith("." + cnstStrNewLine) && 
								 !strResponse.EndsWith(cnstStrNewLine + cnstStrNewLine + "." + cnstStrNewLine)));
		}

		/// <summary>
		///		Trata un error: desconecta del servidor y lanza una excepción
		/// </summary>
		private void TreatError(string strError)
		{ // Desconecta
				ShutDown();
			// Lanza una excepción
				throw new SMTPClientException(strError);
		}
		
		/// <summary>
		///		Lanza un evento
		/// </summary>
		private void RaiseEvent(SMTPEventArgs.ActionType intAction)
		{ RaiseEvent(intAction, null);
		}
		
		/// <summary>
		///		Lanza un evento
		/// </summary>
    private void RaiseEvent(SMTPEventArgs.ActionType intAction, string strDescription)
    {	if (SMTPAction != null)
        SMTPAction(this, new SMTPEventArgs(intAction, Server, strDescription));
    }

		/// <summary>
		///		Servidor SMTP
		/// </summary>
    public MailServer Server { get; set; }
	}
}
