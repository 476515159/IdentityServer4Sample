using System.Collections.Generic;

namespace Fate.Project.Domain.AggregatesModel
{
    public class ProjectProperty:ValueObject
    {

        public int ProjectId { get; set; }
        public string Key { get; set; }

        public string Text { get; set; }

        public string Value { get; set; }


        public ProjectProperty()
        { }

        public ProjectProperty(string key,string Text,string Value)
        {
            this.Key = key;
            this.Text = Text;
            this.Value = Value;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Key;
            yield return Text;
            yield return Value;
        }
    }
}