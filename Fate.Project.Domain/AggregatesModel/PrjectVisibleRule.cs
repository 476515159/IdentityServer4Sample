namespace Fate.Project.Domain.AggregatesModel
{
    public class PrjectVisibleRule:Entity
    {
        public int ProjectId { get; set; }

        public bool? Visible { get; set; }

        public string Tags { get; set; }
    }
}