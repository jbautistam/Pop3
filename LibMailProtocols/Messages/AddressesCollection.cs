using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibMailProtocols.Messages
{
	/// <summary>
	///		Colección de <see cref="Address"/>
	/// </summary>
	public class AddressesCollection : List<Address>
	{
		public AddressesCollection() {}
		
		public AddressesCollection(string strAddresses)
		{ Add(strAddresses);
		}
		
		/// <summary>
		///		Añade una dirección a la colección
		/// </summary>
		public void Add(string strName, string strEMail)
		{ Add(new Address(strName, strEMail));
		}
		
		/// <summary>
		///		Añade una serie de direcciones
		/// </summary>
		/// <example>
		/// jbautistamontejo@gmail.com, jbautistam@ant2e6.site11.com, Jose Antonio Bautista <jbautistam@gmail.com>
		/// </example>
		public void Add(string strAddresses)
		{ if (!string.IsNullOrEmpty(strAddresses))
				{ string [] arrStrAddress = strAddresses.Split(',');
				
						foreach (string strAddress in arrStrAddress)
							if (!string.IsNullOrEmpty(strAddress) && !string.IsNullOrEmpty(strAddress.Trim()))
								Add(new Address(strAddress.Trim()));
				}
		}
		
		/// <summary>
		///		Cadena con las direcciones
		/// </summary>
		public string FullEMail
		{ get
				{ string strEMail = "";
				
						// Añade las direcciones
							foreach (Address objAddress in this)
								{ // Añade el separador
										if (!string.IsNullOrEmpty(strEMail))
											strEMail += ", ";
									// Añade la dirección
										strEMail += objAddress.FullEMail;
								}
						// Devuelve las direcciones
							return strEMail;
				}
		}
	}
}
