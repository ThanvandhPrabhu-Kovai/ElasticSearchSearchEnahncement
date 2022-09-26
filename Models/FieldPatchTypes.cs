using System;
using System.Collections.Generic;
using System.Text;

namespace QueryEditor.Models
{
    public enum FieldPatchTypes
    {
        /// <summary>
        /// Append if target is an array else replace it.
        /// </summary>
        Append,

        /// <summary>
        /// Replace the existing value.
        /// </summary>
        ReplaceExistingValues,

        /// <summary>
        /// Remove the existing value.
        /// </summary>
        Remove,

        Increment,

        Decrement,
    }
}
