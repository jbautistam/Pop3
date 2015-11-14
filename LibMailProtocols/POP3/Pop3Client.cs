using System;

namespace Bau.Libraries.LibMailProtocols.POP3
{
	/// <summary>
	///		Clase cliente para un protocolo Pop3 de recepción de correos electrónicos
	/// </summary>
  public class Pop3Client
  {	// Constantes privadas
			private const string cnstStrOk = "+ok ";
			private const string cnstStrNewLine = "\r\n";
		// Eventos
			public event EventHandler<Pop3EventArgs> Pop3Action;
		// Variables privadas
			private Tools.Network.Connection objConnection;

		public Pop3Client(string strServer, int intPort, string strUser, string strPassword,
											bool blnUseSSL)
							: this(new MailServer(strServer, intPort, strUser, strPassword, blnUseSSL)) { }
		
    public Pop3Client(MailServer objServer)
    {	Server = objServer;
    }

    /// <summary>
    ///		Conecta al servidor de correo
    /// </summary>
    public void Connect()
    {	string strResponse = "";
        
        // Desconecta
					Disconnect();
				// Lanza el evento de comienzo de conexión
					RaiseEvent(Pop3EventArgs.ActionType.ConnectionEstablishing);
        // Conecta al servidor
					objConnection = new Tools.Network.Connection(Server);
					objConnection.Connect();
				// Comprueba la respuesta del servidor
					strResponse = objConnection.Receive();
					if (string.IsNullOrEmpty(strResponse) ||
							!strResponse.StartsWith(cnstStrOk, StringComparison.OrdinalIgnoreCase))
						TreatError("Server " + Server.Address + " did not send welcome message.");
					else
						{	string strDigest = GetAuthenticateDigest(strResponse);
						
								 // Lanza el evento de conexión establecida
									RaiseEvent(Pop3EventArgs.ActionType.ConnectionEstablished);
								// Espera dos segundos
									System.Threading.Thread.Sleep(2000);
								// Lanza el evento de inicio de autentificación
									RaiseEvent(Pop3EventArgs.ActionType.AuthenticationBegan);
								// Envía usuario y contraseña
									Authenticate(strDigest);
								// Lanza el evento de final de autentificación
									RaiseEvent(Pop3EventArgs.ActionType.AuthenticationFinished);
						}
    }

		/// <summary>
		///		Obtiene la cadena para el digest MD5. La cadena de respuesta de inicio del servidor
		///	será del tipo +OK xxxxxxxxx &ltCadena digest&gt
		/// </summary>
		private string GetAuthenticateDigest(string strResponse)
		{ int intIndexStart = strResponse.IndexOf("<");
			string strDigest = "";
		
				// Si hay una cadena Digest en la respuesta del servidor
					if (intIndexStart >= 0)
						{ int intIndexEnd = strResponse.IndexOf(">", intIndexStart);
						
								if (intIndexEnd >= 0)
									{ string strTimeStamp = strResponse.Substring(intIndexStart, 
																																intIndexEnd - intIndexStart + 1);
																																
											// Envía la contraseña
												Send("PASSWORD " + Tools.Cryptography.MD5Helper.Compute(strTimeStamp + Server.Password), false);
									}
						}
				// Devuelve la cadena Digest
					return strDigest;
		}

