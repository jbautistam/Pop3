using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace Bau.Libraries.LibMailProtocols.Tools.Network
{
	/// <summary>
	///		Clase con las rutinas de conexión a la red mediante sockets
	/// </summary>
	internal class Connection
	{	// Constantes privadas
			private const int cnstIntLengthBuffer = 1024;
		// Variables privadas
			private Socket sckTcp;
			private NetworkStream stmNetwork;
			private SslStream stmSecure;
			private System.Text.ASCIIEncoding objEncoding = new System.Text.ASCIIEncoding();
      
		internal Connection(MailServer objServer)
		{ Server = objServer;
		}
		
		/// <summary>
		///		Conecta al servidor
		/// </summary>
		internal void Connect()
		{ // Desconecta
				Disconnect();
			// Asigna el socket
				sckTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			// Conecta el socket
				ConnectSocket(Server.Address, Server.Port);
			// Crea el stream de datos
				stmNetwork = new NetworkStream(sckTcp);
			// Crea el stream seguro si es necesario
				if (Server.UseSSL)
					{ // Crea el stream de datos seguros
							stmSecure = new SslStream(stmNetwork, true);
						// Autentifica el cliente en una conexión segura
							stmSecure.AuthenticateAsClient(Server.Address);
					}
		}
		
		/// <summary>
		///		Conecta el socket a un servidor
		/// </summary>
		private void ConnectSocket(string strAddress, int intPort)
		{ IPAddress[] arrIPAddresses;

				// Obtiene la dirección a partir de la DNS
					try
						{	arrIPAddresses = Dns.GetHostAddresses(strAddress);
						}
					catch
						{	throw new NetworkException("Server \"" + strAddress  + "\" does not exist.");
						}
				// Conecta al punto remoto
					try
						{	sckTcp.Connect((EndPoint) new IPEndPoint(arrIPAddresses[0], intPort));
							sckTcp.LingerState  = new LingerOption(true, 20);
							sckTcp.SendTimeout = 40000;
							sckTcp.SendBufferSize = 8192;
							sckTcp.ReceiveTimeout = 40000;
							sckTcp.ReceiveBufferSize = 8192;
						}
					catch (Exception objException)
						{	throw new NetworkException("Unable to connect to server: " + strAddress + 
																						", on port " + intPort + ".\n" + objException.Message);              
						}
		}
		
		/// <summary>
		///		Desconecta de la red
		/// </summary>
		internal void Disconnect()
		{ // Cierra el stream de red
				if (stmNetwork != null)
					stmNetwork.Close(Server.TimeOut);
				if (stmSecure != null)
					stmSecure.Close();
			// Cierra el socket
				if (sckTcp != null && sckTcp.Connected)
					sckTcp.Close();
			// Libera la memoria
				sckTcp = null;
        stmNetwork = null;
        stmSecure = null;
		}
		
		/// <summary>
		///		Envía un mensaje
		/// </summary>
    internal void Send(string strMessage)
    {	byte[] arrBytBuffer = objEncoding.GetBytes(strMessage);
			
				if (Server.UseSSL)
					stmSecure.Write(arrBytBuffer, 0, arrBytBuffer.Length); 
				else
					stmNetwork.Write(arrBytBuffer, 0, arrBytBuffer.Length);
    }
    
    /// <summary>
    ///		Recibe un mensaje del servidor directamente (blnLockRead = true) o
		///	comprobando antes si se han recibido data
    /// </summary>
    internal string Receive(bool blnWaitDataAvailable)
    { if (blnWaitDataAvailable)
				return Receive();
			else
				return ReceiveLocking();
    }
    
		/// <summary>
		///		Recibe un mensaje del servidor
		/// </summary>
    internal string Receive()
    {	DateTime dtmStart = DateTime.Now;

				// Espera hasta que haya datos disponibles
					while (!stmNetwork.DataAvailable && !CheckTimeOut(dtmStart))
						{ // Permite procesar los eventos
								System.Threading.Thread.Sleep(0);
							// Espera 1 segundo 
								System.Threading.Thread.Sleep(1000);
						}
				// Recoge los datos
					if (stmNetwork.DataAvailable) // ... puede haber salido por el timeOut
						return ReceiveLocking();
					else
						throw new NetworkException("Connection timeout. Server unavailable");
    }
    
    /// <summary>
    ///		Recibe un mensaje esperando los datos
    /// </summary>
    internal string ReceiveLocking()
    { System.Text.StringBuilder sbResponse = new System.Text.StringBuilder();
			byte[] arrBytBuffer = new byte[cnstIntLengthBuffer];

				// Lee los datos hasta que se acaba la transmisión
					do
						{	int intRead = 0;
						
								// Lee los datos
									if (Server.UseSSL)
										intRead = stmSecure.Read(arrBytBuffer, 0, arrBytBuffer.Length);
									else
										intRead = stmNetwork.Read(arrBytBuffer, 0, arrBytBuffer.Length);
								// Concatena los datos
									sbResponse.Append(objEncoding.GetString(arrBytBuffer, 0, intRead));
						}
					while (stmNetwork.DataAvailable);
				// Devuelve la cadena con la respuesta
					return sbResponse.ToString();
    }  
    
    /// <summary>
    ///		Comprueba si se ha sobrepasado el tiempo de espera
    /// </summary>
    private bool CheckTimeOut(DateTime dtmStart)
    { return (DateTime.Now - dtmStart).Minutes >= Server.TimeOut / 60;
    }

		/// <summary>
		///		Obtiene la dirección del ordenador local
		/// </summary>
		internal static string GetLocalHostName()
		{ return Dns.GetHostName();
		}
    
		/// <summary>
		///		Servidor con el que se realiza la conexión
		/// </summary>
		internal MailServer Server { get; private set; }
	}
}