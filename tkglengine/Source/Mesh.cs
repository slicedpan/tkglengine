using System;
using OpenTK.Graphics.OpenGL;

namespace tkglengine
{
	public class Mesh
	{
		float[] _vertexBuffer;
		uint[] _indexBuffer;
		int _vertexCount;
		
		public int VertexCount
		{
			get {return _vertexCount;}
		}

		public int Stride
		{
			get
			{
				switch (_vertexDeclaration)
				{
				case VertexDeclaration.Position:
					return 3;
				case VertexDeclaration.PositionNormal:
					return 6;
				case VertexDeclaration.PositionNormalTexture:
					return 8;
				default:
					throw new Exception("Vertex Declaration does not provide a stride");
				}
			}
		}
		public float[] VertexBuffer
		{
			get {return _vertexBuffer;}
		}
		public uint[] IndexBuffer
		{
			get {return _indexBuffer;}
		}
		VertexDeclaration _vertexDeclaration;
		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
		public Mesh (float[] vertexData, VertexDeclaration vertexDeclaration)
		{
			_vertexBuffer = vertexData;
			_vertexDeclaration = vertexDeclaration;
			_vertexCount = vertexData.Length / Stride;
		}
	}
}

