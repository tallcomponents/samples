using System;
using System.IO;
using System.Text;

using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

using TallComponents.PDF;
using TallComponents.PDF.Fonts;
using TallComponents.PDF.Shapes;
using TallComponents.PDF.Transforms;
using TallComponents.PDF.Forms.Fields;
using TallComponents.PDF.DigitalSignatures;
using TallComponents.PDF.Annotations.Widgets;

namespace CustomSignatureHandler
{
   class Program
   {
      static void Main( string[] args )
      {
         X509Certificate2 certificate = GetCertificate();

         SignatureHandler signHandler = new SampleSignatureHandler( certificate );
         GenerateDocument( signHandler, "test" );

         SignatureHandler verifyHandler = new SampleSignatureHandler();
         VerifyDocument( verifyHandler, "test" );

         // display used cert
         X509Certificate2UI.DisplayCertificate( certificate );
      }

      private static X509Certificate2 GetCertificate()
      {
         // retrieve store
         X509Store store = new X509Store( "MY", StoreLocation.CurrentUser );
         store.Open( OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly );

         // retrieve certificates that can be used.
         var certificates = store.Certificates.Find( X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, true );

         // close the store
         store.Close();

         X509Certificate2 certificate = null;
         X509Certificate2Collection selectedCertificates = X509Certificate2UI.SelectFromCollection( certificates, "Available certificates", "Please select a certificate", X509SelectionFlag.SingleSelection );
         if (certificates.Count > 0)
         {
            // pick a certificate (Note System.Security is referenced for this usage only)

            if (selectedCertificates.Count > 0)
            {
               // retrieve the first selected certificate, multiple selection is prohibit.
               certificate = selectedCertificates[0];
            }
         }
         else
         {
            throw new NotSupportedException( "No certificate can be found" );
         }

         return certificate;
      }

      private static void GenerateDocument( SignatureHandler handler, string fileName )
      {
         Document document = new Document();

         var page = new Page( PageSize.A4 );
         document.Pages.Add( page );
         
         // add descriptive text
         var textShape = new MultilineTextShape();
         textShape.Transform = new TranslateTransform( 20, page.Height - 50 );
         textShape.Width = page.Width - 100;
         textShape.Fragments.Add( new Fragment($"{handler.Filter} / {handler.SubFilter}", Font.HelveticaBold, 12 ) );
         page.VisualOverlay.Add( textShape );

         // create SignatureField
         var signatureField = new SignatureField( "SignatureField" );
         var signatureWidget = new SignatureWidget( 10, page.Height - 200, 100, 50 );
         signatureField.Widgets.Add( signatureWidget );

         // suppress date, so the regression tests can reproduce the same result.
         var appearance = new SignatureAppearance
         {
            DisplaySettings = (DisplaySettings) (DisplaySettings.All - DisplaySettings.Date)
         };
         signatureWidget.SignedAppearance = appearance;

         signatureField.SignatureHandler = handler;

         signatureField.ContactInfo = "Robert";
         signatureField.Location = "The Netherlands";
         signatureField.Reason = "I Agree...";

         // add the signaturefield/widget to the document/page
         document.Fields.Add( signatureField );
         page.Widgets.Add( signatureWidget );

         // write document to disk.
         using (FileStream fileStream = new FileStream($@"{fileName}.pdf", FileMode.Create, FileAccess.ReadWrite ))
         {
            document.Write( fileStream );
         }
      }

      private static void VerifyDocument( SignatureHandler handler, string fileName )
      {
         using (FileStream fs = new FileStream($@"{fileName}.pdf", FileMode.Open, FileAccess.Read ))
         {
            var document = new Document( fs );

            // Verify each signature
            foreach (var field in document.Fields)
            {
               if (field is SignatureField sigField)
               {
                  if (sigField.IsSigned)
                  {
                     bool result = sigField.Verify( handler );

                     if (result)
                     {
                        Console.WriteLine( "Signature {0}:{1} is {2}.", fileName, field.FullName, "valid." );
                     }
                     else
                     {
                        Console.WriteLine( "Signature {0}:{1} is {2}.", fileName, field.FullName, "invalid." );
                     }
                  }
                  else
                  {
                     Console.WriteLine( "Signature {0}:{1} is {2}.", fileName, field.FullName, "not signed" );
                  }
               }
            }
         }
      }
   }
}
