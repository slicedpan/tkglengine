using System;
using System.Xml;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Xml.XPath;

namespace tkglengine
{
	public class GeometryParserException : Exception
	{
		public GeometryParserException(string message) :
			base(message)
		{}	
	}
	public class GeometryParser
	{
		#region fields
		float[] vertexData;

		uint[] indices;
		Dictionary <string, Source> sources;
		Dictionary <string, Semantic> semantics;
		string nsName;
		VertexDeclaration _vertexDeclaration;
		uint[] polyVertexCount;
		public String Name;
		XmlNamespaceManager nsManager;
		int triCount = 0;
		Triangle[] tris;
		uint stride;
		#endregion
		
		public VertexDeclaration VertexDeclaration
		{
			get 
			{
				return _vertexDeclaration;
			}
		}
		
		
		public GeometryParser (XmlNamespaceManager pNSManager)			
		{			
			nsManager = pNSManager;
			sources = new Dictionary<string, Source>();
			semantics = new Dictionary<string, Semantic>();			
		}
		
		private void CreateArrays()
		{
			vertexData = new float[stride * tris.Length * 3];			
			
			uint offset = 0;
			
			for (uint i = 0; i < tris.Length; ++i)
			{
				for (uint j = 0; j < 3; ++j)
				{
					tris[i][j].ToArray().CopyTo(vertexData, offset * stride);
					++offset;
				}
			}
		}
		
		public static void Populate (float[] source, uint sourceOffset, float[] dest, uint destOffset, uint num)
		{
			for (int i = 0; i < num; i++)
			{
				dest[i + destOffset] = source[i + sourceOffset];  
			}
		}
		
		private void PopulateMesh()
		{
			int offset = 0;
			
			tris = new Triangle[triCount];
			int triPos = 0;
			
			
			switch (_vertexDeclaration)
			{
				case VertexDeclaration.PositionNormal:
					stride = 6;
					for (uint i = 0; i < polyVertexCount.Length; ++i)
					{
						IPoly poly;
					
						if (polyVertexCount[i] == 3)						
							poly = new Triangle();
						else						
							poly = new Quad();
						
						for (uint j = 0; j < polyVertexCount[i]; ++j)
						{
							uint posIndex = indices[(semantics.Count * offset) + semantics["VERTEX"].Offset];
							uint normalIndex = indices[(semantics.Count * offset) + semantics["NORMAL"].Offset];
							float[] datArray = new float[6];
							float[] sourceData = sources[semantics["VERTEX"].SourceName].data;
							uint sourceStride = sources[semantics["VERTEX"].SourceName].stride;
						
							Populate(sourceData, posIndex * sourceStride, datArray, 0, 3);
							
							sourceData = sources[semantics["NORMAL"].SourceName].data;
							sourceStride = sources[semantics["NORMAL"].SourceName].stride;
						
							Populate(sourceData, normalIndex * sourceStride, datArray, 3, 3);
						
							poly[j] = new VertexPositionNormal();		
							poly[j].FromArray(datArray);
							++offset;							
						}
						if (polyVertexCount[i] == 4)
						{
							Triangle[] quadTris = PrimitiveHelper.ToTriangles(poly);
							quadTris.CopyTo(tris, triPos);						
							triPos += 2;
						}
						else
						{
							tris[triPos] = (Triangle)poly;
							++triPos;
						}
					}
					break;
				
				case VertexDeclaration.PositionNormalTexture:
					stride = 8;
					for (uint i = 0; i < polyVertexCount.Length; ++i)
					{
						IPoly poly;
					
						if (polyVertexCount[i] == 3)						
							poly = new Triangle();
						else						
							poly = new Quad();
						
						for (uint j = 0; j < polyVertexCount[i]; ++j)
						{
							uint posIndex = indices[(semantics.Count * offset) + semantics["VERTEX"].Offset];
							uint normalIndex = indices[(semantics.Count * offset) + semantics["NORMAL"].Offset];
							uint texIndex = indices[(semantics.Count * offset) + semantics["TEXCOORD"].Offset];
							float[] datArray = new float[8];
							float[] sourceData = sources[semantics["VERTEX"].SourceName].data;
							uint sourceStride = sources[semantics["VERTEX"].SourceName].stride;
						
							Populate(sourceData, posIndex * sourceStride, datArray, 0, 3);
							
							sourceData = sources[semantics["NORMAL"].SourceName].data;
							sourceStride = sources[semantics["NORMAL"].SourceName].stride;
						
							Populate(sourceData, normalIndex * sourceStride, datArray, 2, 3);
						
							sourceData = sources[semantics["TEXCOORD"].SourceName].data;
							sourceStride = sources[semantics["TEXCOORD"].SourceName].stride;
						
							Populate(sourceData, texIndex * sourceStride, datArray, 5, 2);
						
							poly[j] = new VertexPositionNormalTexture();	
							poly[j].FromArray(datArray);
							++offset;							
						}
						if (polyVertexCount[i] == 4)
						{
							Triangle[] quadTris = PrimitiveHelper.ToTriangles(poly);
							quadTris.CopyTo(tris, triPos);						
							triPos += 2;
						}
						else
						{
							tris[triPos] = (Triangle)poly;
							++triPos;
						}
					}
				
					break;
				case VertexDeclaration.Position:
					break;
			}
		}
		
