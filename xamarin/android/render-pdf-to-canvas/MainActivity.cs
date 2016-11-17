using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Graphics;

using TallComponents.PDF.Rasterizer;
using static Android.Graphics.Bitmap;

namespace DrawPdf.Android
{
    [Activity(Label = "DrawPdf.Android", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var assembly = Assembly.GetExecutingAssembly();
            var inputStream = new MemoryStream();
            using (Stream resourceStream = assembly.GetManifestResourceStream("DrawPdf.Android.tiger.pdf"))
            {
                resourceStream.CopyTo(inputStream);
            }

            Document document = new Document(inputStream);
            Page page = document.Pages[0];
            SetContentView(new PdfPageView(this, page));
        }
    }
}