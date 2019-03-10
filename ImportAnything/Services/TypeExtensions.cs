using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportAnything.Services
{
    public static class TypeExtensions
    {
        public static string Describe(this Type t)
        {
            StringBuilder sb = new StringBuilder();

            if (t.IsGenericType)
            {
                sb.Append(t.Name.Split('`')[0]);
                sb.Append("<");
                sb.Append(String.Join(",", t.GenericTypeArguments.Select(p => p.Describe()).ToArray()));
                sb.Append(">");
            }
            else if (t.IsArray)
                sb.Append(t.GetElementType().Describe()).Append("[]");
            else
                sb.Append(t.Name);

            return sb.ToString();
        }
    }
}
