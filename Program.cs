using System;
using System.Threading;
using System.Windows.Forms;

namespace PoE_Price_Lister
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
#if !DEBUG
			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			Shutdown(e.Exception);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Shutdown(e.ExceptionObject as Exception);
		}

		static void Shutdown(Exception e)
		{
			MessageBox.Show(e.Message + "\n" + e.StackTrace, "Exception");
			if (e.InnerException != null)
				MessageBox.Show(e.Message + "\n" + e.InnerException.StackTrace, "Inner Exception");
			Application.Exit();
		}
	}
}
