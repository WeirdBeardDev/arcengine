﻿#region Licence
//
//This file is part of ArcEngine.
//Copyright (C)2008-2009 Adrien Hémery ( iliak@mimicprod.net )
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
using System.Drawing;
using System.Windows.Forms;
using ArcEngine;
using ArcEngine.Asset;
using ArcEngine.Graphic;
using ArcEngine.Input;


namespace ArcEngine.Examples.TextDemo
{
	/// <summary>
	/// Main game class
	/// </summary>
	public class EmptyProject : Game
	{

		/// <summary>
		/// Main entry point.
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
				using (EmptyProject game = new EmptyProject())
					game.Run();
			}
			catch (Exception e)
			{
				// Oops, an error happened !
				MessageBox.Show(e.StackTrace, e.Message);
			}
		}


		/// <summary>
		/// Constructor
		/// </summary>
		public EmptyProject()
		{
			CreateGameWindow(new Size(1024, 768));
		}



		/// <summary>
		/// Load contents 
		/// </summary>
		public override void LoadContent()
		{
			Display.ClearColor = Color.CornflowerBlue;

			// Load the bank
			ResourceManager.LoadBank("data/data.bnk");

			// Creates the font
			Font = Font2d.CreateFromTTF(@"c:\windows\fonts\verdana.ttf", 14, FontStyle.Regular);
			
			// Attach the TileSet to the font
			Font.TextTileset = ResourceManager.CreateAsset<TileSet>("TextTileSet");




			Text = @"Text to display. <tile id='1'/><br>This is a < to display";

		}



		/// <summary>
		/// Unload contents
		/// </summary>
		public override void UnloadContent()
		{
			if (Font != null)
			{
				Font.Dispose();
				Font = null;
			}
		}




		/// <summary>
		/// Called when it is time to draw a frame.
		/// </summary>
		public override void Draw()
		{
			// Clears the background
			Display.ClearBuffers();

			Font.DrawText(new Point(100, 50), Text);

		}




		#region Properties


		/// <summary>
		/// Font
		/// </summary>
		Font2d Font;


		/// <summary>
		/// Text to display
		/// </summary>
		string Text;

		#endregion

	}

}
