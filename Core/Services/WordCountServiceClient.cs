namespace Core.Services
{
    public class WordCountServiceClient
    {
        public int GetWordCount(string line)
        {
            return line.Split(' ').Length;//mundane but effective
        }
    }
}
