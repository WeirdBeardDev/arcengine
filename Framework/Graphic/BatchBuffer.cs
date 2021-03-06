﻿#region Licence
//
//This file is part of ArcEngine.
//Copyright (C)2008-2010 Adrien Hémery ( iliak@mimicprod.net )
//
//ArcEngine is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//any later version.
//
//ArcEngine is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using TK = OpenTK.Graphics.OpenGL;

//
// http://www.spec.org/gwpg/gpc.static/vbo_whitepaper.html
// http://bakura.developpez.com/tutoriels/jeux/utilisation-vbo-avec-opengl-3-x/

namespace ArcEngine.Graphic
{
	/// <summary>
	/// Vertex buffer
	/// </summary>
	public class BatchBuffer : IDisposable
	{

		/// <summary>
		/// Constructor
		/// </summary>
		public BatchBuffer()
		{
			int id = -1;
            TK.GL.GenBuffers(1, out id);
			Handle = id;

            UsageMode = BufferUsage.StaticDraw;

			Declarations = new List<VertexDeclaration>();

			Buffer = new float[8192];
			Offset = 0;
		}


		/// <summary>
		/// Destructor
		/// </summary>
		~BatchBuffer()
		{
			//throw new Exception("BatchBuffer : Call Dispose() !!");
			System.Windows.Forms.MessageBox.Show("BatchBuffer : Call Dispose() !!", "ERROR", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
		}


		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			int id = Handle;
            TK.GL.DeleteBuffers(1, ref id);
			Handle = -1;

			//GL.DeleteVertexArrays(1, ref vaoHandle);
			//vaoHandle = -1;


			GC.SuppressFinalize(this);
		}


		/// <summary>
		/// Sets vertices
		/// </summary>
		/// <param name="data">Data to send to the buffer</param>
		public void SetVertices(float[] data)
		{
			int count = 0;
			if (data != null)
				count = data.Length;

            TK.GL.BindBuffer(TK.BufferTarget.ArrayBuffer, Handle);
            TK.GL.BufferData(TK.BufferTarget.ArrayBuffer, (IntPtr)(count * sizeof(float)), data, (TK.BufferUsageHint)UsageMode);
		}


		/// <summary>
		/// Updates the buffer with the internal vertex data
		/// </summary>
		/// <returns>Number of primitives</returns>
		public int Update()
		{
			SetVertices(Buffer);

			int count = 0;
			if (Stride > 0)
				count = Offset / Stride;
			Offset = 0;

			return count;
		}


		/// <summary>
		/// Binds the buffer
		/// </summary>
		/// <returns>True if successful</returns>
		public bool Bind()
		{
			if (Handle == -1)
				return false;

			TK.GL.BindBuffer(TK.BufferTarget.ArrayBuffer, Handle);

			if (Display.Shader == null)
				return false;

			int offset = 0;
			foreach (VertexDeclaration dec in Declarations)
			{
				int id = Display.Shader.GetAttribute(dec.Name);

				Display.EnableBufferIndex(id);
				Display.SetBufferDeclaration(id, dec.Size, Stride * sizeof(float), offset);

				offset += dec.Size * sizeof(float);
			}

			return true;
		}


		/// <summary>
		/// Adds a vertex declation to the buffer
		/// </summary>
		/// <param name="name">Specifies the name of the generic vertex attribute to be modified.</param>
		/// <param name="size">Specifies the number of components per generic vertex attribute.</param>
		public void AddDeclaration(string name, int size)
		{
			Declarations.Add(new VertexDeclaration(name, size));

			Stride += size;
		}


		/// <summary>
		/// Clear vertex declarations
		/// </summary>
		public void ClearDeclaration()
		{
			Declarations.Clear();
			Stride = 0;
		}


		/// <summary>
		/// Creates a defaut buffer containing position, color and texture data
		/// </summary>
		/// <returns>Batch buffer</returns>
		public static BatchBuffer CreatePositionColorTextureBuffer()
		{
			BatchBuffer buffer = new BatchBuffer();
			buffer.AddDeclaration("in_position", 3);
			buffer.AddDeclaration("in_color", 4);
			buffer.AddDeclaration("in_texture", 2);

			return buffer;
		}


		/// <summary>
		/// Creates a defaut buffer containing position, color and texture data
		/// </summary>
		/// <returns>Batch buffer</returns>
		public static BatchBuffer CreatePositionColorBuffer()
		{
			BatchBuffer buffer = new BatchBuffer();
			buffer.AddDeclaration("in_position", 3);
			buffer.AddDeclaration("in_color", 4);

			return buffer;
		}


		/// <summary>
		/// Clear the buffer
		/// </summary>
		public void Clear()
		{
			Offset = 0;
		}


		#region Helpers


		#region Rectangles

		/// <summary>
		/// Adds a colored rectangle
		/// </summary>
		/// <param name="rect">Rectangle on the screen</param>
		/// <param name="color">Drawing color</param>
		public void AddRectangle(Rectangle rect, Color color)
		{
			AddPoint(rect.Location, color);									// A
			AddPoint(new Point(rect.X, rect.Bottom), color);				// D
			AddPoint(new Point(rect.Right, rect.Bottom), color);			// C

			AddPoint(rect.Location, color);									// A
			AddPoint(new Point(rect.Right, rect.Bottom), color);			// C
			AddPoint(new Point(rect.Right, rect.Top), color);				// B
		}


