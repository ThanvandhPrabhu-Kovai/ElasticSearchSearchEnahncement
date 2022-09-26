using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryEditor.Models
{
    public class FieldSelector
    {
        /// <summary>
        /// Gets or sets the path to the object.
        /// </summary>
        public string FieldPath { get; set; }

        /// <summary>
        /// Gets or sets the value to match.
        /// </summary>
        public object Selector { get; set; }

        public FieldSelector Child { get; set; }

        //public static Tuple<Dictionary<string, IEnumerable<FieldPatchDescriptor>>, List<string>> SeparatePatchDescriptorsBySelectors(
        //    IEnumerable<FieldPatchDescriptor> descriptors)
        //{
        //    var result = new Dictionary<string, IEnumerable<FieldPatchDescriptor>> { };
        //    var selectorNames = new List<string> { };

        //    descriptors.ToList().ForEach((descriptor) =>
        //    {
        //        var selector = descriptor.FieldSelector;
        //        do
        //        {
        //            if (result.ContainsKey(selector.FieldPath))
        //            {
        //                result[selector.FieldPath].ToList().Add(descriptor);
        //            }
        //            else
        //            {
        //                result[selector.FieldPath] = new List<FieldPatchDescriptor> { descriptor };
        //            }
        //            selector = selector.Child;
        //        } while (selector != null);
        //    });

        //    return Tuple.Create(result, selectorNames);
        //}
    }
}