		private void ParseSource(XPathNavigator nav)
		{
			string sourceName = nav.GetAttribute("id", nav.GetNamespace("c"));
			Console.WriteLine("found source: " + sourceName);
			var arrayNode = nav.SelectSingleNode("c:float_array", nsManager);
			int arrayCount;
			if (!int.TryParse(arrayNode.GetAttribute("count", arrayNode.GetNamespace("c")), out arrayCount))
				throw new GeometryParserException("could not parse source: " + sourceName + " array");
			sources.Add(sourceName, new Source(new float[arrayCount]));
			string[] parts = arrayNode.InnerXml.Split(' ');
			
			for (int i = 0; i < arrayCount; ++i)
			{
				float fVal;
				if (!float.TryParse(parts[i], out fVal))
					throw new GeometryParserException("could not parse source: " + sourceName + " array, element: " + i);	
				sources[sourceName][i] = fVal;	    
			}
			
			var accessorNode = nav.SelectSingleNode("c:technique_common/c:accessor", nsManager);			
			uint.TryParse(accessorNode.GetAttribute("stride", nsName), out sources[sourceName].stride);
		}		
		private void ParseMesh(XPathNavigator nav)
		{			
			var sourceIterator = nav.Select("c:source", nsManager);
			while (sourceIterator.MoveNext())
			{
				ParseSource(sourceIterator.Current);
			}
			
			var vertices = nav.SelectSingleNode("c:vertices", nsManager);
			string sourceNewName = vertices.GetAttribute("id", nsName);
			var input = nav.SelectSingleNode("c:vertices/c:input", nsManager);
			string sourceOldName = input.GetAttribute("source", nsName);
			sourceOldName = sourceOldName.Substring(1);
			
			float[] tmpData = sources[sourceOldName].data;
			uint tmpStride = sources[sourceOldName].stride;
			
			sources.Remove(sourceOldName);
			sources.Add(sourceNewName, new Source(tmpData));
			sources[sourceNewName].stride = tmpStride;
			
			var polylist = nav.SelectSingleNode("c:polylist", nsManager);
			int polyCount;
			if (!int.TryParse(polylist.GetAttribute("count", nsName), out polyCount))
				throw new GeometryParserException("could not parse poly count!");
			
			polyVertexCount = new uint[polyCount];
			
			var vcount = polylist.SelectSingleNode("c:vcount", nsManager);
			string[] parts = vcount.InnerXml.Split(' ');
			
			for (int i = 0; i < polyCount; ++i)
			{
				if (!uint.TryParse(parts[i], out polyVertexCount[i]))
					throw new GeometryParserException("could not parse poly vertex count at element: " + i);
				
				if (polyVertexCount[i] == 3)
					++triCount;
				else if (polyVertexCount[i] == 4)
					triCount += 2;
				else
					throw new GeometryParserException("can only parse tris and quads!");
			}
			
			var inputIterator = polylist.Select("c:input", nsManager);
			uint inputCount = 0;
			while (inputIterator.MoveNext())
			{
				string inputSemantic = inputIterator.Current.GetAttribute("semantic", nsName);
				string inputSource = inputIterator.Current.GetAttribute("source", nsName).Substring(1);
				uint inputOffset;
				if (!uint.TryParse(inputIterator.Current.GetAttribute("offset", nsName), out inputOffset))
					throw new GeometryParserException("could not parse input offset, (semantic " + inputSemantic + ")");
				semantics.Add(inputSemantic, new Semantic(inputSource, inputOffset));
				++inputCount;
			}
			
			if (!semantics.ContainsKey("VERTEX"))
				throw new GeometryParserException("Mesh has no vertex position data");
			
			if (semantics.ContainsKey("NORMAL") && semantics.ContainsKey("TEXCOORD"))
			{
				_vertexDeclaration = VertexDeclaration.PositionNormalTexture;
			}
			else if (semantics.ContainsKey("NORMAL"))
			{
				_vertexDeclaration = VertexDeclaration.PositionNormal;
			}
			else
			{
				_vertexDeclaration = VertexDeclaration.Position;
			}
			
			var indicesNode = polylist.SelectSingleNode("c:p", nsManager);
			parts = indicesNode.InnerXml.Split(' ');
			
			uint indexCount = 0;
			
			foreach (uint v in polyVertexCount)			
				indexCount += v;
			
			indexCount *= inputCount;
			indices = new uint[indexCount];
			
			for (int i = 0; i < indexCount; ++i)
			{
				uint.TryParse(parts[i], out indices[i]);
			}		
			
			PopulateMesh();
			
			Console.WriteLine("Finished Parsing Mesh");
		}
		
		public Mesh Parse(XPathNavigator nav, string geomName)
		{
			
			Name = geomName;
			nsName = nav.GetNamespace("c");
			
			var mesh = nav.SelectSingleNode("c:mesh", nsManager);
			if (mesh == null)
				throw new GeometryParserException("Geometry does not contain mesh!");
			
			ParseMesh(mesh);
			
			CreateArrays();
			
			Mesh retMesh = new Mesh(vertexData, VertexDeclaration);
			return retMesh;
			
		}
		public static uint[] PolyToTris (uint[] indices, int numberOfSides, int polyCount)
		{
			if (indices.Length != numberOfSides * polyCount)
				throw new ArgumentException("No. of indices must match the number of sides by the number of polys!");
			int trisPerPoly = (numberOfSides - 2);
			int triCount = trisPerPoly * polyCount;			
			uint[] triIndices = new uint[3 * triCount];
			int counter = 0;
			for (int i = 0; i < polyCount; ++i)
			{		
				int indexOffset = i * numberOfSides;
				for (int j = 0; j < (numberOfSides - 2); j++)
				{
					triIndices[counter + (j * 3)] = indices[indexOffset];
					triIndices[counter + 1 + (j * 3)] = indices[indexOffset + 1 + j];
					triIndices[counter + 2 + (j * 3)] = indices[indexOffset + 2 + j];
				}
				counter += (trisPerPoly * 3);
			}	
			return triIndices;
		}
	}
}
