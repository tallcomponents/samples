using System;
using System.IO;
using Foundation;
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;

namespace XamarinPDFGen
{
    public class ResourceImage : Image
    {
        public ResourceImage()
        {
        }

        protected override void Compose(Document doc)
        {
            base.Path = System.IO.Path.Combine(NSBundle.MainBundle.BundlePath, base.Path);
        }
    }
}
