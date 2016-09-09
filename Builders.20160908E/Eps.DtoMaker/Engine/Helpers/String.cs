using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace EPS.DtoMaker.Engine.Helpers
{
    public static class StringExtentions
    {
        public static string UpdateNameIfItIsReservedWord(this string name)
        {
            CSharpCodeProvider cs = new CSharpCodeProvider();
            return cs.IsValidIdentifier(name) ? name : $"{name}_";
        }

        public static string MakeAbbreviationByCapitals(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                StringBuilder stringBuilder = new StringBuilder();
                bool isUpperCase = false;
                int charIndex = 0;
                int valueLength = value.Length;
                while (charIndex < valueLength)
                {
                    char currentChar = value[charIndex];
                    if (Char.IsUpper(currentChar))
                    {
                        if (!isUpperCase)
                        {
                            stringBuilder.Append(currentChar);
                            isUpperCase = true;
                        }
                    }
                    else
                        isUpperCase = false;
                    ++charIndex;
                }
                return stringBuilder.ToString();
            }
            return value;
        }

        public static string CapitalizeFirstLetter(this string value)
        {
            return !string.IsNullOrEmpty(value) ? Char.ToUpper(value[0]) + value.Substring(1) : value;
        }

        public static string LowercaseFirstLetter(this string value)
        {
            return !string.IsNullOrEmpty(value) ? Char.ToLower(value[0]) + value.Substring(1) : value;
        }

        public static string MakeIndentationByFirstNotEmptyLine(this string value)
        {
            string [] lines = Regex.Split(value, "\r\n|\r|\n");
            if (lines.Length > 0)
            {
                int firstLineNonSpaceCharIndex = 0;
                foreach (var line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        firstLineNonSpaceCharIndex = line.FindFirstNonSpaceCharIndex();
                        break;
                    }
                }
                StringBuilder newValue = new StringBuilder();
                foreach (var line in lines)
                {
                    int currentLineFirstNonChar = line.FindFirstNonSpaceCharIndex();
                    if (firstLineNonSpaceCharIndex <= currentLineFirstNonChar)
                        newValue.AppendLine(line.Substring(firstLineNonSpaceCharIndex));
                    else
                        newValue.AppendLine(line);
                }
                return newValue.ToString();
            }
            return value;
        }

        public static int FindFirstNonSpaceCharIndex(this string value)
        {
            for (int firstLineNonSpaceCharIndex = 0; firstLineNonSpaceCharIndex < value.Length; ++firstLineNonSpaceCharIndex)
            {
                if (!Char.IsWhiteSpace(value[firstLineNonSpaceCharIndex]))
                    return firstLineNonSpaceCharIndex;
            }
            return 0;
        }
    }
}
