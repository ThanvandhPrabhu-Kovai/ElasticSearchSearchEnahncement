using System;

namespace QueryEditor.Helpers
{
    [AttributeUsage(
       AttributeTargets.Class |
       AttributeTargets.Constructor |
       AttributeTargets.Field |
       AttributeTargets.Method |
       AttributeTargets.Property,
       AllowMultiple = true)]

    public class DeBugInfo : System.Attribute { }
}