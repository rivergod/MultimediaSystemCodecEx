using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace CreateDiffImg
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 3 || !(File.Exists(Path.GetFullPath(args[0]))) || !(File.Exists(Path.GetFullPath(args[1]))))
			{
				Help();
				Environment.Exit(0);
			}

			Bitmap dstBitmap = new Bitmap(Path.GetFullPath(args[0]));
			Bitmap srcBitmap = new Bitmap(Path.GetFullPath(args[1]));
			Bitmap outBitmap = new Bitmap(dstBitmap.Width, dstBitmap.Height);

			for (int j = 0; j < dstBitmap.Height; j++)
			{
				for (int i = 0; i < dstBitmap.Width; i++)
				{
					Color dstColor = dstBitmap.GetPixel(i, j);
					Color srcColor = srcBitmap.GetPixel(i, j);
					outBitmap.SetPixel(i, j, Color.FromArgb(Math.Abs(dstColor.R - srcColor.R), Math.Abs(dstColor.G - srcColor.G), Math.Abs(dstColor.B - srcColor.B)));
				}
			}

			outBitmap.Save(Path.GetFullPath(args[2]));
		}

		public static void Help()
		{
			Console.WriteLine("CreateDiffImg");
			Console.WriteLine(" Usage: Cammand dstFileName srcFileName outFileName");
		}
	}
}
