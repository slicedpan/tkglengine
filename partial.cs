ASCII Contents of Fragment 3291145 in sda2-0-0


ata, normalIndex * sourceStride, datArray, 2, 3);
						
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
			
			if (semantics.Con