		/// <summary>
		///		Autentifica el usuario
		/// </summary>
		private void Authenticate(string strDigest)
		{ if (string.IsNullOrEmpty(strDigest)) // autentifica el usuario en plano
				{	string strResponse = Send("USER " + Server.User, false);

						// Comprueba si se le envía una cadena Digest como respuesta al comando USER	
							strDigest = GetAuthenticateDigest(strResponse);
						// Envía la contraseña o el hash de contraseña
							if (string.IsNullOrEmpty(strDigest)) // ... simplemente se envía la contraseña en plano
								Send("PASS " + Server.Password, false);
							else // ... envía el MD5 de la contraseña
								Send("PASS " + Tools.Cryptography.MD5Helper.Compute(strDigest + Server.Password), false);
				}
			else // ... autentifica el usuario utilizando APOP
				Send("APOP " + Server.User + " " + Tools.Cryptography.MD5Helper.Compute(strDigest + Server.Password), false);
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
						RaiseEvent(Pop3EventArgs.ActionType.Disconnected);
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
		///		Obtiene los detalles de un buzón
		/// </summary>
    public void GetMailBoxDetails(out int intNumberEMails, out int intTotalSize)
    { string strResponse = "";
      int intIndexFirstSpace, intIndexSecondSpace, intIndexLineTermination;

				// Inicializa los argumentos de salida
					intNumberEMails = 0;
					intTotalSize = 0;
				// Envía el comando
					strResponse = Send("STAT", false);
				// Obtiene los índices donde se encuentran los datos en la respuesta del servidor
					GetIndexSpaces(strResponse, out intIndexFirstSpace, out intIndexSecondSpace, 
												 out intIndexLineTermination);
				// Recoge los detalles del buzón
					intNumberEMails = ConvertToInt(strResponse, intIndexFirstSpace + 1, intIndexSecondSpace);
					intTotalSize = ConvertToInt(strResponse, intIndexSecondSpace + 1, intIndexLineTermination);
    }

		/// <summary>
		///		Obtiene el tamaño de un correo
		/// </summary>
    public int GetMailSize(int intMessageIndex)
    { string strResponse = "";
      int intIndexFirstSpace, intIndexSecondSpace, intIndexLineTermination;

				// Envía el comando al servidor
					strResponse = Send("LIST " + intMessageIndex, false);
				// Obtiene los índices donde se encuentran los datos en la respuesta del servidor
					GetIndexSpaces(strResponse, out intIndexFirstSpace, out intIndexSecondSpace,
												 out intIndexLineTermination);
				// Devuelve el tamaño del buzón
					return ConvertToInt(strResponse, intIndexSecondSpace + 1, intIndexLineTermination);
    }

		/// <summary>
		///		Obtiene los índices donde se encuentran los espacios y la terminación de línea en una cadena
		/// </summary>
		private void GetIndexSpaces(string strResponse, out int intIndexFirstSpace, out int intIndexSecondSpace,
																out int intIndexLineTermination)
		{ // Obtiene los índices donde se encuentran los espacios
				intIndexFirstSpace = strResponse.IndexOf(" ", cnstStrOk.Length - 1);
        intIndexSecondSpace = strResponse.IndexOf(" ", intIndexFirstSpace + 1);
        intIndexLineTermination = strResponse.IndexOf(cnstStrNewLine, intIndexSecondSpace + 1);
			// Comprueba los errores
        if (intIndexFirstSpace == -1 || intIndexSecondSpace == -1 || intIndexLineTermination == -1)
					throw new Pop3ClientException("Server sent an invalid reply for command.");
		}
		
		/// <summary>
		///		Convierte una cadena en un entero
		/// </summary>
		private int ConvertToInt(string strValue, int intStart, int intEnd)
		{ int intValue = 0;
		
				// Obtiene el valor
					if (!int.TryParse(strValue.Substring(intStart, intEnd - intStart), out intValue))
						return 0;
					else
						return intValue;
		}

		/// <summary>
		///		Borra un correo del servidor
		/// </summary>
    public void Delete(int intMessageIndex)
    {	Send("DELE " + intMessageIndex, false);
    }

		/// <summary>
		///		Quita las marcas de borrado
		/// </summary>
    public void Undelete()
    {	Send("RSET", false);
    }

		/// <summary>
		///		Recibe un correo
		/// </summary>
    public string FetchEmail(int intMessageIndex)
    {	return FetchEmail(intMessageIndex, true);
    }

		/// <summary>
		///		Recibe la cabecera de un correo
		/// </summary>
		public string FetchEMailHeader(int intMessageIndex)
		{ return FetchEmail(intMessageIndex, false);
		}
		
		/// <summary>
		///		Recibe un correo (completo o sólo la cabecera)
		/// </summary>
		private string FetchEmail(int intMessageIndex, bool blnFull)
		{ string strResponse;
			int intIndex;

				// Envía el comando al servidor	
					if (blnFull) // ... recupera todo el correo
						strResponse = Send("RETR " + intMessageIndex, true);
					else // ... recupera las primeras líneas del correo
						strResponse =  Send("TOP " + intMessageIndex + " " + 0, true);
				// Obtiene el contenido del correo
					intIndex = strResponse.IndexOf(cnstStrNewLine);
				// Devuelve el correo interpretado
					return strResponse.Substring(intIndex + cnstStrNewLine.Length, 
																			 strResponse.Length - intIndex - cnstStrNewLine.Length).Replace("\0", "");;
		}
    
    /// <summary>
    ///		Envía un mensaje y recibe la respuesta
    /// </summary>
    private string Send(string strMessage, bool blnResponseMultiline)
    {	string strResponse = "";
			bool blnWaitDataAvailable = true;

				// Lanza el evento de envío de un comando
					RaiseEvent(Pop3EventArgs.ActionType.SendedCommand, strMessage);
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
					RaiseEvent(Pop3EventArgs.ActionType.ServerResponse, strResponse);
				// Comprueba la respuesta del servidor
					if (string.IsNullOrEmpty(strResponse) ||
							!strResponse.StartsWith(cnstStrOk, StringComparison.OrdinalIgnoreCase))
						TreatError(strResponse);
				// Devuelve la respuesta (si todo ha ido bien)
					return strResponse;
		}

		/// <summary>
		///		Comprueba si se ha finalizado una respuesta
		/// </summary>
		private bool IsEndResponse(bool blnResponseMultiline, string strResponse)
		{ return !blnResponseMultiline ||
						 (blnResponseMultiline && 
								((strResponse.EndsWith("." + cnstStrNewLine) || 
									strResponse.EndsWith("." + "\n")) &&
 									!strResponse.EndsWith(cnstStrNewLine + cnstStrNewLine + ".")));
		}

		/// <summary>
		///		Trata un error: desconecta del servidor y lanza una excepción
		/// </summary>
		private void TreatError(string strError)
		{ // Desconecta
				ShutDown();
			// Lanza una excepción
				throw new Pop3ClientException(strError);
		}
		
		/// <summary>
		///		Lanza un evento
		/// </summary>
		private void RaiseEvent(Pop3EventArgs.ActionType intAction)
		{ RaiseEvent(intAction, null);
		}
		
		/// <summary>
		///		Lanza un evento
		/// </summary>
    private void RaiseEvent(Pop3EventArgs.ActionType intAction, string strDescription)
    {	if (Pop3Action != null)
        Pop3Action(this, new Pop3EventArgs(intAction, Server, strDescription));
    }

		/// <summary>
		///		Servidor Pop3
		/// </summary>
    public MailServer Server { get; set; }
	}
}
