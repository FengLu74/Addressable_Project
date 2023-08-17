using System;
namespace LitJson
{
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Field| AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}