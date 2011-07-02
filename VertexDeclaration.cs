
using System;
using System.Runtime.InteropServices;
using OpenTK;
using System.Collections.Generic;

namespace Gametest
{	
	public interface IVertex
	{
		float[] ToArray();
		void FromArray(float[] array);
		void FromArray(float[] array, int offset);
		VertexDeclaration VertexDeclaration
		{
			get;
		}
	}	
	
	public enum VertexDeclaration
	{
		Position,
		PositionNormal,
		PositionNormalTexture
	}
	
	public struct VertexPositionNormal : IVertex
	{		
		public Vector3 Position;
		public Vector3 Normal;
		
		const VertexDeclaration _vertexDeclaration = VertexDeclaration.PositionNormal;
		
		#region IVertex implementation
		float[] IVertex.ToArray ()
		{
			float[] array = new float[8];
			array[0] = Position.X;
			array[1] = Position.Y;
			array[2] = Position.Z;
			array[3] = Normal.X;
			array[4] = Normal.Y;
			array[5] = Normal.Z;
			return array;
		}
		
		void IVertex.FromArray(float[] array)
		{
			if (array.Length < 6)
				throw new ArgumentException("Array does not have enough elements!");
			Position.X = array[0];
			Position.Y = array[1];
			Position.Z = array[2];
			Normal.X = array[3];
			Normal.Y = array[4];
			Normal.Z = array[5];
		}

		VertexDeclaration IVertex.VertexDeclaration {
			get {
				return _vertexDeclaration;
			}			
		}
		




		void IVertex.FromArray (float[] array, int offset)
		{
			if (array.Length < 6 + offset)
				throw new ArgumentException("Array does not have enough elements!");
			Position.X = array[0 + offset];
			Position.Y = array[1 + offset];
			Position.Z = array[2 + offset];
			Normal.X = array[3 + offset];
			Normal.Y = array[4 + offset];
			Normal.Z = array[5 + offset];
		}

	#endregion
	}
	
	public struct VertexPositionNormalTexture : IVertex
	{		
		public Vector3 Position;
		public Vector3 Normal;		
		public Vector2 TexCoord;
		const VertexDeclaration _vertexDeclaration = VertexDeclaration.PositionNormalTexture;

		#region IVertex implementation
		float[] IVertex.ToArray ()
		{
			float[] array = new float[8];
			array[0] = Position.X;
			array[1] = Position.Y;
			array[2] = Position.Z;
			array[3] = Normal.X;
			array[4] = Normal.Y;
			array[5] = Normal.Z;
			array[6] = TexCoord.X;
			array[7] = TexCoord.Y;
			return array;
		}
		
		void IVertex.FromArray(float[] array)
		{
			if (array.Length < 8)
				throw new ArgumentException("Array does not have enough elements!");
			Position.X = array[0];
			Position.Y = array[1];
			Position.Z = array[2];
			Normal.X = array[3];
			Normal.Y = array[4];
			Normal.Z = array[5];
			TexCoord.X = array[6];
			TexCoord.Y = array[7];
		}
		
		void IVertex.FromArray(float[] array, int offset)
		{
			if (array.Length < 8 + offset)
				throw new ArgumentException("Array does not have enough elements!");
			Position.X = array[0 + offset];
			Position.Y = array[1 + offset];
			Position.Z = array[2 + offset];
			Normal.X = array[3 + offset];
			Normal.Y = array[4 + offset];
			Normal.Z = array[5 + offset];
			TexCoord.X = array[6 + offset];
			TexCoord.Y = array[7 + offset];
		}

		VertexDeclaration IVertex.VertexDeclaration {
			get {
				return _vertexDeclaration;
			}			
		}
		#endregion
	}
	
	public struct VertexPosition : IVertex
	{
		public Vector3 Position;
		const VertexDeclaration _vertexDeclaration = VertexDeclaration.Position;
	

		#region IVertex implementation
		void IVertex.FromArray (float[] array)
		{
			if (array.Length < 3)
				throw new ArgumentException("array does not have enough elements!");
			Position.X = array[0];
			Position.Y = array[1];
			Position.Z = array[2];
		}

		void IVertex.FromArray (float[] array, int offset)
		{
			throw new NotImplementedException ();
		}
		
		VertexDeclaration IVertex.VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
		
		float[] IVertex.ToArray()
		{
			float[] array = new float[3];
			array[0] = Position.X;
			array[1] = Position.Y;
			array[2] = Position.Z;
			return array;
		}

		#endregion
	}
}


