using System;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Collections.Generic;
namespace Gametest
{
	public class Shader
	{
		int _glHandle;
		int vertexHandle;
		int fragHandle;
		
		public int Handle
		{
			get {return _glHandle;}
		}
		

		public Dictionary <string, int> uniforms;
		private List<string> uniformPrototypes;
		
		public void SetUniformLocations()
		{
			foreach (string unifName in uniformPrototypes)
			{
				if (!uniforms.ContainsKey(unifName))
				{	
					int uniformLocation = GL.GetUniformLocation(_glHandle, unifName);
					if (uniformLocation < 0)
						throw new Exception("could not find uniform: " + unifName + " in shader");
					uniforms.Add(unifName, uniformLocation);
				}
			}	
		}
			
		private void GetUniforms(string shaderSource)
		{
			string[] lines = shaderSource.Split('\n');
			foreach (string line in lines)
			{
				if (line.Contains("uniform"))
				{
					string[] parts = line.Split(" ;".ToCharArray());
					if (line.Contains("[") && line.Contains("]"))
					{
						int lSqBracketIndex = parts[2].IndexOf('[');
						int digitLength = parts[2].IndexOf(']') - lSqBracketIndex;
						int attrNum;
						int.TryParse(parts[2].Substring(lSqBracketIndex + 1, digitLength - 1), out attrNum);
						for (int i = 0; i < attrNum; ++i)
						{
							uniformPrototypes.Add(parts[2].Substring(0, lSqBracketIndex) + "[" + i.ToString() + "]");
						}
					}
					else
					{
						uniformPrototypes.Add(parts[2]);
					}
				}
				if (line.Contains("main()"))
					return;
			}
		}
		
		public Shader (string vertexFilename, string fragmentFilename)
		{

			uniformPrototypes = new List<string>();
			uniforms = new Dictionary<string, int>();
			
			vertexHandle = GL.CreateShader(ShaderType.VertexShader);
			var vertexSource = File.ReadAllText(vertexFilename);
			
			GL.ShaderSource(vertexHandle, vertexSource);
			
			fragHandle = GL.CreateShader(ShaderType.FragmentShader);
			var fragSource = File.ReadAllText(fragmentFilename);
			GL.ShaderSource(fragHandle, fragSource);
			
			GetUniforms(vertexSource);
			GetUniforms(fragSource);
			
			GL.CompileShader(vertexHandle);	
			GL.CompileShader(fragHandle);
			
			_glHandle = GL.CreateProgram();
			GL.AttachShader(_glHandle, vertexHandle);
			GL.AttachShader(_glHandle, fragHandle);
			GL.LinkProgram(_glHandle);
			
			Console.WriteLine(GL.GetProgramInfoLog(_glHandle));	
			
			int count;
			GL.GetProgram(_glHandle, ProgramParameter.ActiveUniforms, out count);
			Console.WriteLine(count.ToString() + " active uniforms");
			
		}	
	}
}
