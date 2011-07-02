using System;
using OpenTK.Graphics.OpenGL;

namespace Gametest
{
	public class Mesh
	{
		float[] _vertexBuffer;
		uint[] _indexBuffer;
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
		}
	}
}

