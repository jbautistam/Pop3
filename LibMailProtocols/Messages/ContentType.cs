using System;

namespace Bau.Libraries.LibMailProtocols.Messages
{
	/// <summary>
	///		Tipo de contenido
	/// </summary>
	public class ContentType
	{ // Constantes internas con el tipo de contenido
			internal const string cnstStrContentTypeBase64 = "base64";
			internal const string cnstStrContentTypeHTML = "text/html";
			internal const string cnstStrContentTypeMultipart = "multipart";
			internal const string cnstStrContentTypeMultipartAlternative = "multipart/alternative";
			internal const string cnstStrContentTypeMultipartMixed = "multipart/mixed";
			internal const string cnstStrContentTypeMultipartRelated = "multipart/related";
			internal const string cnstStrContentTypeMultipartReport = "multipart/report";
			internal const string cnstStrContentTypeText = "text/plain";
			internal const string cnstStrContentTypeOctectStream = "application/octet-stream";
		// Tipos de contenido
			public enum ContentTypeEnum
				{ Unknown,
					Base64,
					HTML,
					MultipartAlternative,
					MultipartMixed,
					MultipartRelated,
					MultipartReport,
					Multipart,
					OctectStream,
					Other,
					Text
				}
		// Variables privadas
			private string strContentType;
			
		public ContentType()
		{
		}

		/// <summary>
		///		Obtiene la descripción de un tipo de contenido
		/// </summary>
		internal static string GetContentType(ContentTypeEnum intContentType)
		{ switch (intContentType)
				{	case ContentTypeEnum.Base64:
						return cnstStrContentTypeBase64;
					case ContentTypeEnum.HTML:
						return cnstStrContentTypeHTML;
					case ContentTypeEnum.MultipartAlternative:
						return cnstStrContentTypeMultipartAlternative;
					case ContentTypeEnum.MultipartMixed:
						return cnstStrContentTypeMultipartMixed;
					case ContentTypeEnum.MultipartRelated:
						return cnstStrContentTypeMultipartRelated;
					case ContentTypeEnum.MultipartReport:
						return cnstStrContentTypeMultipartReport;
					case ContentTypeEnum.Multipart:
						return cnstStrContentTypeMultipart;
					case ContentTypeEnum.OctectStream:
						return cnstStrContentTypeOctectStream;
					case ContentTypeEnum.Text:
						return cnstStrContentTypeText;
					default:
						return "";
				}
		}
		
		/// <summary>
		///		Tipo de contenido
		/// </summary>
		public ContentTypeEnum Type { get; set; }
		
		/// <summary>
		///		Cadena con el tipo de contenido de la sección (sólo es necesario si es otros)
		/// </summary>
		public string ContentTypeDefinition 
		{ get
				{ string strContentTypeDefinition = GetContentType(Type);
				
						if (!string.IsNullOrEmpty(strContentTypeDefinition))
							return strContentTypeDefinition;
						else
							return strContentType;
				}
			set
				{ // Asigna el tipo de contenido 
						strContentType = value;
						Type = ContentTypeEnum.Other;
					// Comprueba si es alguno de los tipos de contenido identificados
						if (!string.IsNullOrEmpty(strContentType))
							{ // Limpia el tipo
									strContentType = strContentType.Trim();
								// Asigna el tipo
									if (strContentType.Equals(cnstStrContentTypeBase64, 
																						StringComparison.CurrentCultureIgnoreCase))
										Type = ContentTypeEnum.Base64;
									else if (strContentType.Equals(cnstStrContentTypeHTML, 
																								 StringComparison.CurrentCultureIgnoreCase))
										Type = ContentTypeEnum.HTML;
									else if (strContentType.Equals(cnstStrContentTypeMultipartAlternative,
																								 StringComparison.CurrentCultureIgnoreCase))
										Type = ContentTypeEnum.MultipartAlternative;
									else if (strContentType.Equals(cnstStrContentTypeMultipartMixed,
																								 StringComparison.CurrentCultureIgnoreCase))
										Type = ContentTypeEnum.MultipartMixed;
									else if (strContentType.Equals(cnstStrContentTypeMultipartRelated,
																								 StringComparison.CurrentCultureIgnoreCase))
										Type = ContentTypeEnum.MultipartRelated;
									else if (strContentType.Equals(cnstStrContentTypeMultipartReport,
																								 StringComparison.CurrentCultureIgnoreCase))
										Type = ContentTypeEnum.MultipartReport;
									else if (strContentType.StartsWith(cnstStrContentTypeMultipart,
																										 StringComparison.CurrentCultureIgnoreCase))
										Type = ContentTypeEnum.Multipart;
									else if (strContentType.Equals(cnstStrContentTypeOctectStream,
																								 StringComparison.CurrentCultureIgnoreCase))
										Type = ContentTypeEnum.OctectStream;
									else if (strContentType.Equals(cnstStrContentTypeText, 
																								 StringComparison.CurrentCultureIgnoreCase))
										Type = ContentTypeEnum.Text;
							}						
				}
		}

		/// <summary>
		///		Conjunto de caracteres
		/// </summary>
		public string CharSet { get; set; }
		
		/// <summary>
		///		Boundary del mensaje
		/// </summary>
		public string Boundary { get; set; }
		
		/// <summary>
		///		Indica si es una sección múltiple
		/// </summary>
		public bool IsMultipart
		{ get 
				{ return Type == ContentTypeEnum.Multipart || Type == ContentTypeEnum.MultipartAlternative ||
								 Type == ContentTypeEnum.MultipartMixed || Type == ContentTypeEnum.MultipartRelated; 
				}
		}
	}
}
