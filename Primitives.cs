using System;
namespace Gametest
{
	public struct Triangle : IPoly
	{
		public IVertex p1, p2, p0;
	

		#region IPoly implementation
		public IVertex this[uint index] {
			get {
				switch (index)
				{
				case 0:
					return p0;
				case 1: 
					return p1;
				case 2:
					return p2;
				default:
					throw new ArgumentException("index must be 0 - 2");
				}
			}
			set {
				switch (index)
				{
				case 0:
					p0 = value;
					break;
				case 1:
					p1 = value;
					break;
				case 2:
					p2 = value;
					break;				
				}
			}
		}
		#endregion
}	
	public struct Quad : IPoly
	{
		public IVertex p1, p2, p3, p0;
		#region IPoly implementation
		public IVertex this[uint index] {
			get {
				switch (index)
				{
				case 0:
					return p0;
				case 1: 
					return p1;
				case 2:
					return p2;
				case 3:
					return p3;
				default:
					throw new ArgumentException("index must be 0 - 3");
				}
			}
			set {
				switch (index)
				{
				case 0:
					p0 = value;
					break;
				case 1:
					p1 = value;
					break;
				case 2:
					p2 = value;
					break;
				case 3:
					p3 = value;
					break;
				}
			}
		}
		#endregion
		
	}	
	public interface IPoly
	{
		IVertex this[uint index]
		{
			get; set;
		}
	}
	public class PrimitiveHelper
	{
		public static Triangle[] ToTriangles(IPoly quad)
		{
			Triangle[] tris = new Triangle[2];
			
			string quadType = quad[0].GetType().ToString();
			var type = Type.GetType(quadType);
			for (uint i = 0; i < 3; ++i)
			{
				tris[0][i] = (IVertex)Activator.CreateInstance(type);
				tris[1][i] = (IVertex)Activator.CreateInstance(type);
			}			
			
			tris[0].p0.FromArray(quad[0].ToArray());
			tris[0].p1.FromArray(quad[1].ToArray());
			tris[0].p2.FromArray(quad[2].ToArray());
			
			tris[1].p0.FromArray(quad[0].ToArray());
			tris[1].p1.FromArray(quad[2].ToArray());
			tris[1].p2.FromArray(quad[3].ToArray());
			
			return tris;
		}
	}
}
