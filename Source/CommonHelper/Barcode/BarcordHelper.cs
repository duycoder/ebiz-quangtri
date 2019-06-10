using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public static class BarcordHelper
    {
        public static Stream CreateBarcode(string strSerial)
        {
            string key = revertNumber(strSerial.Substring(0, strSerial.Length - 1));
            string strBar = string.Format("{0:000000000000}", EncriptSerial(strSerial, key));
            BarcodeLib.Barcode bar = new BarcodeLib.Barcode();
            bar.Alignment = BarcodeLib.AlignmentPositions.CENTER;
            bar.IncludeLabel = true;
            bar.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
            Image img = bar.Encode(BarcodeLib.TYPE.CODE128, strBar, 150, 50);

            //Stream ms = null;
            Stream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);


            return ms;
        }


        public static long EncriptSerial(string strSerial, string key)
        {
            int stt = int.Parse(strSerial[strSerial.Length - 1].ToString());
            int id = int.Parse(strSerial.Substring(0, strSerial.Length - 1));
            //id = id * 7;
            stt = convertSTT(stt);
            id = id * stt;
            return long.Parse(id.ToString() + stt.ToString()) ^ long.Parse(key);
        }

        private static int convertSTT(int stt)
        {
            Random rnd = new Random();
            int num = rnd.Next(25, 248);
            num = num * 4 + stt;
            return num;
        }

        private static int revertSTT(int stt)
        {
            return stt % 4;
        }

        public static long DecriptSerial(string strSerial, string key)
        {
            string res = (long.Parse(strSerial) ^ long.Parse(key)).ToString();
            int stt = int.Parse(res.Substring(res.Length - 3));
            int id = int.Parse(res.Substring(0, res.Length - 3));
            id = id / stt;
            stt = revertSTT(stt);
            return int.Parse(id.ToString() + stt.ToString());
        }

        public static string revertNumber(string number)
        {
            string res = "";
            foreach (char s in number)
            {
                if (s == '0')
                {
                    res += s;
                }
                else
                {
                    res += (10 - int.Parse(s.ToString()));
                }
            }
            
            return res;
        }

    }


}
