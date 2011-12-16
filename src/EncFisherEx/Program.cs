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

namespace EncFisherEx
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 2 || !(File.Exists(Path.GetFullPath(args[0]))))
			{
				//System.Diagnostics.Trace.WriteLine("Args[0] target file => " + Path.GetFullPath(args[0]));
				Help();
				Environment.Exit(0);
			}

			Bitmap srcBitmap = null;

			try
			{
				srcBitmap = new Bitmap(Path.GetFullPath(args[0]));
			}
			catch (FileNotFoundException e)
			{
				Help();
				Environment.Exit(0);
			}

			//for (int x = 0; x < srcBitmap.Width; x++)
			//{
			//    for (int y = 0; y < srcBitmap.Height; y++)
			//    {
			//    }
			//}

			// Pre Step.
			Console.WriteLine("Running Pre Step.");
			DenseMatrix imgMat = new DenseMatrix(srcBitmap.Height, srcBitmap.Width);
			for (int j = 0; j < srcBitmap.Height; j++)
			{
				for (int i = 0; i < srcBitmap.Width; i++)
				{
					imgMat.At(j, i, srcBitmap.GetPixel(i, j).R);
				}
			}

			// Step 1.
			Console.WriteLine("Running Step 1.");

			int lowBlkWidthCnt = srcBitmap.Width / 8;
			int lowBlkHeightCnt = srcBitmap.Height / 8;

			List<DenseMatrix> lowBlkList = new List<DenseMatrix>(lowBlkWidthCnt * lowBlkHeightCnt);
			List<double> lowIntwList = new List<double>(lowBlkWidthCnt * lowBlkHeightCnt);

			for (int j = 0; j < lowBlkHeightCnt; j++)
			{
				for (int i = 0; i < lowBlkWidthCnt; i++)
				{
					DenseMatrix mat = null;
					double intw = 0;

					try
					{
						MathNet.Numerics.LinearAlgebra.Generic.Matrix<double> tmpMatrix = imgMat.SubMatrix(j * 8, 8, i * 8, 8);

						mat = new DenseMatrix(tmpMatrix.RowCount, tmpMatrix.ColumnCount);
						mat.SetSubMatrix(0, 8, 0, 8, tmpMatrix);

						IEnumerable<Tuple<int, int, double>> e = mat.IndexedEnumerator();

						intw = e.Sum(element => element.Item3);
					}
					catch (ArgumentOutOfRangeException e)
					{
					}
					catch (ArgumentException e)
					{
					}

					lowBlkList.Add(mat);
					lowIntwList.Add(intw);
					Console.Write(".");
				}
			}
			Console.WriteLine();

			// Step 2.
			Console.WriteLine("Running Step 2.");

			int highBlkWidthCnt = (srcBitmap.Width - 16) / 4;
			int highBlkHeightCnt = (srcBitmap.Height - 16) / 4;

			List<DenseMatrix> highBlkList = new List<DenseMatrix>(highBlkWidthCnt * highBlkHeightCnt);
			List<double> highggAList = new List<double>(highBlkWidthCnt * highBlkHeightCnt);
			List<double> highgAList = new List<double>(highBlkWidthCnt * highBlkHeightCnt);
			List<DenseMatrix> highInvMatList = new List<DenseMatrix>(highBlkWidthCnt * highBlkHeightCnt);

			for (int j = 0; j < highBlkHeightCnt; j++)
			{
				for (int i = 0; i < highBlkWidthCnt; i++)
				{
					DenseMatrix mat = new DenseMatrix(8, 8);

					for (int l = 0; l < 8; l++)
					{
						for (int k = 0; k < 8; k++)
						{
							mat.At(l, k, 0.25 * (double)(srcBitmap.GetPixel((i * 4) + (k * 2), (j * 4) + (l * 2)).R + srcBitmap.GetPixel((i * 4) + (k * 2), (j * 4) + (l * 2 + 1)).R + srcBitmap.GetPixel((i * 4) + (k * 2 + 1), (j * 4) + (l * 2)).R + srcBitmap.GetPixel((i * 4) + (k * 2), (j * 4) + (l * 2 + 1)).R));
						}
					}

					double intggA = mat.IndexedEnumerator().Sum(element => element.Item3 * element.Item3);
					double intgA = mat.IndexedEnumerator().Sum(element => element.Item3);

					highBlkList.Add(mat);
					highggAList.Add(intggA);
					highgAList.Add(intgA);

					DenseMatrix multMat = new DenseMatrix(2, 2);
					multMat.At(0, 0, intggA);
					multMat.At(0, 1, intgA);
					multMat.At(1, 0, intgA);
					multMat.At(1, 1, 64);

					DenseMatrix invMat = new DenseMatrix(2, 2);
					invMat.SetSubMatrix(0, 2, 0, 2, multMat.Inverse());

					highInvMatList.Add(invMat);

					Console.Write(".");
				}
			}
			Console.WriteLine();

			// Step 3.
			Console.WriteLine("Running Step 3.");

			FDT result = new FDT();

			for (int j = 0; j < lowBlkHeightCnt; j++)
			{
				for (int i = 0; i < lowBlkWidthCnt; i++)
				{
					XYSO minxyso;
					minxyso.lowx = i;
					minxyso.lowy = j;
					minxyso.highx = 0;
					minxyso.highy = 0;
					minxyso.s = 0;
					minxyso.o = 0;
					minxyso.e = double.MaxValue;

					for (int l = 0; l < highBlkHeightCnt; l++)
					{
						for (int k = 0; k < highBlkWidthCnt; k++)
						{
							DenseMatrix targetHighMat = highBlkList[l * highBlkWidthCnt + k];
							DenseMatrix targetLowMat = lowBlkList[j * lowBlkWidthCnt + i];

							double[] targetHighMatData = targetHighMat.ToRowWiseArray();
							double[] targetLowMatData = targetLowMat.ToRowWiseArray();

							//IEnumerable<Tuple<int, int, double>> targetHighMatEnum = targetHighMat.IndexedEnumerator();
							//IEnumerable<Tuple<int, int, double>> targetLowMatEnum = targetLowMat.IndexedEnumerator();

							double targetHnLElementSum = 0;

							for ( int m = 0; m < 64; m++ )
							{
								targetHnLElementSum += targetHighMatData[m] * targetLowMatData[m];
							}

							MathNet.Numerics.LinearAlgebra.Generic.Matrix<double> so = highInvMatList[l * highBlkWidthCnt + k].Multiply(new DenseMatrix(2, 1, new double[] { targetHnLElementSum, lowIntwList[j * lowBlkWidthCnt + i] }));
							double e = (lowBlkList[j * lowBlkWidthCnt + i] - (so[0, 0] * highBlkList[l * highBlkWidthCnt + k]) - new DenseMatrix(8, 8, so[1, 0])).IndexedEnumerator().Sum(element => element.Item3 * element.Item3);

							if (e < minxyso.e)
							{
								minxyso.highx = k;
								minxyso.highy = l;
								minxyso.s = so[0, 0];
								minxyso.o = so[1, 0];
								minxyso.e = e;
							}
						}
					}

					//Console.WriteLine("Low Block (x,y)=(" + minxyso.lowx + "," + minxyso.lowy + "), High Block (x,y)=(" + minxyso.highx + "," + minxyso.highy + "), so= (" + minxyso.s + "," + minxyso.o + "), e=" + minxyso.e);

					result.DataList.Add(minxyso);
					Console.Write(".");
				}
			}


			Console.WriteLine();

			Console.WriteLine("Running Post Step.");
			FDT.SerializeObject(args[1] + ".fdt", result);

			Console.WriteLine("Done.");
		}


		public static void Help()
		{
			Console.WriteLine("EncFisherEx");
			Console.WriteLine(" Usage: Cammand ImageFileName OutDataFileName");
		}
	}

}
