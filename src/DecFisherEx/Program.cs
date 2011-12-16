using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

using LibFisherEx;

namespace DecFisherEx
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

			Console.WriteLine("Running Pre Step.");

			Bitmap srcBitmap = null;
			DenseMatrix srcMat = new DenseMatrix(256, 256);

			try
			{
				srcBitmap = new Bitmap(Path.GetFullPath(args[0]));
			}
			catch (FileNotFoundException e)
			{
				Help();
				Environment.Exit(0);	
			}

			Console.WriteLine("Running Step1.");

			for (int j = 0; j < 256; j++)
			{
				for (int i = 0; i < 256; i++)
				{
					srcMat.At(j, i, srcBitmap.GetPixel(i, j).R);		
				}
			}

			Console.WriteLine(" Load target image.");

			FDT data = FDT.DeSerializeObject(Path.GetFullPath(args[1]));
			Console.WriteLine(" Load data file.");

			DenseMatrix output = new DenseMatrix(256, 256);

			Console.WriteLine("Running Step2.");

			foreach (XYSO xyso in data.DataList)
			{
				MathNet.Numerics.LinearAlgebra.Generic.Matrix<double> highMat = srcMat.SubMatrix(xyso.highy * 4, 16, xyso.highx * 4, 16);
				DenseMatrix high8Mat = new DenseMatrix(8, 8);

				for (int j = 0; j < 8; j++)
				{
					for (int i = 0; i < 8; i++)
					{
						double z = xyso.s * 0.25 * (highMat[2 * j, 2 * i] + highMat[2 * j, 2 * i + 1] + highMat[2 * j + 1, 2 * i] + highMat[2 * j, 2 * i + 1]) + xyso.o;

						high8Mat.At(j, i, z);
					}
				}

				output.SetSubMatrix(xyso.lowy * 8, 8, xyso.lowx * 8, 8, high8Mat);
				
			}

			Bitmap outBitmap = new Bitmap(256, 256);

			for (int j = 0; j < 256; j++)
			{
				for (int i = 0; i < 256; i++)
				{
					if (output[j, i] > 255)
					{
						outBitmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));
					}
					else if (output[j, i] < 0)
					{
						outBitmap.SetPixel(i, j, Color.FromArgb(0, 0, 0));
					}
					else
					{
						outBitmap.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(output[j, i]), Convert.ToInt32(output[j, i]), Convert.ToInt32(output[j, i])));
					}
				}
			}

			outBitmap.Save(Path.GetFullPath(args[2]));
			
			Console.WriteLine("Done.");
		}

		public static void Help()
		{
			Console.WriteLine("DecFisherEx");
			Console.WriteLine(" Usage: Cammand ImageFileName DataFileName OutImageFileName");
		}
	}
}
