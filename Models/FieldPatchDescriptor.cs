using System;
using System.Collections.Generic;
using System.Text;

namespace QueryEditor.Models
{
    public class FieldPatchDescriptor
    {
        /// <summary>
        /// Gets or sets the path to the field.
        /// </summary>
        public string FieldPath { get; set; }

        /// <summary>
        /// Gets or sets the palue to be patched.
        /// </summary>
        public object PatchValue { get; set; }

        /// <summary>
        /// Gets or sets the type of patch to be applied.
        /// </summary>
        public FieldPatchTypes FieldPatchType { get; set; }

        /// <summary>
        /// Gets or sets the selector value to be used to identify the child object to be updated.
        /// </summary>
        public FieldSelector FieldSelector { get; set; }

        public FieldPatchDescriptor() { }

        public FieldPatchDescriptor(string fieldPath, object patchValue, FieldPatchTypes patchType, FieldSelector selector)
        {
            this.FieldPath = fieldPath;
            this.PatchValue = patchValue;
            this.FieldPatchType = patchType;
            this.FieldSelector = selector;
        }
    }

}
