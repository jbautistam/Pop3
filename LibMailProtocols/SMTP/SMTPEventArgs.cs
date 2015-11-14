using System;

namespace Bau.Libraries.LibMailProtocols.SMTP
{
	/// <summary>
	///		Argumentos de los eventos
	/// </summary>
	public class SMTPEventArgs : EventArgs
	{	// Enumerados
			public enum ActionType
				{ Unknown,
					ConnectionEstablishing,
					ConnectionEstablished,
					AuthenticationBegan,
					AuthenticationFinished,
					Disconnected,
					SendedCommand,
					ServerResponse
				}
		
		public SMTPEventArgs(ActionType intAction, MailServer objServer, string strDescription)
		{ Action = intAction;
			Server = objServer;
			Description = strDescription;
		}
		
		/// <summary>
		///		Acción realizada con el servidor
		/// </summary>
		public ActionType Action { get; private set; }
		
		/// <summary>
		///		Servidor del que se envía o recibe un mensaje
		/// </summary>
		public MailServer Server { get; private set; }
		
		/// <summary>
		///		Descripción del evento
		/// </summary>
		public string Description { get; private set; }
	}
}
