using System;

namespace tkglengine
{
	public class Source
	{
		public float[] data;
		public uint stride;
		public Source (float[] pData)			
		{
			data = pData;
		}
		public float this[int i]
		{
			get {return data[i];}
			set {data[i] = value;}
		}
	}
}
