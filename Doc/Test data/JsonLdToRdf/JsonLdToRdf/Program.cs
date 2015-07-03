using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JsonLdToRdf
{
    class Program
    {
        private const string medicinalProductsDirName = @"..\..\..\..\MedicinalProducts";
        private const string ingredientsDirName = @"..\..\..\..\Ingredients";

        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(convertUrl);

                ConvertDir(ingredientsDirName, client);
                ConvertDir(medicinalProductsDirName, client);
            }

            Console.ReadLine();
        }

        private static void ConvertDir(string dirName, HttpClient client)
        {
            var inputFilesDir = new DirectoryInfo(dirName);
            var inputFiles = inputFilesDir.GetFiles("*.json");
            var outputFileDir = dirName + ".n3";
            if (Directory.Exists(outputFileDir))
            {
                Directory.Delete(outputFileDir, true);
            }
            Directory.CreateDirectory(outputFileDir);

            int inputFileCount = inputFiles.Length;
            Console.WriteLine("Converting directory {0} with {1} files ", dirName, inputFileCount);
            Console.WriteLine("Writing files to directory {0}", outputFileDir);

            for (var i = 0; i < inputFiles.Length; i++)
            {
                var inputFile = inputFiles[i];
                var outputFileName = Path.GetFileNameWithoutExtension(inputFile.Name) + ".n3";
                var fullOutputFileName = Path.Combine(outputFileDir, outputFileName);

                CreateConvertedFile(inputFile, client, fullOutputFileName);

                Console.WriteLine("Converted file {0}/{1} : {2}", i+1, inputFileCount, outputFileName);
            }

            Console.WriteLine("Converting directory {0} Complete", dirName);
        }

        private const string source = "json-ld";
        private const string target = "n3";
        private const string convertUrl = @"http://rdf-translator.appspot.com/convert/" + source + "/" + target + "/content";

        private static void CreateConvertedFile(FileInfo inputFile, HttpClient client,
            string outputFileName)
        {
            using (var reader = new StreamReader(inputFile.FullName))
            {
                var content = new StringContent(reader.ReadToEnd());

                var c = new MultipartFormDataContent();
                c.Add(content, "content");

                var result = client.PostAsync(convertUrl, c).Result;
                var resultContent = result.Content.ReadAsStringAsync().Result;

                using (var writer = new StreamWriter(outputFileName, false))
                {
                    writer.WriteLine(resultContent);
                    writer.Flush();
                }
            }
        }
    }
}
