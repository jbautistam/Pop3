using System;

namespace Bau.Libraries.LibMailProtocols.Tools.Network
{
	/// <summary>
	///		Excepciones de red
	/// </summary>
	public class NetworkException : Exception
	{
		public NetworkException(string strMessage) : base(strMessage) {}
	}
}
