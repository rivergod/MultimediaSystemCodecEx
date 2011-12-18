using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace DeinterlacingELAEx
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

			if (isodd)
			{
				for (int j = 0; j < prevBitmap.Height; j++)
				{
					for (int i = 0; i < prevBitmap.Width; i++)
					{
						if (j % 2 == 0 || j == prevBitmap.Height - 1)
						{
							// 가장 마지막줄 이거나 홀수라인일때
							prevBitmap.SetPixel(i, j, inputBitmap.GetPixel(i, (int)(j / 2)));
						}
						else
						{
							//짝수라인일때
							if (i == 0 || i == prevBitmap.Width - 1)
							{
								//border line
								Color top = inputBitmap.GetPixel(i, (int)j / 2);
								Color bottom = inputBitmap.GetPixel(i, (int)(j / 2 + 1));
								prevBitmap.SetPixel(i, j, Color.FromArgb((int)((top.R + bottom.R) / 2), (int)((top.G + bottom.G) / 2), (int)((top.B + bottom.B) / 2)));
							}
							else
							{
								//in space
								Color topLeft = inputBitmap.GetPixel(i - 1, (int)j / 2);
								Color topCenter = inputBitmap.GetPixel(i, (int)j / 2);
								Color topRight = inputBitmap.GetPixel(i + 1, (int)j / 2);
								Color bottomLeft = inputBitmap.GetPixel(i - 1, (int)(j / 2 + 1));
								Color bottomCenter = inputBitmap.GetPixel(i, (int)(j / 2 + 1));
								Color bottomRight = inputBitmap.GetPixel(i + 1, (int)(j / 2 + 1));

								int absTopLeftBottomRight = Math.Abs(topLeft.R - bottomRight.R) + Math.Abs(topLeft.G - bottomRight.G) + Math.Abs(topLeft.B - bottomRight.B);
								int absTopCenterBottomCenter = Math.Abs(topCenter.R - bottomCenter.R) + Math.Abs(topCenter.G - bottomCenter.G) + Math.Abs(topCenter.B - bottomCenter.B);
								int absTopRightBottomLeft = Math.Abs(topRight.R - bottomLeft.R) + Math.Abs(topRight.G - bottomLeft.G) + Math.Abs(topRight.B - bottomLeft.B);

								if (absTopLeftBottomRight >= absTopCenterBottomCenter && absTopLeftBottomRight >= absTopRightBottomLeft)
								{
									prevBitmap.SetPixel(i, j, Color.FromArgb((int)((topLeft.R + bottomRight.R) / 2), (int)((topLeft.G + bottomRight.G) / 2), (int)((topLeft.B + bottomRight.B) / 2)));
								}
								else if (absTopCenterBottomCenter >= absTopLeftBottomRight && absTopCenterBottomCenter >= absTopRightBottomLeft)
								{
									prevBitmap.SetPixel(i, j, Color.FromArgb((int)((topCenter.R + bottomCenter.R) / 2), (int)((topCenter.G + bottomCenter.G) / 2), (int)((topCenter.B + bottomCenter.B) / 2)));
								}
								else
								{
									prevBitmap.SetPixel(i, j, Color.FromArgb((int)((topRight.R + bottomLeft.R) / 2), (int)((topRight.G + bottomLeft.G) / 2), (int)((topRight.B + bottomLeft.B) / 2)));
								}
							}
						}

					}
				}
			}
			else
			{
				for (int j = 0; j < prevBitmap.Height; j++)
				{
					for (int i = 0; i < prevBitmap.Width; i++)
					{
						if (j % 2 == 1 || j == 0)
						{
							// 가장 첫줄 이거나 짝수라인일때
							prevBitmap.SetPixel(i, j, inputBitmap.GetPixel(i, (int)(j / 2)));
						}
						else
						{
							//홀수라인일때
							if (i == 0 || i == prevBitmap.Width - 1)
							{
								//border line
								Color top = inputBitmap.GetPixel(i, (int)j / 2);
								Color bottom = inputBitmap.GetPixel(i, (int)(j / 2 + 1));
								prevBitmap.SetPixel(i, j, Color.FromArgb((int)((top.R + bottom.R) / 2), (int)((top.G + bottom.G) / 2), (int)((top.B + bottom.B) / 2)));
							}
							else
							{
								//in space
								Color topLeft = inputBitmap.GetPixel(i - 1, (int)j / 2);
								Color topCenter = inputBitmap.GetPixel(i, (int)j / 2);
								Color topRight = inputBitmap.GetPixel(i + 1, (int)j / 2);
								Color bottomLeft = inputBitmap.GetPixel(i - 1, (int)(j / 2 + 1));
								Color bottomCenter = inputBitmap.GetPixel(i, (int)(j / 2 + 1));
								Color bottomRight = inputBitmap.GetPixel(i + 1, (int)(j / 2 + 1));

								int absTopLeftBottomRight = Math.Abs(topLeft.R - bottomRight.R) + Math.Abs(topLeft.G - bottomRight.G) + Math.Abs(topLeft.B - bottomRight.B);
								int absTopCenterBottomCenter = Math.Abs(topCenter.R - bottomCenter.R) + Math.Abs(topCenter.G - bottomCenter.G) + Math.Abs(topCenter.B - bottomCenter.B);
								int absTopRightBottomLeft = Math.Abs(topRight.R - bottomLeft.R) + Math.Abs(topRight.G - bottomLeft.G) + Math.Abs(topRight.B - bottomLeft.B);

								if (absTopLeftBottomRight >= absTopCenterBottomCenter && absTopLeftBottomRight >= absTopRightBottomLeft)
								{
									prevBitmap.SetPixel(i, j, Color.FromArgb((int)((topLeft.R + bottomRight.R) / 2), (int)((topLeft.G + bottomRight.G) / 2), (int)((topLeft.B + bottomRight.B) / 2)));
								}
								else if (absTopCenterBottomCenter >= absTopLeftBottomRight && absTopCenterBottomCenter >= absTopRightBottomLeft)
								{
									prevBitmap.SetPixel(i, j, Color.FromArgb((int)((topCenter.R + bottomCenter.R) / 2), (int)((topCenter.G + bottomCenter.G) / 2), (int)((topCenter.B + bottomCenter.B) / 2)));
								}
								else
								{
									prevBitmap.SetPixel(i, j, Color.FromArgb((int)((topRight.R + bottomLeft.R) / 2), (int)((topRight.G + bottomLeft.G) / 2), (int)((topRight.B + bottomLeft.B) / 2)));
								}
							}
						}

					}
				}
			}
		}

		public static void Help()
		{
			Console.WriteLine("DeinterlacingELAEx");
			Console.WriteLine(" Usage: Cammand ImageListFileName outputFilePreName");
		}

	}
}
