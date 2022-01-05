
using System;
using System.IO;

class Redump2xiso
{
    static void Main(string[] args)
    {
        long REDUMP_ISO_LENGTH = 0x1D26A8000;
        long VP_LENGTH = 0x18300000;
        long XISO_LENGTH = 0x1BA3A8000;
        if ((args.Length == 0) || (args.Length > 2))
        {
            Console.WriteLine("Usage: redump2xiso.exe <isofile> [<videopartitionfile>]");
            return;
        }
        string infile = Path.GetFileName(args[0]);
        string videofile = "";
        if ((new System.IO.FileInfo(infile).Length != REDUMP_ISO_LENGTH) && (new System.IO.FileInfo(infile).Length != XISO_LENGTH))
        {
            Console.WriteLine("Error. Invalid filesize: " + infile);
            return;
        }
        bool xiso = (new System.IO.FileInfo(infile).Length == XISO_LENGTH);
        if (xiso && (args.Length == 1))
        {
            Console.WriteLine("Error. You need to specify a Videopartition file to convert xiso to redump.");
            return;
        }
        if (args.Length == 2)
        {
            videofile = Path.GetFileName(args[1]);
            if (xiso && (new System.IO.FileInfo(videofile).Length != VP_LENGTH))
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
                        while (br.BaseStream.Position < VP_LENGTH)
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
                    br.BaseStream.Position = VP_LENGTH;
                }
                else
                {
                    bw.BaseStream.Position = VP_LENGTH;
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
