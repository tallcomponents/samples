using System;
using System.IO;
using System.Net;

using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Colors;
using TallComponents.PDF.Layout.Paragraphs;

using XamarinPDFGen;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(XamarinPDFGen.iOS.CustomWebViewRenderer))]

namespace XamarinPDFGen.iOS
{
    public class CustomWebViewRenderer : ViewRenderer<CustomWebView, UIWebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new UIWebView());
            }
            if (e.OldElement != null)
            {
                // Cleanup
            }
            if (e.NewElement != null)
            {
                var customWebView = Element as CustomWebView;

                string bundlePath = NSBundle.MainBundle.BundlePath;
                string xmlPath = Path.Combine(bundlePath, WebUtility.UrlEncode(customWebView.Uri));

                Document document = new Document();
                document.Read(xmlPath);

                // convert to PDF and save
                string pdfPath = Path.Combine(bundlePath, "out.pdf");
                using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
                {
                    document.Write(fs);
                }

                Control.LoadRequest(new NSUrlRequest(new NSUrl(pdfPath, false)));
                Control.ScalesPageToFit = true;
            }
        }
    }
}