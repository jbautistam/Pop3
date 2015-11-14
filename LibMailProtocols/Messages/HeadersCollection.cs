using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibMailProtocols.Messages
{
	/// <summary>
	///		Colección e <see cref="Header"/>
	/// </summary>
	public class HeadersCollection : List<Header>
	{
		/// <summary>
		///		Añade una cabecera a la colección
		/// </summary>
		public void Add(string strName, string strValue)
		{ Add(new Header(strName, strValue));
		}

		/// <summary>
		///		Busca una cabecera por su nombre
		/// </summary>
		public Header Search(string strName)
		{ // Recorre la colección buscando el elemento
				foreach (Header objHeader in this)
					if (objHeader.Name.Equals(strName, StringComparison.CurrentCultureIgnoreCase))
						return objHeader;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
				return null;
		}
		
		/// <summary>
		///		Comprueba si existe una cabecera por su nombre
		/// </summary>
		public bool Exists(string strName)
		{ return Search(strName) != null;
		}
	}
}
