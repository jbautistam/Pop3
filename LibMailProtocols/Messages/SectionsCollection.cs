using System;

namespace Bau.Libraries.LibMailProtocols.Messages
{
	/// <summary>
	///		Colección de secciones (<see cref="Section"/>
	/// </summary>
	public class SectionsCollection : System.Collections.Generic.List<Section>
	{
		/// <summary>
		///		Añade un nombre de archivo como adjunto
		/// </summary>
		public void Add(string strFileName)
		{ Section objSection = new Section();
		
				// Asigna las propiedades
					objSection.ContentType.Type = ContentType.ContentTypeEnum.Base64;
					objSection.TransferEncoding.TransferEncoding = ContentTransfer.ContentTransferEncoding.Base64;
				// Asigna la disposición de archivo
					objSection.ContentDisposition.Type = ContentDisposition.ContentDispositionEnum.Attachment;
					objSection.ContentDisposition.FileName = strFileName;
				// Añade la sección
					Add(objSection);
		}

		/// <summary>
		///		Busca las secciones de determinado tipo
		/// </summary>
		public SectionsCollection Search(ContentType.ContentTypeEnum intContentType)
		{ SectionsCollection objColSections = new SectionsCollection();
		
				// Recorre las secciones buscando las de determinado tipo
					foreach (Section objSection in this)
						{ // Comprueba si la sección es del tipo buscado
								if (objSection.ContentType.Type == intContentType)
									objColSections.Add(objSection);
							// Añade las secciones hijas
								if (objSection.Sections.Count > 0)
									objColSections.AddRange(objSection.Sections.Search(intContentType));
						}
				// Devuelve las secciones
					return objColSections;
		}

		/// <summary>
		///		Busca una sección a partir de su ID
		/// </summary>
		public Section Search(string strID)
		{ // Busca la sección
				foreach (Section objSection in this)
					if (objSection.ID.Equals(strID, StringComparison.CurrentCultureIgnoreCase))
						return objSection;
			// Si ha llegado hasta aquí es porque no han encontrado nada
				return null;
		}
	}
}
