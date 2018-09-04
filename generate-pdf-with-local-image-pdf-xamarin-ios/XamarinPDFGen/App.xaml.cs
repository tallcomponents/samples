using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;
using System.Reflection;

namespace XamarinPDFGen
{
	public partial class App : Application
	{
        public App ()
		{
			InitializeComponent();

            MainPage = new XamarinPDFGen.MainPage();
		}

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
