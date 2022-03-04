
using System;
using System.IO;

class Redump2xiso
{
    static void Main(string[] args)
    {
        long[] REDUMP_ISO_LENGTH = {0x1D26A8000, 0x1D2FEF800, 0x1D3082000, 0x1D3390000, 0x208E03800};  // XGD1, XGD2w1, XGD2w2, XGD2w3+, XGD3
        long[] VP_LENGTH = {0x18300000, 0xFD90000, 0x2080000}; // XGD1, XGD2, XGD3
        long[] XISO_LENGTH = {REDUMP_ISO_LENGTH[0]-VP_LENGTH[0], REDUMP_ISO_LENGTH[1]-VP_LENGTH[1], REDUMP_ISO_LENGTH[2]-VP_LENGTH[1], REDUMP_ISO_LENGTH[3]-VP_LENGTH[1], REDUMP_ISO_LENGTH[4]-VP_LENGTH[2]};  // XGD1, XGD2w1, XGD2w2, XGD2w3+, XGD3
        if ((args.Length == 0) || (args.Length > 2))
        {
            Console.WriteLine("Usage: redump2xiso.exe <isofile> [<videopartitionfile>]");
            return;
        }
        string infile = Path.GetFileName(args[0]);
        string videofile = "";
        int lenidx = 0;
        int vpidx = 0;
        long inlen = new System.IO.FileInfo(infile).Length;
        if (!Array.Exists(REDUMP_ISO_LENGTH, element => element == inlen) && !Array.Exists(XISO_LENGTH, element => element == inlen))
        {
            Console.WriteLine("Error. Invalid filesize: " + infile);
            return;
        }
        bool xiso = Array.Exists(XISO_LENGTH, element => element == inlen);
        
        lenidx = xiso ? Array.IndexOf(XISO_LENGTH, inlen) : Array.IndexOf(REDUMP_ISO_LENGTH, inlen);
        
        if (lenidx == 4)
        {
            vpidx = 2;
        }
        else if (lenidx != 0)
        {
            vpidx = 1;
        }
        
        if (xiso && (args.Length == 1))
        {
            Console.WriteLine("Error. You need to specify a Videopartition file to convert xiso to redump.");
            return;
        }
        if (args.Length == 2)
        {
            videofile = Path.GetFileName(args[1]);
            if (xiso && !Array.Exists(VP_LENGTH, element => element == new System.IO.FileInfo(videofile).Length))
            {
                Console.WriteLine("Error. Invalid filesize of video partition file.");
                return;
            }
        }
        
        string label = xiso ? "redump" : "xiso";
        string outfile = infile.Substring(0, infile.LastIndexOf(".") + 1) + label + ".iso";
        //Console.WriteLine(outfile);
        //return;
        
        if (!xiso)  //handle video partition
        {
            if (videofile.Length > 0)
            {
                using (BinaryReader br = new BinaryReader(new FileStream(infile, FileMode.Open)))
                {
                    using (BinaryWriter bw = new BinaryWriter(new FileStream(videofile, FileMode.OpenOrCreate)))
                    {
                        while (br.BaseStream.Position < VP_LENGTH[vpidx])
                        {
                            byte[] tempBuffer = br.ReadBytes(2048);
                            bw.Write(tempBuffer);
                        }
                    }
                }
            }
        }
        
        else
        {
            using (BinaryReader br = new BinaryReader(new FileStream(videofile, FileMode.Open)))
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream(outfile, FileMode.OpenOrCreate)))
                {
                    while (br.BaseStream.Position != br.BaseStream.Length)
                    {
                        byte[] tempBuffer = br.ReadBytes(2048);
                        bw.Write(tempBuffer);
                    }
                }
            }
        }
        
        //handle game partition
        using (BinaryReader br = new BinaryReader(new FileStream(infile, FileMode.Open)))
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(outfile, FileMode.OpenOrCreate)))
            {
                if (!xiso)
                {
                    br.BaseStream.Position = VP_LENGTH[vpidx];
                }
                else
                {
                    bw.BaseStream.Position = VP_LENGTH[vpidx];
                }
                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    byte[] tempBuffer = br.ReadBytes(2048);
                    bw.Write(tempBuffer);
                }
            }
        }
    }
}
