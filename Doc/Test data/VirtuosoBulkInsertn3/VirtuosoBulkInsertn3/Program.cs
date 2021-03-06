﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenLink.Data.Virtuoso;

namespace VirtuosoBulkInsertn3
{
    class Program
    {
        //path that is accessible to the virtuoso server
        const string IngredientsPath = @"d:\School\Diplomka\Instalatory\virtuoso-opensource\database\Ingredients.n3_uprava";
        const string MedicinalProductsPath = @"d:\School\Diplomka\Instalatory\virtuoso-opensource\database\MedicinalProducts.n3_uprava";

        static void Main(string[] args)
        {
            var connectionString = "Server=localhost;Uid=dba;pwd=dba";

            using (var connection = new VirtuosoConnection(connectionString))
            {
                Console.WriteLine("connecting");
                connection.Open();
                Console.WriteLine("connected");
                var graph = "http://localhost:8890/DAV";

                InsertN3FilesInPath(IngredientsPath, graph, connection);
                InsertN3FilesInPath(MedicinalProductsPath, graph, connection);
            }
        }

        private static void InsertN3FilesInPath(string path, string graph, VirtuosoConnection connection)
        {
            var inputDir = new DirectoryInfo(path);
            var inputFiles = inputDir.GetFiles("*.n3");

            int fileCount = inputFiles.Length;
            Console.WriteLine("Inserting files from dir {0}, count: {1}", inputDir, fileCount);

            for (int index = 0; index < fileCount; index++)
            {
                var inputFile = inputFiles[index];
                var result = InsertN3File(inputFile, graph, connection);

                Console.WriteLine("File - {0}/{1} - inserted - {2}", index, fileCount, inputFile.Name );

                Console.WriteLine(result);
            }
        }

        private static int InsertN3File(FileInfo inputFile, string graph, VirtuosoConnection connection)
        {
            var insertTripleCommandString =
                @"DB.DBA.TTLP_MT (file_to_string_output(@fileName), '', @graph)";

            var cmd = new VirtuosoCommand();
            cmd.CommandText = insertTripleCommandString;
            cmd.Parameters.Add("fileName", VirtDbType.VarChar);
            cmd.Parameters["fileName"].Value = inputFile.FullName;
            cmd.Parameters.Add("graph", VirtDbType.VarChar);
            cmd.Parameters["graph"].Value = graph;
            cmd.Connection = connection;
            var result = cmd.ExecuteNonQuery();
            return result;
        }
    }
}

