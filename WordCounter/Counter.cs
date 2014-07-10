namespace WordCounter
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Core.FileSystemControls;
    using System;
    using Core.Repository;
    using System.Threading.Tasks;

    public class Counter
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("'s' stars the application; 'gc' initiates Garbage Collection; 'x' exits the application");
            var command = "";
            while (command != "x")
            {
                command = Console.ReadLine();
                
            }
        }

        public static void DetermineCommandProcess(string command)
        {
            if (command == "s")
            {
                Start();
            }
            else if (command == "gc")
            {
                GC.Collect();
            }
        }

        private static void Start()
        {
            var archiver = new FolderWatcher();
            archiver.Start("test", "*.txt", ProcessFile);
        }

        private static void ProcessFile(string path)
        {
            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine("Processing file: " + path);
            var archivePath = string.Format(@"c:\documents\{0}.txt", Guid.NewGuid());
            Console.WriteLine("Archiving to: " + archivePath);
            new FileArchiver().CopyFile(path, archivePath);

            Console.WriteLine("Initializing db results");
            var sqlClient = new SqlClient();
            sqlClient.CreateFeedResults(path, 0, 0, 0);

            var cancellationTokenSource = new CancellationTokenSource();

            var lines = File.ReadAllLines(path);
            var wordCounts = new ConcurrentBag<int>();
            var apiTasks = new List<Task>();
            for (var i = 0; i < lines.Length; i++)
            {
                var lineNumber = i;
                var line = lines[i];
                apiTasks.Add(Task.Factory.StartNew(() => GetWordCount(path, lineNumber, line, wordCounts, cancellationTokenSource)));
            }
            try
            {
                Task.WaitAll(apiTasks.ToArray(), cancellationTokenSource.Token);
                var wordCount = wordCounts.Sum();
                Console.WriteLine("Saving results to the DB");
                sqlClient.UpdateFeedResults(path, lines.Length, wordCount, stopwatch.ElapsedMilliseconds);
                Console.WriteLine("Complete, took: {0}ms", stopwatch.ElapsedMilliseconds);
                File.Delete(path);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Transaction has failed.");
            }
        }

        private static void GetWordCount(string path, int lineNumber, string line, 
            ConcurrentBag<int> wordCounts, CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                var excerpt = line.Length > 100 ? line.Substring(0, 100) : line;
                Console.WriteLine("Processing line: {0}, {1}...", lineNumber, excerpt);
                var apiClient = new ApiClient();
                var wordCount = apiClient.GetWordCount(line);
                var sqlClient = new SqlClient();
                sqlClient.SaveLineResults(path, lineNumber, wordCount, excerpt);
                wordCounts.Add(apiClient.GetWordCount(line));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                cancellationTokenSource.Cancel();
            }
        }
    }
}
