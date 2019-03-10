using ImportAnything.Models;
using ImportAnything.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportAnything.Services.Transformers
{
    /// <summary>
    /// Reads a text file containing lines with comma separated data. Commas inside of quoted strings are treated as characters
    /// and not delimiters.
    /// </summary>
    public class FileToCSV : Transformer<FileInfo, CSV>
    {
        protected virtual string[] GetImpliedHeader(FileInfo src)
        {
            return null;
        }

        protected virtual IEnumerable<string> GetImpliedColumns(FileInfo src)
        {
            return new string[] { };
        }

        protected override CSV Transform(FileInfo file)
        {
            int expectedRowLength = 0;

            var rows = new List<string[]>();

            var impliedHeaders = GetImpliedHeader(file);
            if(impliedHeaders != null && impliedHeaders.Any())
                rows.Add(impliedHeaders);

            var currentRow = new List<String>();
            var text = File.ReadAllText(file.FullName);
            bool inQuote = false;

            List<char> currentWord = new List<char>();

            text += Environment.NewLine; //makes parsing the last line easier

            foreach (var c in text)
            {
                if (c == '\r' && !inQuote)
                {
                    currentRow.Add(new String(currentWord.ToArray()).Trim());
                    currentWord.Clear();

                    if (expectedRowLength == 0)
                        expectedRowLength = currentRow.Count;

                    if (currentRow.Count() > 1)
                    {
                        if (expectedRowLength != currentRow.Count)
                            throw new Exception("Error reading row");

                        currentRow.AddRange(GetImpliedColumns(file));
                        rows.Add(currentRow.ToArray());
                    }

                    currentRow.Clear();
                }
                else if (c == '\n')
                {
                    if (currentRow.Any())
                    {
                        currentRow.Add(new String(currentWord.ToArray()).Trim());
                        currentWord.Clear();

                        if (expectedRowLength == 0)
                            expectedRowLength = currentRow.Count;

                        if (expectedRowLength != currentRow.Count)
                            throw new Exception("Error reading row");

                        currentRow.AddRange(GetImpliedColumns(file));
                        rows.Add(currentRow.ToArray());
                        currentRow.Clear();
                    }
                }
                if (c == ',' && !inQuote)
                {
                    currentRow.Add(new String(currentWord.ToArray()).Trim());
                    currentWord.Clear();
                }
                else if (c == '"')
                {
                    inQuote = !inQuote;
                }
                else
                {
                    currentWord.Add(c);
                }
            }

            return new CSV(rows.ToList());
        }    
    }
}
