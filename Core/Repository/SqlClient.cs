namespace Core.Repository
{
    using System.Data.SqlClient;
    using System.Data;
    using System.Configuration;

    public class SqlClient
    {
        public void CreateFeedResults(string path, int lineCount, int wordCount, long processingTime)
        {
            InitializeConnectionCommand(string.Format("{0}{1}{2}{3}", path, lineCount, wordCount, processingTime));
        }

        public void UpdateFeedResults(string path, int lineCount, int wordCount, long processingTime)
        {
            InitializeConnectionCommand(string.Format("{0}{1}{2}{3}", path, lineCount, wordCount, processingTime));
        }

        private void InitializeConnectionCommand(string commandText)
        {
            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Test"].ToString());
            sqlConnection.Open();
            var command = sqlConnection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }

        public void SaveLineResults(string path, int lineNumber, int wordCount, string excerpt)
        {
            throw new System.NotImplementedException();
        }
    }
}
