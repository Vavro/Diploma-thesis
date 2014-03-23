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
        static void Main(string[] args)
        {
            var source = "json-ld";
            var target = "n3";
            var convertUrl = String.Format(@"http://rdf-translator.appspot.com/convert/{0}/{1}/content", source,
                target);

            var ingredientsDirName = @"..\..\..\..\Ingredients";

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(convertUrl);

                var ingredientsDir = new DirectoryInfo(ingredientsDirName);
                var ingredientsFiles = ingredientsDir.GetFiles("*.json");
                var outputFileDir = ingredientsDirName + ".n3";
                if (Directory.Exists(outputFileDir))
                {
                    Directory.Delete(outputFileDir, true);
                }
                Directory.CreateDirectory(outputFileDir);

                foreach (var inputFile in ingredientsFiles)
                {
                    var outputFileName = inputFile.Name + ".n3";
                    var fullOutputFileName = Path.Combine(outputFileDir, outputFileName);

                    CreateConvertedFile(inputFile, client, convertUrl, fullOutputFileName);
                }
            }

            Console.ReadLine();
        }

        private static void CreateConvertedFile(FileInfo inputFile, HttpClient client, string convertUrl,
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
