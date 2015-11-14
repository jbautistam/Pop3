using System;

namespace Bau.Libraries.LibMailProtocols.Messages
{
	/// <summary>
	///		Dirección de correo
	/// </summary>
	public class Address
	{ 
		public Address() : this(null, null) {}
		
		public Address(string strAddress)
		{ Parse(strAddress);
		}
		
		public Address(string strName, string strEMail)
		{ Name = strName;
			EMail = strEMail;
		}

		/// <summary>
		///		Interpreta una dirección de correo 
		/// </summary>
		/// <example>
		///		Jose Antonio Bautista <jbautistam@gmail.com>
		/// </example>
		private void Parse(string strAddress)
		{ if (!string.IsNullOrEmpty(strAddress))
				{ int intIndex = strAddress.LastIndexOf('<');
				
						if (intIndex < 0)
							{ Name = null;
								EMail = strAddress;
							}
						else
							{ int intIndexEnd = strAddress.LastIndexOf('>');
							
									Name = strAddress.Substring(0, intIndex);
									EMail = strAddress.Substring(intIndex + 1, intIndexEnd - intIndex - 1);
							}
				}
		}

		/// <summary>
		///		Comprueba una dirección de correo electrónico
		/// </summary>
		public static bool CheckEMail(string strEMail) 
		{ bool blnValid = false;
		
				// Comprueba si el correo es sintácticamente válido
					if (!string.IsNullOrEmpty(strEMail))
						{ string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
																	 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + 
																	 @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
							System.Text.RegularExpressions.Regex objExpression = new System.Text.RegularExpressions.Regex(strRegex);
							
								// Comprueba si el correo es correcto
									blnValid = objExpression.IsMatch(strEMail);
						}
				// Devuelve el valor que indica si el correo es válido
					return blnValid;
		}

		/// <summary>
		///		Nombre del usuario
		/// </summary>
		public string Name { get; set; }		
		
		/// <summary>
		///		Dirección del correo
		/// </summary>
		public string EMail { get; set; }
		
		/// <summary>
		///		Dirección completa de correo electrónico
		/// </summary>
		public string FullEMail
		{ get
				{ if (string.IsNullOrEmpty(Name))
						return EMail;
					else
						return Name + " <" + EMail + ">";
				}
		}
	}
}
