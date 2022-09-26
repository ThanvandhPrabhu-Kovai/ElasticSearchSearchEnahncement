using System;
using System.Collections.Generic;
using System.Text;

namespace QueryEditor.Models
{
    public class Editable
    {
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
