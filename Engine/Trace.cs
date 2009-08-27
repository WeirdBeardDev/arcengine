﻿using System;
using System.Collections.Generic;
using System.Text;
using Diag = System.Diagnostics;
using OpenTK.Graphics;
using System.Reflection;

namespace ArcEngine
{
	/// <summary>
	/// Provides a set of methods and properties that help you trace the execution of your code.
	/// </summary>
	public static class Trace
	{
		/// <summary>
		/// Constructor
		/// </summary>
		static Trace()
		{
			FileName = "trace.log";

			if (System.IO.File.Exists(FileName))
				System.IO.File.Delete(FileName);

			Diag.Trace.Listeners.Add(new Diag.TextWriterTraceListener(FileName));
			Diag.Trace.AutoFlush = true;

			TraceInventory();
			
		}


		#region Inventory


		/// <summary>
		/// Log inventory 
		/// </summary>
		public static void TraceInventory()
		{
			WriteLine(DateTime.Now.ToString());
			WriteLine("Hardware informations :");
			Indent();
			WriteLine("OS : {0}", Environment.OSVersion.ToString());
			WriteLine("Platform : {0}", Environment.OSVersion.Platform.ToString());
			WriteLine("SP : {0}", Environment.OSVersion.ServicePack);
			WriteLine("Processor count : {0}", Environment.ProcessorCount);
			Unindent();

			WriteLine("Software informations :");
			Indent();
			WriteLine("CLR : {0}", Environment.Version.ToString());
			Unindent();

/*

			WriteLine("Video informations :");
			Indent();
			WriteLine("Graphics card vendor : {0}", GL.GetString(StringName.Vendor));
			WriteLine("Renderer : {0}", GL.GetString(StringName.Renderer));
			int major, minor;
			GL.GetInteger(GetPName.MajorVersion, out major);
			GL.GetInteger(GetPName.MinorVersion, out minor);
			WriteLine("Version : {0} ({1}, {2})", GL.GetString(StringName.Version), major, minor);


			WriteLine("Display modes");
			Indent();

			foreach (DisplayDevice device in DisplayDevice.AvailableDisplays)
				WriteLine(device.ToString());

			Unindent();
*/
			Unindent();

			// Look through each loaded dll
			List<string> list = new List<string>();
			foreach (AssemblyName name in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
				list.Add(name.FullName);
			list.Sort();

			WriteLine("Referenced assemblies :");
			Indent();
			foreach (string name in list)
				WriteLine(name);
			Unindent();







		}

		#endregion


		#region Assert

		/// <summary>
		/// Checks for a condition, and outputs the call stack if the condition is false.
		/// </summary>
		/// <param name="condition"></param>
		public static void Assert(bool condition)
		{
			Diag.Trace.Assert(condition);

			if (OnAssert != null)
				OnAssert();
		}


		/// <summary>
		/// Checks for a condition, and displays a message if the condition is false.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		public static void Assert(bool condition, string message)
		{
			Diag.Trace.Assert(condition, message);

			if (OnAssert != null)
				OnAssert();
		}

		#endregion


		#region Write

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public static void Write(string message)
		{
			Diag.Trace.Write(message);

			if (OnTrace != null)
				OnTrace(message);
		}



		/// <summary>
		/// Writes to the log
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public static void Write(string format, params object[] args)
		{
			Write(string.Format(format, args));
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		public static void WriteIf(bool condition, string message)
		{
			Diag.Trace.WriteIf(condition, message);

			if (OnTrace != null)
				OnTrace(message);
		}

		/// <summary>
		/// Writes a line to the log
		/// </summary>
		/// <param name="format"></param>		/// <param name="args"></param>
		/// <param name="condition"></param>
		public static void WriteIf(bool condition, string format, params object[] args)
		{
			WriteIf(condition, string.Format(format, args));
		}


		#endregion


		#region WriteLine

		/// <summary>
		/// Writes a line to the log
		/// </summary>
		/// <param name="message"></param>
		public static void WriteLine(string message)
		{
			Diag.Trace.WriteLine(message);

			if (OnTrace != null)
				OnTrace(message + Environment.NewLine);
		}

		/// <summary>
		/// Writes a line to the log
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public static void WriteLine(string format, params object[] args)
		{
			WriteLine(string.Format(format, args));
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		public static void WriteLineIf(bool condition, string message)
		{
			Diag.Trace.WriteLineIf(condition, message);

			if (OnTrace != null)
				OnTrace(message + Environment.NewLine);
		}

		/// <summary>
		/// Writes a line to the log
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <param name="condition"></param>
		public static void WriteLineIf(bool condition, string format, params object[] args)
		{
			WriteLineIf(condition, string.Format(format, args));
		}



		#endregion


		#region Indent

		/// <summary>
		/// 
		/// </summary>
		public static void Indent()
		{
			Diag.Trace.Indent();
		}


		/// <summary>
		/// 
		/// </summary>
		public static void Unindent()
		{
			Diag.Trace.Unindent();
		}



		#endregion


		#region Events

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public delegate void OnTraceEvent(string message);

		/// <summary>
		/// Event fired when a trace occure
		/// </summary>
		public static event OnTraceEvent OnTrace;


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public delegate void OnAssertEvent();

		/// <summary>
		/// Event fired when an assert occure
		/// </summary>
		public static event OnAssertEvent OnAssert;




		#endregion


		#region Properties


		/// <summary>
		/// Filename of the log
		/// </summary>
		static string FileName;

		#endregion
	}
}
