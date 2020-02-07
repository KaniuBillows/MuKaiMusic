namespace MuKai_Music.Model.ResponseEntity
{
    /// <summary>
    /// 爬取到的数据
    /// </summary>
    public abstract class ProcessedData
    {
        public ProcessedData(DataSource sourceType)
        {
            this.DataSource = sourceType;
        }

        public DataSource DataSource { get; set; }

        public int NetEaseId { get; set; }

        public int KuwoId { get; set; }

        public string MiguId { get; set; }
    }
}
