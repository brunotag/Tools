using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SimpleNumberEncoder
{
	class Program
	{
		static readonly Dictionary<char, byte> _mapping = new Dictionary<char, byte>
        {
            {'0', 0},
            {'1', 1},
            {'2', 2},
            {'3', 3},
            {'4', 4},
            {'5', 5},
            {'6', 6},
            {'7', 7},
            {'8', 8},
            {'9', 9},
            {'A', 10},
            {'B', 11},
            {'C', 12},
            {'D', 13},
            {'E', 14},
            {'F', 15},
        };

		static void Main(string[] args)
		{
			try
			{
				Run(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				ShowUsage();
			}
		}

		static void ShowUsage()
		{
			Console.WriteLine("Usage: ");
			Console.WriteLine();
			Console.WriteLine("Encode a text file with digits into a 'byte' file:");
			Console.WriteLine("SimpleNumberEncoder.exe -e input.txt output.dat");
			Console.WriteLine();
			Console.WriteLine("Decode a 'byte' file into a text file with digits:");
			Console.WriteLine("SimpleNumberEncoder.exe -d input.dat output.txt");
		}

		static void Run(string[] args)
		{
			string inputFileName = args[0];
			string outputFileName = args[1];
			string mode = args[2];

			if (mode == "-e")
			{
				using (var fileIn = File.OpenText(inputFileName))
				{
					using (var fileOut = File.OpenWrite(outputFileName))
					{
						EncodeAsciiDigitsInBytes(fileIn, fileOut);
					}
				}
			}
			else if (mode == "-d")
			{
				using (var fileIn = File.OpenRead(inputFileName))
				{
					using (var fileOut = File.CreateText(outputFileName))
					{
						DecodeByteDigitsToAscii(fileIn, fileOut);
					}
				}
			}
		}

		private static void DecodeByteDigitsToAscii(Stream fileIn, TextWriter fileOut)
		{
			while (fileIn.Position != fileIn.Length)
			{
				int firstNum, secondNum;

				var input = (byte)fileIn.ReadByte();

				ExtractNumbersFromEncodedDigits(input, out firstNum, out secondNum);

				fileOut.Write(
					firstNum.ToString(CultureInfo.InvariantCulture) +
					secondNum.ToString(CultureInfo.InvariantCulture)
					);
			}
		}

		private static void ExtractNumbersFromEncodedDigits(byte encodedInput, out int firstNum, out int secondNum)
		{
			secondNum = encodedInput & 0x0F;
			firstNum = (encodedInput >> 4) & 0x0F;
		}

		private static void EncodeAsciiDigitsInBytes(StreamReader fileIn, Stream fileOut)
		{
			while (!fileIn.EndOfStream)
			{
				var msn = (char)fileIn.Read();
				var lsn = (char)fileIn.Read();

				var result = CompactNibblesIntoAByte(lsn, msn);
				fileOut.WriteByte(result);
			}
		}

		private static byte CompactNibblesIntoAByte(char leastSignificantNibble, char mostSignificantNibble)
		{
			return (byte)(_mapping[mostSignificantNibble] | (_mapping[leastSignificantNibble] << 4));
		}
	}
}
