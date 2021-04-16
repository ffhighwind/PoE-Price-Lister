#region License
// Copyright © 2018 Wesley Hamilton
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/
#endregion

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace PoE_Price_Lister
{
	public static class Util
	{
		public static void DoubleBuffer(this DataGridView dgv)
		{
			typeof(DataGridView).InvokeMember(
			   "DoubleBuffered",
			   BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
			   null,
			   dgv,
			   new object[] { true });
		}

		/// <summary>
		/// Detects the <see cref="Encoding"/> for UTF-7, UTF-8/16/32 (bom, no bom, little & big endian),
		/// the local default codepage, and others.
		/// </summary>
		/// <param name="path">The file to detect <see cref="Encoding"/> of.</param>
		/// <returns>The text of the file after it has been read.</returns>
		public static string GetEncodedText(string path)
		{
			return GetEncodedText(path, out Encoding encoding);
		}

		/// <summary>
		/// Detects the <see cref="Encoding"/> for UTF-7, UTF-8/16/32 (bom, no bom, little & big endian),
		/// the local default codepage, and others.
		/// </summary>
		/// <param name="path">The file to detect <see cref="Encoding"/> of.</param>
		/// <param name="encoding">The <see cref="Encoding"/> of the file.</param>
		/// <param name="maxBytes">The number of bytes to check of the file. Higher value is slower
		/// but more reliable (especially UTF-8 with special characters later on may appear to be ASCII initially).
		/// If negative then the whole file is read in (maximum reliability)</param>
		/// <returns>The text of the file after it has been processed for encoding.</returns>
		public static string GetEncodedText(string path, out Encoding encoding, int maxBytes = -1)
		{
			//////////// If the code reaches here, no BOM/signature was found, so now
			//////////// we need to 'taste' the file to see if can manually discover
			//////////// the encoding. A high taster value is desired for UTF-8
			byte[] b;
			if (maxBytes < 0)
				b = System.IO.File.ReadAllBytes(path);
			else {
				b = new byte[maxBytes];
				using (System.IO.FileStream fs = System.IO.File.OpenRead(path)) {
					fs.Read(b, 0, maxBytes);
				}
			}
			encoding = GetTextEncoding(b, out int index);
			return encoding.GetString(b, index, b.Length - index);
		}

		/// <summary>
		/// Creates a <see cref="System.IO.TextReader"/> from the path and attempts to detect the file <see cref="Encoding"/>.
		/// </summary>
		/// <param name="path">The path of the file.</param>
		/// <param name="maxBytesRead">The maximum number of bytes to read at once.
		/// If the file is bigger than this size then it is read as a Stream.</param>
		/// <returns>The <see cref="System.IO.TextReader"/> with the file <see cref="Encoding"/> automatically detected.</returns>
		public static TextReader TextReader(string path, int maxBytesRead = 100000000)
		{
			return TextReader(new FileInfo(path), maxBytesRead);
		}

		/// <summary>
		/// Creates a <see cref="System.IO.TextReader"/> from the path and attempts to detect the file <see cref="Encoding"/>.
		/// </summary>
		/// <param name="fi">The <see cref="FileInfo"/> of the file.</param>
		/// <param name="maxBytesRead">The maximum number of bytes to read at once.
		/// If the file is bigger than this size then it is read as a Stream.</param>
		/// <returns>The <see cref="System.IO.TextReader"/> with the file <see cref="Encoding"/> automatically detected.</returns>
		public static TextReader TextReader(FileInfo fi, int maxBytesRead = 100000000)
		{
			if (!fi.Exists)
				throw new FileNotFoundException(fi.FullName);
			if (fi.Length > maxBytesRead)
				return new StreamReader(fi.FullName, GetEncoding(fi.FullName), true);
			return new StringReader(GetEncodedText(fi.FullName));
		}

		/// <summary>
		/// Attempts to detect the <see cref="Encoding"/> of a file.
		/// </summary>
		/// <param name="path">The path of the file.</param>
		/// <param name="maxBytes">The maximum number of bytes to read for detecting <see cref="Encoding"/>.</param>
		/// <returns>The <see cref="Encoding"/> of the file.</returns>
		public static Encoding GetEncoding(string path, int maxBytes = 1000)
		{
			//////////// If the code reaches here, no BOM/signature was found, so now
			//////////// we need to 'taste' the file to see if can manually discover
			//////////// the encoding. A high taster value is desired for UTF-8
			byte[] b = new byte[maxBytes];
			using (System.IO.FileStream fs = System.IO.File.OpenRead(path)) {
				fs.Read(b, 0, maxBytes);
			}
			return GetTextEncoding(b, out int _);
		}

		/// <summary>
		/// Gets the file <see cref="Encoding"/> from a <see cref="byte"/>[].
		/// </summary>
		/// <param name="b">The <see cref="byte"/>[] to detect <see cref="Encoding"/> for.</param>
		/// <param name="index">The start of the file. This will be after the <see cref="Encoding"/> BOM/signature if one exists.</param>
		/// <returns>The <see cref="Encoding"/> of the <see cref="byte"/>[].</returns>
		/// <source>https://stackoverflow.com/questions/1025332/determine-a-strings-encoding-in-c-sharp</source>
		private static Encoding GetTextEncoding(byte[] b, out int index)
		{
			//////////////// First check the low hanging fruit by checking if a
			//////////////// BOM/signature exists (sourced from http://www.unicode.org/faq/utf_bom.html#bom4)
			if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF) {
				index = 4;
				return Encoding.GetEncoding("utf-32BE"); // UTF-32, big-endian
			}
			else if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00) {
				index = 4;
				return Encoding.UTF32; // UTF-32, little-endian
			}
			else if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF) {
				index = 2;
				return Encoding.BigEndianUnicode; // UTF-16, big-endian
			}
			else if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) {
				index = 2;
				return Encoding.Unicode; // UTF-16, little-endian
			}
			else if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) {
				index = 3;
				return Encoding.UTF8;
			}
			else if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76) {
				index = 3;
				return Encoding.UTF7;
			}
			index = 0;

			// Some text files are encoded in UTF8, but have no BOM/signature. Hence
			// the below manually checks for a UTF8 pattern. This code is based off
			// the top answer at: https://stackoverflow.com/questions/6555015/check-for-invalid-utf8
			// For our purposes, an unnecessarily strict (and terser/slower)
			// implementation is shown at: https://stackoverflow.com/questions/1031645/how-to-detect-utf-8-in-plain-c
			// For the below, false positives should be exceedingly rare (and would
			// be either slightly malformed UTF-8 (which would suit our purposes
			// anyway) or 8-bit extended ASCII/UTF-16/32 at a vanishingly long shot).
			int i = 0;
			bool utf8 = false;
			while (i < b.Length - 4) {
				if (b[i] <= 0x7F) {
					i += 1;
					continue;
				}     // If all characters are below 0x80, then it is valid UTF8, but UTF8 is not 'required' (and therefore the text is more desirable to be treated as the default codepage of the computer). Hence, there's no "utf8 = true;" code unlike the next three checks.
				if (b[i] >= 0xC2 && b[i] <= 0xDF && b[i + 1] >= 0x80 && b[i + 1] < 0xC0) {
					i += 2;
					utf8 = true;
					continue;
				}
				if (b[i] >= 0xE0 && b[i] <= 0xF0 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0) {
					i += 3;
					utf8 = true;
					continue;
				}
				if (b[i] >= 0xF0 && b[i] <= 0xF4 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0 && b[i + 3] >= 0x80 && b[i + 3] < 0xC0) {
					i += 4;
					utf8 = true;
					continue;
				}
				utf8 = false;
				break;
			}
			if (utf8)
				return Encoding.UTF8;

			// The next check is a heuristic attempt to detect UTF-16 without a BOM.
			// We simply look for zeroes in odd or even byte places, and if a certain
			// threshold is reached, the code is 'probably' UF-16.
			double threshold = 0.1; // proportion of chars step 2 which must be zeroed to be diagnosed as utf-16. 0.1 = 10%
			int count = 0;
			for (int n = 0; n < b.Length; n += 2) {
				if (b[n] == 0)
					count++;
			}
			if (((double) count) / b.Length > threshold)
				return Encoding.BigEndianUnicode;
			count = 0;
			for (int n = 1; n < b.Length; n += 2) {
				if (b[n] == 0)
					count++;
			}
			if (((double) count) / b.Length > threshold)
				return Encoding.Unicode; // (little-endian)

			// Finally, a long shot - let's see if we can find "charset=xyz" or
			// "encoding=xyz" to identify the encoding:
			for (int n = 0; n < b.Length - 9; n++) {
				if (((b[n + 0] == 'c' || b[n + 0] == 'C') && (b[n + 1] == 'h' || b[n + 1] == 'H')
					&& (b[n + 2] == 'a' || b[n + 2] == 'A') && (b[n + 3] == 'r' || b[n + 3] == 'R')
					&& (b[n + 4] == 's' || b[n + 4] == 'S') && (b[n + 5] == 'e' || b[n + 5] == 'E')
					&& (b[n + 6] == 't' || b[n + 6] == 'T') && (b[n + 7] == '='))
					||
					((b[n + 0] == 'e' || b[n + 0] == 'E')
					&& (b[n + 1] == 'n' || b[n + 1] == 'N')
					&& (b[n + 2] == 'c' || b[n + 2] == 'C')
					&& (b[n + 3] == 'o' || b[n + 3] == 'O')
					&& (b[n + 4] == 'd' || b[n + 4] == 'D')
					&& (b[n + 5] == 'i' || b[n + 5] == 'I')
					&& (b[n + 6] == 'n' || b[n + 6] == 'N')
					&& (b[n + 7] == 'g' || b[n + 7] == 'G')
					&& (b[n + 8] == '='))) {
					if (b[n + 0] == 'c' || b[n + 0] == 'C')
						n += 8;
					else
						n += 9;
					if (b[n] == '"' || b[n] == '\'')
						n++;
					int oldn = n;
					while (n < b.Length && (b[n] == '_'
						|| b[n] == '-' || (b[n] >= '0' && b[n] <= '9')
						|| (b[n] >= 'a' && b[n] <= 'z') || (b[n] >= 'A' && b[n] <= 'Z'))) {
						n++;
					}
					byte[] nb = new byte[n - oldn];
					Array.Copy(b, oldn, nb, 0, n - oldn);
					try {
						return Encoding.GetEncoding(Encoding.ASCII.GetString(nb));
					}
					catch {
						break; // C# doesn't recognize the name of the encoding
					}
				}
			}

			// If all else fails, the encoding is probably (though certainly not
			// definitely) the user's local codepage! One might present to the user a
			// list of alternative encodings as shown here: https://stackoverflow.com/questions/8509339/what-is-the-most-common-encoding-of-each-language
			//// A full list can be found using Encoding.GetEncodings();
			return Encoding.Default;
		}

		public static string ReadWebPage(string url, string headerMedia = "", Encoding encoding = null)
		{
			using (HttpClient client = new HttpClient()) {
				client.BaseAddress = new Uri(url);
				if (headerMedia.Length > 0)
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(headerMedia));
				HttpResponseMessage response;
				try {
					response = client.GetAsync(url).Result;
					if (response.IsSuccessStatusCode) {
						return encoding == null ? response.Content.ReadAsStringAsync().Result
							: encoding.GetString(response.Content.ReadAsByteArrayAsync().Result);
					}
				}
				catch {
					return null;
				}
				return null;
			}
		}
	}
}
