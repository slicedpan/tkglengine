//ASCII Contents of Fragment 3303426 in sda2-0-0


using System;
using System.Xml;
using System.Xml.Schema;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;

namespace tkglengine
{	
	public class TestWindow : GameWindow
	{
		uint buf, buf2;
		int vbo;
		Shader shader;
		int shaderProgram;
		int wvp;
		double counter = 0;
		Matrix4 WVP;
		Vector3[] vertices;
		Mesh mesh;
		Vector3 cameraPosition;
		Vector2 cameraOrientation;
		Matrix4 world, view, projection; 
		int mouseX = 400;
		int mouseY = 300;
		MouseState lastState;
		
		public TestWindow () : 
			base (1920, 1080, GraphicsMode.Default, "test", GameWindowFlags.Fullscreen)
		{
			
			GL.ClearColor(Color4.Wheat);
			Keyboard.KeyDown += HandleKeyboardKeyDown;
			WVP = Matrix4.Identity;
			cameraPosition = Vector3.UnitZ * - 10.0f;
			cameraOrientation = Vector2.Zero;
			projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 4.0f / 3.0f, 1.0f, 1000.0f);
			lastState = new MouseState();
			Paths.Init();
		}

			void HandleKeyboardKeyDown (object sender, OpenTK.Input.KeyboardKeyEventArgs e)
			{
				if (e.Key == OpenTK.Input.Key.Escape)
					Exit();
			}	
		protected override void OnUpdateFrame (FrameEventArgs e)
		{
			world = Matrix4.Identity;
			Vector3 forwardVec = Vector3.Transform(Vector3.UnitZ, Matrix4.CreateRotationY(cameraOrientation.Y));
			Vector3 rightVec = Vector3.Transform(Vector3.UnitZ, Matrix4.CreateRotationY(cameraOrientation.Y - MathHelper.DegreesToRadians(90.0f)));
			
			Vector3 camTarget = cameraPosition + Vector3.Transform(Vector3.UnitZ, Matrix4.CreateRotationX(cameraOrientation.X) * Matrix4.CreateRotationY(cameraOrientation.Y));
			view = Matrix4.LookAt(cameraPosition, camTarget, Vector3.UnitY);
			
			if (Keyboard[Key.W])
			{
				cameraPosition += forwardVec * 0.2f;
			}
			else if (Keyboard[Key.S])
			{
				cameraPosition -= forwardVec * 0.2f;
			}
			
			if (Keyboard[Key.D])
			{
				cameraPosition += rightVec * 0.2f;
			}
			else if (Keyboard[Key.A])
			{
				cameraPosition -= rightVec * 0.2f;
			}
			
			if (Keyboard[Key.Space])
			{
				cameraPosition.Y += 0.2f;
			}
			else if (Keyboard[Key.ControlLeft])
			{
				cameraPosition.Y -= 0.2f;
			}
			
			if (Keyboard[Key.Up])
				cameraOrientation.X -= 0.02f;
			else if (Keyboard[Key.Down])
				cameraOrientation.X += 0.02f;
			
			if (Keyboard[Key.Left])
				cameraOrientation.Y += 0.02f;
			else if (Keyboard[Key.Right])
				cameraOrientation.Y -= 0.02f;
			
			var state = OpenTK.Input.Mouse.GetState();
			cameraOrientation.X += (float)(state.Y - lastState.Y) / 100.0f;
			cameraOrientation.Y -= (float)(state.X - lastState.X) / 100.0f;
			
			lastState = state;
			
			OpenTK.Input.Mouse.SetPosition((double)mouseX, (double)mouseY);			

			WVP = world * view * projection;
			
			base.OnUpdateFrame (e);
		}
		
		
		protected override void OnRenderFrame (FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);			

			GL.UseProgram(shader.Handle);
			GL.UniformMatrix4(shader.uniforms["WVP"], false, ref WVP);
			counter += 0.01f;
			Vector3 lightpos = new Vector3((float)Math.Sin(counter), (float)Math.Cos(counter), 0.0f);			
			GL.Uniform3(shader.uniforms["LightPos"], lightpos);
			GL.EnableVertexAttribArray(0);	
			GL.EnableVertexAttribArray(1);
			GL.BindBuffer(BufferTarget.ArrayBuffer, buf);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, mesh.Stride * sizeof(float), 0);			
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, mesh.Stride * sizeof(float), 12);
			GL.DrawArrays(BeginMode.Triangles, 0, mesh.VertexCount);			
			
			GL.DisableVertexAttribArray(0);
			GL.DisableVertexAttribArray(1);			
			
			SwapBuffers();
			
			base.OnRenderFrame (e);
		}
		protected override void OnLoad (EventArgs e)
		{
			
			ColladaXML daeReader = new ColladaXML("collada_schema_1_4.xsd");
			Console.WriteLine("Parsing File...");
			daeReader.Parse("test.dae");
			mesh = daeReader.Meshes[0];
			
			GL.ClearColor(Color4.Wheat);
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);
			shader = new Shader("hello-gl.v.glsl", "hello-gl.f.glsl");
			
			GL.GenBuffers(1, out buf);
			GL.BindBuffer(BufferTarget.ArrayBuffer, buf);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(mesh.VertexBuffer.Length * sizeof(float)), mesh.VertexBuffer, BufferUsageHint.StaticDraw);
			
			CreateShaders();			
			
			OpenTK.Input.Mouse.SetPosition((double)mouseX, (double)mouseY);
			lastState = OpenTK.Input.Mouse.GetState();
			
			//CursorVisible = false;
			
			base.OnLoad (e);
		}
		void CreateBuffer()
		{
			vertices = new Vector3[3];
			vertices[0] = new Vector3(-10.0f, -10.0f, 0.0f);
			vertices[1] = new Vector3(10.0f, -10.0f, 0.0f);
			vertices[2] = new Vector3(0.0f, 10.0f, 0.0f);
			
			GL.GenBuffers(1, out vbo);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * Vector3.SizeInBytes), vertices, BufferUsageHint.StaticDraw);
		}
		void CreateShaders()
		{
			string vertexSource = @"#version 330

layout(location = 0) in vec3 Position;

uniform mat4 WVP;

void main()
{	
    gl_Position = WVP * vec4(Position, 1.0);
}";
				
			string fragmentSource = @"#version 330

out vec4 frag_Color;

void main()
{
    frag_Color = vec4(0.0, 0.0, 1.0, 1.0);
}";
			int vertexShader, fragmentShader;
			
			vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, vertexSource);
			GL.CompileShader(vertexShader);
			
			fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, fragmentSource);
			GL.CompileShader(fragmentShader);
			
			shaderProgram = GL.CreateProgram();
			GL.AttachShader(shaderProgram, vertexShader);
			GL.AttachShader(shaderProgram, fragmentShader);
			GL.LinkProgram(shaderProgram);
			
			Console.WriteLine(GL.GetProgramInfoLog(shaderProgram));
			
			GL.UseProgram(shaderProgram);	
			
			wvp = GL.GetUniformLocation(shaderProgram, "WVP");
			if (wvp < 0)
				throw new Exception("could not get uniform location");
			
		}
		public static void Main()
		{
			var window = new TestWindow();
			window.Run(60);
		}
		
	}
}



