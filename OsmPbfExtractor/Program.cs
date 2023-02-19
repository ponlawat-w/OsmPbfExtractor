using OsmSharp;
using OsmSharp.Streams;
using System;
using System.IO;
using System.Linq;

namespace OsmPbfExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Source file: ");
            string sourcePath = (args.Length > 0 ? args[0] : Console.ReadLine()).Trim();
            if (args.Length > 0)
            {
                Console.WriteLine(args[0]);
            }
            if (!File.Exists(sourcePath))
            {
                Console.Error.WriteLine("File not found!");
                return;
            }

            Console.Write("Target file: ");
            string targetPath = (args.Length > 1 ? args[1] : Console.ReadLine()).Trim();
            if (args.Length > 1)
            {
                Console.WriteLine(args[1]);
            }

            Console.Write("First corner point [lat,lng]: ");
            float[] firstPoint = (args.Length > 2 ? args[2] : Console.ReadLine()).Split(',').Select(s => float.Parse(s.Trim())).ToArray();
            if (args.Length > 2)
            {
                Console.WriteLine(args[2]);
            }
            if (firstPoint.Length < 2)
            {
                Console.Error.WriteLine("Invalid input!");
                return;
            }

            Console.Write("Second corner point [lat,lng]: ");
            float[] secondPoint = (args.Length > 3 ? args[3] : Console.ReadLine()).Split(',').Select(s => float.Parse(s.Trim())).ToArray();
            if (args.Length > 3)
            {
                Console.WriteLine(args[3]);
            }
            if (secondPoint.Length < 2)
            {
                Console.Error.WriteLine("Invalid input!");
                return;
            }

            Console.Write("Openning source...");
            using (FileStream sourceStream = new FileInfo(sourcePath).OpenRead())
            {
                float left = Math.Min(firstPoint[1], secondPoint[1]);
                float top = Math.Max(firstPoint[0], secondPoint[0]);
                float right = Math.Max(firstPoint[1], secondPoint[1]);
                float bottom = Math.Min(firstPoint[0], secondPoint[0]);
                OsmStreamSource osmSource = new PBFOsmStreamSource(sourceStream).FilterBox(left, top, right, bottom, true);
                Console.WriteLine("OK");

                Console.Write("Openning target...");
                using (FileStream targetStream = new FileInfo(targetPath).Open(FileMode.Create, FileAccess.ReadWrite))
                {
                    PBFOsmStreamTarget osmTarget = new PBFOsmStreamTarget(targetStream);
                    osmTarget.RegisterSource(osmSource);
                    Console.WriteLine("OK");

                    Console.Write("Pulling to target...");
                    osmTarget.Pull();
                    Console.WriteLine("OK");
                }
            }

            Console.WriteLine("Finish!");
        }
    }
}
