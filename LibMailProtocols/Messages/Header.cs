using System;

namespace Bau.Libraries.LibMailProtocols.Messages
{
	/// <summary>
	///		Datos de una cabecera de correo
	/// </summary>
	public class Header
	{ // Constantes con los nombres básicos de las cabeceras
			internal const string cnstStrUndefined = "Undefined";
			internal const string cnstStrID = "Message-id";
			internal const string cnstStrContentType = "Content-Type";
			internal const string cnstStrFrom = "From";
			internal const string cnstStrTo = "To";
			internal const string cnstStrCC = "cc";
			internal const string cnstStrSubject = "subject";
			internal const string cnstStrDate = "date";
			internal const string cnstStrMimeVersion = "mime-version";
			internal const string cnstStrContentTransferEncoding = "Content-Transfer-Encoding";
			internal const string cnstStrContentDisposition = "Content-Disposition";
			internal const string cnstStrIDAttachment = "X-Attachment-Id";
			internal const string cnstStrContentTypeBoundary = "boundary";
			internal const string cnstStrCharSet = "charset";
			internal const string cnstStrFileName = "filename";
			
		public Header() : this(null, null) {}
		
		public Header(string strName, string strValue)
		{ Name = string.IsNullOrEmpty(strName) ? "" : strName.Trim();
			Value = string.IsNullOrEmpty(strValue) ? "" : strValue.Trim();
			SubHeaders = new HeadersCollection();
		}

		/// <summary>
		///		Busca el valor de una cabecera / subcabecera
		/// </summary>
		internal string SearchValue(string strName) 
		{ string strValue = "";

				// Obtiene el valor		
					if (Name.Equals(strName, StringComparison.CurrentCultureIgnoreCase))
						strValue = Value;
					else
						{ Header objHeader = SubHeaders.Search(strName);
						
								if (objHeader != null)
									strValue = objHeader.Value;
						}
				// Devuelve el valor encontrado
					return strValue;
		}

		/// <summary>
		///		Nombre de la cabecera
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		///		Valor de la cabecera
		/// </summary>
		public string Value { get; set; }
		
		/// <summary>
		///		Subcabeceras
		/// </summary>
		public HeadersCollection SubHeaders { get; set; }
	}
}
