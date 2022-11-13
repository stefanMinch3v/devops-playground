namespace TaskTronic.Statistics.Models.Statistics
{
    using Data.Models;
    using TaskTronic.Models;

    public class OutputStatisticsServiceModel : IMapFrom<Statistics>
    {
        public int TotalFolders { get; set; }

        public int TotalFiles { get; set; }
    }
}
