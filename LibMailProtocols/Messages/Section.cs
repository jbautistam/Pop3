using System;

namespace Bau.Libraries.LibMailProtocols.Messages
{
	/// <summary>
	///		Seccion de un mensaje de correo
	/// </summary>
	public class Section
	{ // Variables privadas
			private string strID;
			
		public Section()
		{ ContentType = new ContentType();
			ContentDisposition = new ContentDisposition();
			TransferEncoding = new ContentTransfer();
			Content = "";
			Headers = new HeadersCollection();
			Sections = new SectionsCollection();
		}
		
		/// <summary>
		///		Graba la sección
		/// </summary>
		public void Save(string strFileName)
		{ if (TransferEncoding.TransferEncoding == ContentTransfer.ContentTransferEncoding.Base64)
				LibEncoder.Encoder.DecodeToFile(LibEncoder.Encoder.EncoderType.Base64, Content, strFileName);
			else if (TransferEncoding.TransferEncoding == ContentTransfer.ContentTransferEncoding.QuotedPrintable)
				LibEncoder.Encoder.DecodeToFile(LibEncoder.Encoder.EncoderType.QuotedPrintable, Content, strFileName);
			else
				{ using (System.IO.FileStream fsOutput = new System.IO.FileStream(strFileName, System.IO.FileMode.CreateNew,
																																					System.IO.FileAccess.Write,
																																					System.IO.FileShare.None))
						{ byte [] arrBytBuffer = System.Text.Encoding.UTF8.GetBytes(Content);
						
								// Escribe los bytes en el stream
									fsOutput.Write(arrBytBuffer, 0, arrBytBuffer.Length);
								// Cierra el stream de salida
									fsOutput.Close();																	
						}
				}
		}

		/// <summary>
		///		Identificador de la sección
		/// </summary>
		public string ID
		{ get
				{ // Si no se le ha asignado un ID se le añade una
						if (string.IsNullOrEmpty(strID))
							strID = Guid.NewGuid().ToString();
					// Devuelve el ID
						return strID;
				}
			set { strID = value; }
		}
		
		/// <summary>
		///		Tipo de contenido de la sección
		/// </summary>
		public ContentType ContentType { get; set; }
	
		/// <summary>
		///		Disposición de la sección
		/// </summary>
		public ContentDisposition ContentDisposition { get; set; }
		
		/// <summary>
		///		Codificación
		/// </summary>
		public ContentTransfer TransferEncoding { get; set; }
		
		/// <summary>
		///		Contenido de la sección
		/// </summary>
		public string Content { get; set; }
		
		/// <summary>
		///		Cabeceras de la sección
		/// </summary>
		public HeadersCollection Headers { get; set; }
		
		/// <summary>
		///		Secciones de la sección
		/// </summary>
		public SectionsCollection Sections { get; private set; }
	}
}
