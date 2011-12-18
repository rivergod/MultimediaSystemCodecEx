using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InterFieldInterpolationEx
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1 || !(File.Exists(Path.GetFullPath(args[0]))))
			{
				Help();
				Environment.Exit(0);
			}

			List<string> imgFiles = new List<string>();


		}

		public static void Help()
		{
			Console.WriteLine("InterFieldInterpolationEx");
			Console.WriteLine(" Usage: Cammand ImageListFileName");
		}
	}
}
