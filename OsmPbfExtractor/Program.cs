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
            string sourcePath = Console.ReadLine();
            if (!File.Exists(sourcePath))
            {
                Console.Error.WriteLine("File not found!");
                return;
            }

            Console.Write("Target file: ");
            string targetPath = Console.ReadLine();

            Console.Write("Northeast point [lat,lng]: ");
            float[] northeast = Console.ReadLine().Split(',').Select(s => float.Parse(s)).ToArray();
            if (northeast.Length < 2)
            {
                Console.Error.WriteLine("Invalid input!");
                return;
            }

            Console.Write("Southwest point [lat,lng]: ");
            float[] southwest = Console.ReadLine().Split(',').Select(s => float.Parse(s)).ToArray();
            if (southwest.Length < 2)
            {
                Console.Error.WriteLine("Invalid input!");
                return;
            }

            Console.Write("Openning target...");
            FileInfo fileInfo = new FileInfo(targetPath);
            fileInfo.Directory.Create();
            FileStream targetStream = fileInfo.OpenWrite();
            OsmStreamTarget osmTarget = new PBFOsmStreamTarget(targetStream, true);
            Console.WriteLine("OK");

            Console.Write("Openning source...");
            using (FileStream sourceStream = new FileInfo(sourcePath).OpenRead())
            {
                OsmStreamSource osmSource = new PBFOsmStreamSource(sourceStream).FilterBox(
                    northeast[1], northeast[0], southwest[1], southwest[0]);
                Console.WriteLine("OK");

                long i = 0;
                foreach (OsmGeo geo in osmSource)
                {
                    switch (geo.Type)
                    {
                        case OsmGeoType.Node:
                            osmTarget.AddNode((Node)geo);
                            i++;
                            break;
                        case OsmGeoType.Way:
                            osmTarget.AddWay((Way)geo);
                            i++;
                            break;
                        case OsmGeoType.Relation:
                            osmTarget.AddRelation((Relation)geo);
                            i++;
                            break;
                    }
                    if (i % 100 == 0)
                    {
                        Console.Write("\r{0:N0} items added", i);
                    }
                }

                Console.WriteLine("\r{0:N0} items added", i);
                sourceStream.Close();
            }

            Console.Write("Flushing...");
            osmTarget.Flush();
            Console.WriteLine("OK");

            targetStream.Close();

            Console.WriteLine("Finish!");
        }
    }
}
