using System;

namespace Bau.Libraries.LibMailProtocols
{
	/// <summary>
	///		Clase con los datos de un servidor
	/// </summary>
	public class MailServer
	{
		public MailServer() : this(null, 110, null, null, false, 60) {}
		
		public MailServer(string strAddress, int intPort, string strUser, string strPassword) 
												: this(strAddress, intPort, strUser, strPassword, false, 60) {}

		public MailServer(string strAddress, int intPort, string strUser, string strPassword,
											bool blnUseSSL) : this(strAddress, intPort, strUser, strPassword, blnUseSSL, 60) {}
		
		public MailServer(string strAddress, int intPort, string strUser, string strPassword,
											bool blnUseSSL, int intTimeOut)
		{ Address = strAddress;
			Port = intPort;
			UseSSL = blnUseSSL;
			User = strUser;
			Password = strPassword;
			TimeOut = intTimeOut;
		}
		
		/// <summary>
		///		Dirección o nombre del servidor
		/// </summary>
		public string Address { get; set; }
		
		/// <summary>
		///		Puerto del servidor
		/// </summary>
		public int Port { get; set; }
		
		/// <summary>
		///		Usuario del servidor
		/// </summary>
		public string User { get; set; }
		
		/// <summary>
		///		Contraseña del usuario
		/// </summary>
		public string Password { get; set; }
		
		/// <summary>
		///		Indica si debe utilizar conexiones seguras
		/// </summary>
		public bool UseSSL { get; set; }
		
		/// <summary>
		///		Tiempo de espera máximo (en segundos)
		/// </summary>
		public int TimeOut { get; set; }
	}
}
