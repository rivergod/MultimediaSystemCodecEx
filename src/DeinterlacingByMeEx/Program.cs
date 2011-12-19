using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace DeinterlacingByMeEx
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 2 || !(File.Exists(Path.GetFullPath(args[0]))))
			{
				Help();
				Environment.Exit(0);
			}

			List<string> tmpImgFiles = new List<string>(File.ReadAllLines(Path.GetFullPath(args[0])));
			List<string> imgFiles = new List<string>();

			tmpImgFiles.ForEach(txt => imgFiles.Add(Path.GetFullPath(txt)));

			//For List Debug
			//imgFiles.ForEach(txt => System.Diagnostics.Trace.WriteLine(txt));

			int currPos = 0;

			// 버퍼의 처음 이미지를 초기화 하고 블랙으로 채운다.
			Bitmap currBitmap = new Bitmap(720, 480);
			for (int j = 0; j < currBitmap.Height; j++)
			{
				for (int i = 0; i < currBitmap.Width; i++)
				{
					currBitmap.SetPixel(i, j, Color.Black);
				}
			}

			imgFiles.ForEach(path =>
			{
				Program.DoStep(ref currBitmap, ref currPos, path);
				currBitmap.Save(Path.GetFullPath(args[1] + String.Format("{0:D6}", currPos)) + ".bmp");
				System.Console.Write(".");
			});
			System.Console.Write("\n");
		}

		public static void DoStep(ref Bitmap prevBitmap, ref int currPos, string inputPath)
		{
			currPos++;

			Boolean isodd = (currPos % 2 == 0);

			Bitmap inputBitmap = new Bitmap(inputPath);

			for (int j = (isodd ? 0 : 1); j < prevBitmap.Height; j += 2)
			{
				for (int i = 0; i < prevBitmap.Width; i++)
				{
					prevBitmap.SetPixel(i, j, inputBitmap.GetPixel(i, (int)(j / 2)));
				}
			}

			//Low pass filter
			for (int j = 1; j < prevBitmap.Height - 1; j += 2)
			{
				for (int i = 1; i < prevBitmap.Width - 1; i++)
				{
					int r = 0, g = 0, b = 0;

					Color topLeft, topCenter, topRight, centerLeft, centerRight, bottomLeft, bottomCenter, bottomRight;

					topLeft = prevBitmap.GetPixel(i - 1, j - 1);
					topCenter = prevBitmap.GetPixel(i, j - 1);
					topRight = prevBitmap.GetPixel(i + 1, j - 1);
					centerLeft = prevBitmap.GetPixel(i - 1, j);
					centerRight = prevBitmap.GetPixel(i + 1, j);
					bottomLeft = prevBitmap.GetPixel(i - 1, j + 1);
					bottomCenter = prevBitmap.GetPixel(i, j + 1);
					bottomRight = prevBitmap.GetPixel(i + 1, j + 1);

					r = topLeft.R + topCenter.R + topRight.R + centerLeft.R + centerRight.R + bottomLeft.R + bottomCenter.R + bottomRight.R;
					g = topLeft.G + topCenter.G + topRight.G + centerLeft.G + centerRight.G + bottomLeft.G + bottomCenter.G + bottomRight.G;
					b = topLeft.B + topCenter.B + topRight.B + centerLeft.B + centerRight.B + bottomLeft.B + bottomCenter.B + bottomRight.B;

					prevBitmap.SetPixel(i, j, Color.FromArgb(r/8, g/8, b/8));
				}
			}
		}

		public static void Help()
		{
			Console.WriteLine("DeinterlacingByMeEx");
			Console.WriteLine(" Usage: Cammand ImageListFileName outputFilePreName");
		}
	}
}
