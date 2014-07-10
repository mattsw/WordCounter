namespace Core.FileSystemControls
{
    using System;
    using System.IO;
    using System.Threading;

    public class FolderWatcher
    {

        public void Start(string path, string fileExtension, Action<string> action)
        {
            var fsw = new FileSystemWatcher(path, fileExtension);
            fsw.Created += (x, y) =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("New file created: " + y.Name);
                    action(y.FullPath);
                };
            fsw.EnableRaisingEvents = true;
        }
    }
}
