using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LibFisherEx
{
	[Serializable()]
	public class FDT : ISerializable
	{
		private List<XYSO> dataList;

		public List<XYSO> DataList
		{
			get
			{
				return this.dataList;
			}
			set
			{
				this.dataList = value;
			}
		}

		public FDT()
		{
			this.dataList = new List<XYSO>();
		}

		public FDT(SerializationInfo info, StreamingContext ctxt)
		{
			this.dataList = (List<XYSO>)info.GetValue("DataList", typeof(List<XYSO>));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("DataList", this.dataList);
		}

		public static void SerializeObject(string filename, FDT objectToSerialize)
		{
			Stream stream = File.Open(filename, FileMode.Create);
			BinaryFormatter bFormatter = new BinaryFormatter();
			bFormatter.Serialize(stream, objectToSerialize);
			stream.Close();
		}

		public static FDT DeSerializeObject(string filename)
		{
			FDT objectToSerialize;
			Stream stream = File.Open(filename, FileMode.Open);
			BinaryFormatter bFormatter = new BinaryFormatter();
			objectToSerialize = (FDT)bFormatter.Deserialize(stream);
			stream.Close();
			return objectToSerialize;
		}
	}

	[Serializable()]
	public struct XYSO
	{
		public int lowx;
		public int lowy;
		public int highx;
		public int highy;
		public double s;
		public double o;
		public double e;
	}

}
