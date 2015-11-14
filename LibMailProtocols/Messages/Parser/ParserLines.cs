using System;
using System.IO;

namespace Bau.Libraries.LibMailProtocols.Messages.Parser
{
	/// <summary>
	///		Intérprete de líneas
	/// </summary>
	internal class ParserLines
	{ // Variables privadas
			private StringReader objReader;
			private bool blnIsEof = false;
			
		/// <summary>
		///		Interpreta las líneas de un mensaje
		/// </summary>
		internal ParserLines(string strMessage)
		{ // Quita el punto final al mensaje
				if (strMessage.EndsWith(".\r\n"))
					strMessage = strMessage.Substring(0, strMessage.Length - 3);
				if (strMessage.EndsWith(".\n"))
					strMessage = strMessage.Substring(0, strMessage.Length - 2);
				else if (strMessage.EndsWith("."))
					strMessage = strMessage.Substring(0, strMessage.Length - 1);
			// Quita los espacios
				if (!string.IsNullOrEmpty(strMessage))
					strMessage = strMessage.Trim();
			// Crea el lector
				objReader = new StringReader(strMessage);
		}
		
		/// <summary>
		///		Lee una línea del buffer
		/// </summary>
		internal string ReadLine()
		{ string strLine = objReader.ReadLine();

				// Comprueba si es fin de archivo		
					if (strLine == null)
						blnIsEof = true;
				// Devuelve la línea
					return strLine;
		}
		
		/// <summary>
		///		Lee las líneas concatenando con las siguientes
		/// </summary>
		internal string ReadLineContinuous()
		{ string strLine = ReadLine();
		
				// Lee las siguientes líneas y las concatena
					if (strLine != "")
						while (objReader.Peek() == (int) ' ' || objReader.Peek() == (int) '\t')
							{ string strNextLine = ReadLine();
							
									if (!string.IsNullOrEmpty(strNextLine))
										strLine += " " + strNextLine.Trim();
							}
				// Devuelve las líneas concatenadas
					return strLine;
		}

		/// <summary>
		///		Indica si es un fin de archivo
		/// </summary>
		internal bool IsEof
		{ get { return objReader == null || blnIsEof; }
		}
	}
}
