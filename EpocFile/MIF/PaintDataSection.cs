using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Utility;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace EpocData.MIF
{

    // Invece di bitmap restutisce istanza di IImage

    public class PaintDataSection : IImage
    {
        public uint head1;
        public uint head2;
        public uint head3;
        public uint fileSize;
        public uint head4;  
        public uint colorImg;
        public uint head6;
        public uint colorMask;
        public byte[] data;
        private string _svg;


        public PaintDataSection(BinaryReader br)
        {
            head1 = br.ReadUInt32(); //Debug.WriteLine( head1 ); Debug.Assert( head1 == 874718019 );
            head2 = br.ReadUInt32(); //Debug.WriteLine( head2 ); Debug.Assert( head2 == 1 );
            head3 = br.ReadUInt32(); //Debug.WriteLine( head3 ); Debug.Assert( head3 == 32 );
            fileSize = br.ReadUInt32(); //Debug.WriteLine( "Size:"+ fileSize );
            head4 = br.ReadUInt32(); //Debug.WriteLine( head4 ); Debug.Assert( head4 == 1 );
            colorImg = br.ReadUInt32(); //Debug.WriteLine( "ColorImg:" + colorImg );
            head6 = br.ReadUInt32(); //Debug.WriteLine( head6 ); Debug.Assert( head6 == 0 );
            colorMask = br.ReadUInt32(); //Debug.WriteLine( "ColorMask:" + colorMask );
            data = br.ReadBytes( (int)fileSize ); 
        }


        public override object GetData()
        {
            if (_svg != null && _svg != "")
                return _svg;  
            if (data[0] == 0xCC)
            {
                Svgb svgb = new Svgb();
                BinaryReader br = new BinaryReader(new MemoryStream(data));
                _svg = svgb.DoReversingJob(br);
                data = null;
                br.Close();
                return _svg;
            }
            _svg = Encoding.ASCII.GetString(data);
            data = null;
            return _svg;
        }


        public override void SaveTo(string path)
        {
            string s = GetData() as string;
            System.IO.StreamWriter sw = new StreamWriter(path + ".svg", false, Encoding.ASCII);
            sw.WriteLine(s);
            sw.Close();
        }

        #region IDisposable Members

        public override void Dispose()
        {
/*            if (_bitmap != null)
                _bitmap.Dispose();
            _bitmap = null;*/
        }

        #endregion
    }
}
