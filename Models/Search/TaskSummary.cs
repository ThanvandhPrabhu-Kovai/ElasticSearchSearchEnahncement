using System.Collections.Generic;
using System.Linq;

namespace Kovai.Churn360.Customers.Core.Models
{
    public class TaskSummary : Dictionary<int, Dictionary<int, TaskCount>>
    {
        public TaskSummary()
        {
        }

        public TaskSummary(Dictionary<int, Dictionary<int, TaskCount>> summary)
            : base(summary)
        {
        }

        public int Total
        {
            get
            {
                return this.Values.Select(_ => _.Values.Select(__ => __.Count).Sum()).Sum();
            }
        }

        public int Overdue
        {
            get
            {
                return this.Values.Select(_ => _.Values.Select(__ => __.Count).Sum()).Sum();
            }
        }

        public int NotStarted { get; set; }

        public int InProgress { get; set; }

        public int Completed { get; set; }
    }
}