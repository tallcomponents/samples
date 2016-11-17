using System;

using Android.Content;
using Android.Views;
using Android.Graphics;

using TallComponents.PDF.Rasterizer;

namespace DrawPdf.Android
{
    public class PdfPageView : View
    {
        Page _page;

        public PdfPageView(Context context, Page page) :
            base(context)
        {
            _page = page;
        }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.Scale(2, 2);
            _page.Draw(canvas, 1);
        }
    }
}
