namespace Core.FileSystemControls
{
    using System.IO;

    public class FileArchiver
    {
        public void CopyFile(string srcPath, string targetPath)
        {
            var inputstream = File.OpenRead(srcPath);
            var outputStream = File.Create(targetPath);
            inputstream.CopyTo(outputStream);
        }
    }
}
