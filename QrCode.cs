using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace RegisterParcelsFromPC
{
    class QrCode
    {
        //https://marunaka-blog.com/c-sharp-zxing-qrcode/2092/

        public void QRcodeCreate(string code, string filename)
        {
            try
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
                using (var ms = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    bmp.Save(ms, ImageFormat.Gif);
                }
            }
            catch (Exception ee)
            {
                NLogService.PrintInfoLog("例外_Form7_postingTXT");

                NLogService.PrintInfoLog(ee.ToString());
            }
        }
    }
}