		/// <summary>
		/// Adds a textured rectangle
		/// </summary>
		/// <param name="rect">Rectangle on the screen</param>
		/// <param name="color">Drawing color</param>
		/// <param name="tex">Texture coordinate</param>
		public void AddRectangle(Rectangle rect, Color color, Rectangle tex)
		{
			AddPoint(rect.Location, color, tex.Location);												// A
			AddPoint(new Point(rect.X, rect.Bottom), color, new Point(tex.X, tex.Bottom));				// D
			AddPoint(new Point(rect.Right, rect.Bottom), color, new Point(tex.Right, tex.Bottom));		// C

			AddPoint(rect.Location, color, tex.Location);												// A
			AddPoint(new Point(rect.Right, rect.Bottom), color, new Point(tex.Right, tex.Bottom));		// C
			AddPoint(new Point(rect.Right, rect.Top), color, new Point(tex.Right, tex.Top));			// B
		}


		/// <summary>
		/// Adds a textured rectangle
		/// </summary>
		/// <param name="destination">Rectangle on the screen</param>
		/// <param name="color">Drawing color for each corner</param>
		/// <param name="texture">Texture coordinate</param>
		public void AddRectangle(Rectangle destination, Color[] color, Rectangle texture)
		{
			if (color.Length != 4)
				return;

			AddPoint(destination.Location, color[0], texture.Location);															// A
			AddPoint(new Point(destination.X, destination.Bottom), color[3], new Point(texture.X, texture.Bottom));				// D
			AddPoint(new Point(destination.Right, destination.Bottom), color[2], new Point(texture.Right, texture.Bottom));		// C

			AddPoint(destination.Location, color[0], texture.Location);															// A
			AddPoint(new Point(destination.Right, destination.Bottom), color[2], new Point(texture.Right, texture.Bottom));		// C
			AddPoint(new Point(destination.Right, destination.Top), color[1], new Point(texture.Right, texture.Top));			// B
		}


		/// <summary>
		/// Adds a textured rectangle
		/// </summary>
		/// <param name="destination">Rectangle on the screen</param>
		/// <param name="color">Drawing color for each corner</param>
		/// <param name="texture">Texture coordinate</param>
		public void AddRectangle(Vector4 destination, Color color, Vector4 texture)
		{
			AddPoint(destination.Xy, color, texture.Xy);																			// A
			AddPoint(new Vector2(destination.X, destination.Bottom), color, new Vector2(texture.X, texture.Bottom));				// D
			AddPoint(new Vector2(destination.Right, destination.Bottom), color, new Vector2(texture.Right, texture.Bottom));		// C

			AddPoint(destination.Xy, color, texture.Xy);																			// A
			AddPoint(new Vector2(destination.Right, destination.Bottom), color, new Vector2(texture.Right, texture.Bottom));		// C
			AddPoint(new Vector2(destination.Right, destination.Top), color, new Vector2(texture.Right, texture.Top));				// B
		}


		/// <summary>
		/// Adds a textured rectangle
		/// </summary>
		/// <param name="destination">Rectangle on the screen</param>
		/// <param name="color">Drawing color for each corner</param>
		/// <param name="texture">Texture coordinate</param>
		public void AddRectangle(Vector4 destination, Color[] color, Vector4 texture)
		{
			if (color.Length != 4)
				return;

			AddPoint(new Vector2(destination.X, destination.Bottom), color[3], new Vector2(texture.X, texture.Bottom));				// D
			AddPoint(destination.Xy, color[0], texture.Xy);																			// A
			AddPoint(new Vector2(destination.Right, destination.Bottom), color[2], new Vector2(texture.Right, texture.Bottom));		// C

			AddPoint(destination.Xy, color[0], texture.Xy);																			// A
			AddPoint(new Vector2(destination.Right, destination.Bottom), color[2], new Vector2(texture.Right, texture.Bottom));		// C
			AddPoint(new Vector2(destination.Right, destination.Top), color[1], new Vector2(texture.Right, texture.Top));			// B
		}

		#endregion


		/// <summary>
		/// Check for buffer overflow
		/// </summary>
		void CheckForOverflow()
		{
			if (Offset + Stride >= Buffer.Length)
				Array.Resize<float>(ref Buffer, Buffer.Length * 2);
		}


		#region Points

		/// <summary>
		/// Adds a textured point
		/// </summary>
		/// <param name="point">Location on the screen</param>
		/// <param name="color">Color of the point</param>
		/// <param name="texture">Texture coordinate</param>
		public void AddPoint(Point point, Color color, Point texture)
		{
			CheckForOverflow();

			// Vertex
			Buffer[Offset++] = point.X;
			Buffer[Offset++] = point.Y;
			Buffer[Offset++] = 0.0f;

			// Color
			Buffer[Offset++] = (color.R / 255.0f);
			Buffer[Offset++] = (color.G / 255.0f);
			Buffer[Offset++] = (color.B / 255.0f);
			Buffer[Offset++] = (color.A / 255.0f);

			// Texture
			Buffer[Offset++] = (texture.X);
			Buffer[Offset++] = (texture.Y);
		}


