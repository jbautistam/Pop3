using System;
using System.IO;

namespace Bau.Libraries.LibEncoder.Encoders.Tools
{
	/// <summary>
	///		Archivo de ayuda para grabación
	/// </summary>
	internal static class FileHelper
	{
		/// <summary>
		///		Graba los datos en un archivo
		/// </summary>
		internal static void Save(string strMessage, string strFileName)
		{ Save(System.Text.Encoding.UTF8.GetBytes(strMessage), strFileName);
		}

		/// <summary>
		///		Graba los datos en un archivo
		/// </summary>
		internal static void Save(byte [] arrBytSource, string strFileName)
		{ using (FileStream fsOutput = new FileStream(strFileName, FileMode.CreateNew, FileAccess.Write,
																									FileShare.None))
				{ // Escribe los bytes en el stream
						fsOutput.Write(arrBytSource, 0, arrBytSource.Length);
					// Cierra el stream de salida
						fsOutput.Close();
				}
		}
	}
}
