﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kveer.XBeeApi.Utils
{
	/// <summary>
	/// Utility class containing methods to work with hexadecimal values and several  data type conversions.
	/// </summary>
	public class HexUtils
	{
		// Constants.
		private const string HEXES = "0123456789ABCDEF";
		private const string HEX_HEADER = "0x";

		/// <summary>
		/// Converts the given byte array into an hex string.
		/// </summary>
		/// <param name="value">Byte array to convert to hex string.</param>
		/// <returns>Converted byte array to hex string.</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="value"/> is null.</exception>
		/// <seealso cref="HexStringToByteArray"/>
		public static string ByteArrayToHexString(byte[] value)
		{
			Contract.Requires<ArgumentNullException>(value != null, "Value to convert cannot be null");

			return value.Aggregate(new StringBuilder(), (acc, b) => { acc.Append(HEXES[(b & 0xf0) >> 4]).Append(HEXES[b & 0xf]); return acc; }, (acc) => acc.ToString());
		}

		/// <summary>
		/// Converts the given byte into an hex string.
		/// </summary>
		/// <param name="value">Byte to convert to hex string.</param>
		/// <returns>Converted byte to hex string.</returns>
		public static string ByteToHexString(byte value)
		{
			StringBuilder hex = new StringBuilder(2);
			byte b = value;

			hex.Append(HEXES[(b & 0xF0) >> 4])
				.Append(HEXES[(b & 0x0F)]);

			return hex.ToString();
		}

		/// <summary>
		/// Converts the given hex string into a byte array.
		/// </summary>
		/// <param name="value">Hex string to convert to byte array.</param>
		/// <returns>Byte array of the given hex string.</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="value"/> is null.</exception>
		/// <seealso cref="ByteArrayToHexString"/>
		public static byte[] HexStringToByteArray(string value)
		{
			Contract.Requires<ArgumentNullException>(value != null, "Value to convert cannot be null.");

			value = value.Trim();
			if (value.StartsWith(HEX_HEADER))
				value = value.Substring(HEX_HEADER.Length);
			int len = value.Length;
			if (len % 2 != 0)
			{
				value = "0" + value;
				len = value.Length;
			}
			byte[] data = new byte[len / 2];
			for (int i = 0; i < len; i += 2)
			{
				data[i / 2] = byte.Parse(value.Substring(i, 2), NumberStyles.HexNumber);
			}
			return data;
		}

		/// <summary>
		/// Checks whether the given parameter is a string or a numeric value.
		/// </summary>
		/// <param name="parameter">Parameter to check.</param>
		/// <returns>true if the given parameter is a string, false otherwise.</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="parameter"/> is null.</exception>
		public static bool ContainsLetters(string parameter)
		{
			Contract.Requires<ArgumentNullException>(parameter != null, "Parameter cannot be null.");

			byte[] byteArray = Encoding.UTF8.GetBytes(parameter);

			return byteArray.Any(b => !(b >= '0' && b <= '9'));
		}

		/// <summary>
		/// Converts the given integer into an hexadecimal string.
		/// </summary>
		/// <param name="value">The integer value to convert to hexadecimal string.</param>
		/// <param name="minBytes">The minimum number of bytes to be represented.</param>
		/// <returns>The integer value as hexadecimal string.</returns>
		public static string IntegerToHexString(int value, int minBytes)
		{
			byte[] intAsByteArray = ByteUtils.IntToByteArray(value);
			String intAsHexString = "";
			bool numberFound = false;
			for (int i = 0; i < intAsByteArray.Length; i++)
			{
				if (intAsByteArray[i] == 0x00 && !numberFound && intAsByteArray.Length - i > minBytes)
					continue;
				intAsHexString += HexUtils.ByteArrayToHexString(new byte[] { (byte)(intAsByteArray[i] & 0xFF) });
				numberFound = true;
			}
			return intAsHexString;
		}

		/// <summary>
		/// Converts the given hexadecimal string to a pretty format by splitting the content byte by byte.
		/// </summary>
		/// <param name="hexString">The hexadecimal string to convert.</param>
		/// <returns>The hexadecimal string with pretty format.</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="hexString"/> is null.</exception>
		/// <seealso cref="PrettyHexString(byte[])"/>
		public static string PrettyHexString(string hexString)
		{
			Contract.Requires<ArgumentNullException>(hexString != null, "Hexadecimal string cannot be null.");

			String prettyHexString = "";
			if (hexString.Length % 2 != 0)
				hexString = "0" + hexString;
			int iterations = hexString.Length / 2;
			for (int i = 0; i < iterations; i++)
				prettyHexString += hexString.Substring(2 * i, 2) + " ";
			return prettyHexString.Trim();
		}

		/// <summary>
		/// Converts the given byte array into an hex string and retrieves it in pretty format by splitting the content byte by byte.
		/// </summary>
		/// <param name="value">The byte array to convert to pretty hex string.</param>
		/// <returns>The hexadecimal pretty string.</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="value"/> is null.</exception>
		/// <seealso cref="PrettyHexString(string)"/>
		public static string PrettyHexString(byte[] value)
		{
			return PrettyHexString(ByteArrayToHexString(value));
		}
	}
}
