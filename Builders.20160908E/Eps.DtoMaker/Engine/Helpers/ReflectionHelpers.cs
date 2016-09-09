using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS.DtoMaker.Engine.Helpers
{
    public static class ReflectionHelpers
    {
        public static string NamespaceFromQName(this string fullyQualifiedName)
        {
            string value = fullyQualifiedName;
            int firstAngleIndex = value.IndexOf('<');
            if (firstAngleIndex < 0)
                firstAngleIndex = value.Length - 1;
            int lastDotIndex = value.LastIndexOf('.', firstAngleIndex);
            return lastDotIndex < 0 ? value : value.Substring(0, lastDotIndex);
        }

        public static string NameFromQName(this string fullyQualifiedName)
        {
            string value = fullyQualifiedName;
            int firstAngleIndex = value.IndexOf('<');
            if (firstAngleIndex < 0)
                firstAngleIndex = value.Length - 1;
            int lastDotIndex = value.LastIndexOf('.', firstAngleIndex);
            return lastDotIndex < 0 ? value : ((lastDotIndex + 1) < value.Length ? value.Substring(lastDotIndex + 1) : string.Empty);
        }
    }

    public class NamespaceTracker
    {
        private readonly SortedList<string, string> _namespaces = new SortedList<string, string>();

        public NamespaceTracker()
        {
        }

        public NamespaceTracker(NamespaceTracker sourceNamespaceTracker)
        {
            foreach (string facedNamespace in sourceNamespaceTracker.FacedNamespaces)
                _namespaces.Add(facedNamespace, facedNamespace);
        }

        public NamespaceTracker(IEnumerable<NamespaceTracker> sourceNamespaceTrackers)
        {
            foreach (var sourceNamespaceTracker in sourceNamespaceTrackers)
            {
                foreach (string facedNamespace in sourceNamespaceTracker.FacedNamespaces)
                    _namespaces.Add(facedNamespace, facedNamespace);
            }
        }

        public IEnumerable FacedNamespaces => _namespaces.Select(kv => kv.Value);

        public string SimplifyName(string typeName, bool simplifyGenericPart)
        {
            TypeName parsedTypeName = TypeName.Parse(typeName);
            ProcessAllNames(parsedTypeName);
            string simplifiedName = parsedTypeName.ToString();
            return simplifiedName;
        }

        private void ProcessAllNames(TypeName typeName)
        {
            if (IsQualifiedName(typeName.Name))
            {
                AddNamespace(typeName.Name.NamespaceFromQName());
                typeName.Name = typeName.Name.NameFromQName();
            }
            foreach (var typeArgument in typeName.TypeArguments)
            {
                string name = typeArgument.Name;
                if (IsQualifiedName(name))
                {
                    AddNamespace(name.NamespaceFromQName());
                    typeArgument.Name = name.NameFromQName();
                }
                ProcessAllNames(typeArgument);
            }
        }

        private bool IsQualifiedName(string name)
        {
            return name.IndexOf('.') >= 0;
        }

        public void AddNamespace(string @namespace)
        {
            if (!_namespaces.ContainsKey(@namespace))
                _namespaces.Add(@namespace, @namespace);
        }

        public string SimplifyName_(string typeName, bool simplifyGenericPart)
        {
            string value = typeName;
            if (simplifyGenericPart)
            {
                List<string> angledComplesParts = new List<string>();
                value = ExtractGenericParts(value, angledComplesParts);

                List<string> angledSimplifiedParts = new List<string>();
                foreach (var angledComplexPart in angledComplesParts)
                {
                    string namespaceName = angledComplexPart.NamespaceFromQName();
                    _namespaces[namespaceName] = namespaceName;
                    angledSimplifiedParts.Add(angledComplexPart.NameFromQName());
                }

                angledSimplifiedParts.Reverse();
                foreach (var angledSimplifiedPart in angledSimplifiedParts)
                {
                    value = value + $"<{angledSimplifiedPart}";
                }
                value = value + new string('>', angledSimplifiedParts.Count);
            }
            string typeNamespace = value.NamespaceFromQName();
            if (!string.IsNullOrEmpty(typeNamespace))
                _namespaces[typeNamespace] = typeNamespace;
            return value.NameFromQName();
        }

        private string ExtractGenericParts(string value, List<string> genericParts)
        {
            while (true)
            {
                int index1 = value.LastIndexOf('<');
                int index2 = value.IndexOf('>', index1 + 1);
                if (index1 < 0 && index2 < 0)
                    break;
                if (index1 >= 0 && index2 >= 0)
                {
                    string angledPart = value.Substring(index1 + 1, index2 - index1 - 1);
                    string [] angledPartParts = angledPart.Split(',');
                    genericParts.Add(angledPart.Trim());
                    value = value.Substring(0, index1) + (index2 + 1 < value.Length ? value.Substring(index2 + 1) : string.Empty);
                }
                else
                    throw new Exception($"Bad angles balance in '{value}'");
            }
            return value;
        }

        public void AddNamespaces(IEnumerable<string> namespaces)
        {
            foreach (var @namespace in namespaces)
                _namespaces[@namespace] = @namespace;
        }
    }
}
