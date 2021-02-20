using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using TallComponents.PDF.DigitalSignatures;

namespace CustomSignatureHandler
{
   class SampleSignatureHandler : SignatureHandler
   {
      private readonly X509Certificate2 _signingCertificate;

      public SampleSignatureHandler()
      {
      }

      public SampleSignatureHandler( X509Certificate2 signingCertificate )
      {
         _signingCertificate = signingCertificate;
      }

      /// <summary>
      /// The name of this signature handler. 
      /// </summary>
      public override string Filter => "Adobe.PPKLite";

      /// <summary>
      /// The name that identifies the encoding of the signature value and key information. 
      /// </summary>
      public override string SubFilter => "adbe.pkcs7.detached";

      /// <summary>
      /// The date and time of signing.
      /// </summary>
      public override DateTime SignDate => DateTime.Now;

      /// <summary>
      /// The subject name in the signing certificate (the element "CN")
      /// </summary>
      public override string Name => _signingCertificate.SubjectName.Name;

      /// <summary>
      /// The Distinguished Name of the person or authority signing the document (the element "DN"). 
      /// </summary>
      public override string DistinguishedName => _signingCertificate.IssuerName.Name;

      /// <summary>
      /// Revision number of the SignatureHandler.
      /// </summary>
      public override int Revision => 131101;

      /// <summary>
      /// You can specify if you want to use the PropertyBuild structure or not. (default false) 
      /// </summary>
      public override bool UsePropBuild => true;

      /// <summary>
      /// The maximum length in bytes of the digest as returned by the Sign function (default 300). 
      /// </summary>
      public override int MaxDigestLength
      {
         get
         {
            var chain =
               new X509Chain
               {
                  ChainPolicy =
                  {
                     RevocationMode = X509RevocationMode.Offline
                  }
               };
            chain.Build( _signingCertificate );

            X509Certificate2Collection certificateList =
              new X509Certificate2Collection();

            foreach (var element in chain.ChainElements)
            {
               certificateList.Add( element.Certificate );
            }


            var certBytes = certificateList.Export( X509ContentType.Pkcs7 );
            
            return 1600 + certBytes.Length; //1600 is the max enveloped Pkcs#7 data.
         }
      }

      /// <summary>
      /// Returns true if and only if this signature handler can be used to sign a document. 
      /// </summary>
      public override bool CanSign => (null != _signingCertificate) && _signingCertificate.HasPrivateKey;

      /// <summary>
      /// Signs the given bytes and returns the digest. 
      /// </summary>
      public override byte[] Sign( byte[] bytesToSign )
      {
         if (null == bytesToSign) throw new ArgumentNullException(nameof(bytesToSign));

         // create a ContentInfo object from the bytes to validate
         var contentInfo = new ContentInfo(bytesToSign);

         // create a new, detached SignedCms message.
         var signedCms = new SignedCms(contentInfo, true);

         // wrap the certificate
         var signer = new CmsSigner(_signingCertificate);

         // create signature
         signedCms.ComputeSignature( signer );

         // encode the signature
         byte[] encoded = signedCms.Encode( );

         return encoded;
      }

      /// <summary>
      /// Returns true if this signature handler can be used to verify a document. 
      /// </summary>
      /// <remarks>
      /// Just return true because verification does not require any certificate.
      /// </remarks>
      public override bool CanVerify => true;

      /// <summary>
      /// This function verifies the given bytes. 
      /// </summary>
      public override bool Verify( byte[] bytesToVerify, byte[] digest, byte[][] certificates )
      {
         if (null == bytesToVerify) throw new ArgumentNullException(nameof(bytesToVerify));
         if (null == digest) throw new ArgumentNullException(nameof(digest));
         if (null != certificates) throw new ArgumentNullException(nameof(certificates));

         // create a ContentInfo object from the bytes to validate
         ContentInfo contentInfo = new ContentInfo( bytesToVerify );

         // create a new, detached SignedCms message
         SignedCms signedCms = new SignedCms( contentInfo, true );

         // decode the signature
         signedCms.Decode( digest );

         try
         {
            // check with verify signature only mode. 
            // note an exception will be thrown if not valid.
            signedCms.CheckSignature( true );
            return true;
         }
         catch
         {
            return false;
         }
      }
   }
}
