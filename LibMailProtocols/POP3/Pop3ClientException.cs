using System;

namespace Bau.Libraries.LibMailProtocols.POP3
{
	/// <summary>
	///		Excepción de comunicación con un servidor POP3
	/// </summary>
  public class Pop3ClientException : Exception
  {
		public Pop3ClientException() : this(null, null) {}
		
		public Pop3ClientException(string strMessage) : this(strMessage, null) {}
		
		public Pop3ClientException(string strMessage, Exception objInnerException) 
							: base(strMessage, objInnerException) {}
  }
}