		/// <summary>
		/// Adds a textured point
		/// </summary>
		/// <param name="point">Location on the screen</param>
		/// <param name="color">Color of the point</param>
		/// <param name="texture">Texture coordinate</param>
		public void AddPoint(Vector2 point, Color color, Vector2 texture)
		{
			AddPoint(new Vector3(point), color, texture);
		}


		/// <summary>
		/// Adds a textured point
		/// </summary>
		/// <param name="point">Location on the screen</param>
		/// <param name="color">Color of the point</param>
		/// <param name="texture">Texture coordinate</param>
		public void AddPoint(Vector3 point, Color color, Vector2 texture)
		{
			CheckForOverflow();

			// Vertex
			Buffer[Offset++] = point.X;
			Buffer[Offset++] = point.Y;
			Buffer[Offset++] = point.Z;

			// Color
			Buffer[Offset++] = (float)(color.R) / 255.0f;
			Buffer[Offset++] = (float)(color.G) / 255.0f;
			Buffer[Offset++] = (float)(color.B) / 255.0f;
			Buffer[Offset++] = (float)(color.A) / 255.0f;
							   
			// Texture		   
			Buffer[Offset++] = texture.X;
			Buffer[Offset++] = texture.Y;
		}


		/// <summary>
		/// Adds a colored point
		/// </summary>
		/// <param name="point">Location on the screen</param>
		/// <param name="color">Color of the point</param>
		public void AddPoint(Point point, Color color)
		{
			CheckForOverflow();

			// Vertex
			Buffer[Offset++] = point.X;
			Buffer[Offset++] = point.Y;
			Buffer[Offset++] = 0.0f;

			// Color
			Buffer[Offset++] = (color.R / 255.0f);
			Buffer[Offset++] = (color.G / 255.0f);
			Buffer[Offset++] = (color.B / 255.0f);
			Buffer[Offset++] = (color.A / 255.0f);

			Buffer[Offset++] = 0.0f;
			Buffer[Offset++] = 0.0f;

		}

		#endregion


		/// <summary>
		/// Adds a colored line
		/// </summary>
		/// <param name="from">Start point</param>
		/// <param name="to">Ending point</param>
		/// <param name="color">Color of the line</param>
		public void AddLine(Point from, Point to, Color color)
		{
			AddPoint(from, color);
			AddPoint(to, color);
		}

		#endregion


		#region Properties

		/// <summary>
		/// Buffer internal handles
		/// </summary>
		internal int Handle
		{
			get;
			private set;
		}


		/// <summary>
		/// Usage mode
		/// </summary>
		public BufferUsage UsageMode
		{
			get;
			set;
		}



		/// <summary>
		/// Vertex declarations
		/// </summary>
		List<VertexDeclaration> Declarations;



		/// <summary>
		/// Vertex buffer data
		/// </summary>
		float[] Buffer;


		/// <summary>
		/// Offset in the buffer
		/// </summary>
		int Offset;


		/// <summary>
		/// Stride
		/// </summary>
		public int Stride
		{
			get;
			private set;
		}

		#endregion

	}

	/// <summary>
	/// Declares the format of a set of vertex inputs
	/// </summary>
	struct VertexDeclaration
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Specifies the name of the generic vertex attribute to be modified.</param>
		/// <param name="size">Specifies the number of components per generic vertex attribute.</param>
		public VertexDeclaration(string name, int size)
		{
			Name = name;
			Size = size;
		}

		/// <summary>
		/// Name of the attribute
		/// </summary>
		public string Name;


		/// <summary>
		/// Size of the elements
		/// </summary>
		public int Size;

	}


    /// <summary>
    /// Specifies special usage of the buffer contents
    /// </summary>
    public enum BufferUsage
    {

		/// <summary>
		/// 
		/// </summary>
		StreamDraw = TK.BufferUsageHint.StreamDraw,

		/// <summary>
		/// 
		/// </summary>
		StreamRead = TK.BufferUsageHint.StreamRead,

		/// <summary>
		/// 
		/// </summary>
		StreamCopy = TK.BufferUsageHint.StreamCopy,

		/// <summary>
		/// 
		/// </summary>
		StaticDraw = TK.BufferUsageHint.StaticDraw,

		/// <summary>
		/// 
		/// </summary>
		StaticRead = TK.BufferUsageHint.StaticRead,

		/// <summary>
		/// 
		/// </summary>
		StaticCopy = TK.BufferUsageHint.StaticCopy,

		/// <summary>
		/// 
		/// </summary>
		DynamicDraw = TK.BufferUsageHint.DynamicDraw,

		/// <summary>
		/// 
		/// </summary>
		DynamicRead = TK.BufferUsageHint.DynamicRead,

		/// <summary>
		/// 
		/// </summary>
        DynamicCopy = TK.BufferUsageHint.DynamicCopy,
    }
}
