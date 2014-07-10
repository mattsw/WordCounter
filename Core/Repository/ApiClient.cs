namespace Core.Repository
{
    using Services;

    public class ApiClient
    {
        public int GetWordCount(string line)
        {
            var client = new WordCountServiceClient();
            return client.GetWordCount(line);
        }
    }
}
