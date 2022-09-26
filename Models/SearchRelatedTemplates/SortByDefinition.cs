using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchSearchEnhancement.Models
{
    public class SortByDefinition
    {
        public SortByDefinition()
        {
        }

        public SortByDefinition(string name, SortType sortType)
        {
            this.Name = name;
            this.SortType = sortType;
        }

        public string Name { get; set; }

        public SortType SortType { get; set; }

        public ICollection<string> SortedValues { get; } = new List<string>();
    }

}
