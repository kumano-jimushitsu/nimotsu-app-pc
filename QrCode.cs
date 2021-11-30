using System;
using System.Collections.Generic;
using System.Text;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;

namespace RegisterParcelsFromPC
{
    class QrCode
    {
        //https://marunaka-blog.com/c-sharp-zxing-qrcode/2092/

        public void QRcodeCreate(string code, string filename)
        {
            var qrCode = new BarcodeWriter
            {
                // バーコードの種類を選択
                Format = BarcodeFormat.QR_CODE,
                // QRコードのオプション設定
                Options = new QrCodeEncodingOptions
                {
                    QrVersion = 4,
                    ErrorCorrection = ErrorCorrectionLevel.M,
                    CharacterSet = "UTF-8",
                    Width = 200,
                    Height = 200,
                    Margin = 5,
                },
            };
            Image image;
            //QRコード生成
            using (var bmp = qrCode.Write(code))
            using (var ms = new FileStream(filename,FileMode.OpenOrCreate))
            {
                bmp.Save(ms, ImageFormat.Gif);
            }

        }
    }
}
