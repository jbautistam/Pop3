using System;
using System.Text;

namespace Bau.Libraries.LibMailProtocols.Tools.Encoder
{
	/// <summary>
	///		Codificación Quoted Print
	/// </summary>
	public static class QuotedPrint
	{
        public static string Encode(string s, string charset)
        {
            if (s == null) { return null; }
            if (charset == null) charset = "ISO-8859-1"; // default charset

            // environment newline char
            char[] nl = Environment.NewLine.ToCharArray();

            // source char array
            char[] source = s.ToCharArray();
            char ch;

            StringBuilder sb = new StringBuilder();
            StringBuilder currLine = new StringBuilder();
            bool bNewline = false;

            Encoding oEncoding = Encoding.GetEncoding(charset);

            for (int sidx = 0; sidx < s.Length; sidx++)
            {
                ch = source[sidx];

                // RULE # 4: LINE BREAKS
                if (ch == nl[0] && sidx <= (s.Length - nl.Length))
                {
                    // peek ahead make sure the "whole" newline is present
                    if (s.Substring(sidx, nl.Length) == Environment.NewLine)
                    {
                        // RULE #3: ENCODE WHITESPACE PRECEEDING A HARD BREAK
                        if (currLine.Length > 0)
                        {
                            if (source[sidx - 1] == ' ')
                            {   // if char is preceded by space char add =20
                                currLine.Remove(currLine.Length - 1, 1);
                                currLine.Append("=20");
                            }
                            else if (source[sidx - 1] == '\t')
                            {   // if char is preceded by tab char add =09
                                currLine.Remove(currLine.Length - 1, 1);
                                currLine.Append("=09");
                            }
                        }

                        // flag for new line
                        bNewline = true;
                        sidx += nl.Length - 1;  // jump ahead 

                    }
                    else
                    {	// not actually a newline.  Encode per Rule #1
                        currLine.Append("=0" + Convert.ToString((byte)ch, 16).ToUpper());
                    }
                }
                // RULE #1 and #2
                // Optional characters are: !"#$@[\]^`{|}~
                else if (ch > 126 || (ch < 32 && ch != '\t') || ch == '=')
                {
                    byte[] outByte = new byte[10];
                    int iCount = oEncoding.GetBytes("" + ch, 0, 1, outByte, 0);

                    for (int i = 0; i < iCount; i++)
                    {
                        if (outByte[i] < 16)
                            currLine.Append("=0" + Convert.ToString(outByte[i], 16).ToUpper());
                        else
                            currLine.Append("=" + Convert.ToString(outByte[i], 16).ToUpper());
                    }
                }
                else
                {
                    currLine.Append(ch);
                }

                // Rule #5: MAXIMUM length 76 characters per line
                if (currLine.Length >= 76)
                {
                    // just make sure not to split an encoded char
                    string cLine = currLine.ToString();
                    int breakAt = cLine.LastIndexOf("=");
                    if (breakAt < 70) breakAt = 74;
                    sb.Append(cLine.Substring(0, breakAt) + "=\r\n");
                    currLine = new StringBuilder(cLine.Substring(breakAt));
                }

                if (bNewline)
                {
                    // RFC 822 linebreak = CRLF
                    sb.Append(currLine.ToString() + "\r\n");
                    currLine = new StringBuilder();
                    bNewline = false;
                }
            }

            // add last line
            sb.Append(currLine.ToString());

            return sb.ToString();
        }

	
	}
}
