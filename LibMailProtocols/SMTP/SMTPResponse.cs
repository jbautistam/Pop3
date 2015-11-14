using System;

namespace Bau.Libraries.LibMailProtocols.SMTP
{
	/// <summary>
	///		Clase con los datos de una respuesta de un servidor SMTP
	/// </summary>
	internal class SMTPResponse
	{
		internal SMTPResponse(string strMessage)
		{ Parse(strMessage);
		}
		
		/// <summary>
		///		Interpreta el mensaje
		/// </summary>
		internal void Parse(string strMessage)
		{ int intCode = -1;
		
				if (!string.IsNullOrEmpty(strMessage) && strMessage.Length > 4)
					{ if (int.TryParse(strMessage.Substring(0, 3), out intCode))
							{ // Se ha interpretado correctamente. Se guarda el código y el mensaje
									Code = intCode;
									Message = strMessage.Substring(4);
							}
						else
							{ Code = -1;
								Message = strMessage;
							}
					}
		}
		
		/// <summary>
		///		Código de la respuesta
		/// </summary>
		internal int Code { get; set; }
		
		/// <summary>
		///		Mensaje de la respuesta
		/// </summary>
		internal string Message { get; set; }
		
		/// <summary>
		///		Indica si el mensaje es una respuesta correcta
		/// </summary>
		internal bool IsOk 
		{ get { return Code >= 200 && Code <= 299; }
		}
		
		/// <summary>
		///		Indica si el mensaje es una respuesta errónea
		/// </summary>
		internal bool IsFatalError
		{ get { return Code >= 500 && Code <= 599; }
		}
	}
}
