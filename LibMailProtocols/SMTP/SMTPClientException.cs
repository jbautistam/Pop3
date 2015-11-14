using System;

namespace Bau.Libraries.LibMailProtocols.SMTP
{
	/// <summary>
	///		Excepci�n de comunicaci�n con un servidor SMTP
	/// </summary>
  public class SMTPClientException : Exception
  {
		public SMTPClientException() : this(null, null) {}
		
		public SMTPClientException(string strMessage) : this(strMessage, null) {}
		
		public SMTPClientException(string strMessage, Exception objInnerException) 
							: base(strMessage, objInnerException) {}
  }
}
