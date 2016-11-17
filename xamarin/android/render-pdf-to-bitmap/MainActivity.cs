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
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            var assembly = Assembly.GetExecutingAssembly();
            var inputStream = new MemoryStream();

            using (Stream resourceStream = assembly.GetManifestResourceStream("DrawPdf.Android.tiger.pdf"))
            {
                resourceStream.CopyTo(inputStream);
            }

            using (var outputStream = new MemoryStream())
            {
                await Task.Run(() =>
                {
                    Document document = new Document(inputStream);
                    Page page = document.Pages[0];
                    page.SaveAsBitmap(outputStream, CompressFormat.Png, 72);
                });

                Bitmap bmp = global::Android.Graphics.BitmapFactory.DecodeByteArray(outputStream.GetBuffer(), 0, (int) outputStream.Length);

                ImageView imageView = FindViewById<ImageView>(Resource.Id.imageView);
                imageView.SetImageBitmap(bmp);
            }
        }
    }
}