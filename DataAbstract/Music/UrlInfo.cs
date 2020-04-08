namespace DataAbstract
{
    public class UrlInfo
    {
        public UrlInfo(DataSource dataSource)
        {
            this.DataSource = dataSource;
        }
        public string Url { get; set; }

        public DataSource DataSource { get; set; }
    }
}
