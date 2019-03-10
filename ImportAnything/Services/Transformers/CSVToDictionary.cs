using ImportAnything.Services.Interfaces;
using System.Collections.Generic;
using System;
using System.Linq;
using ImportAnything.Models;

namespace ImportAnything.Services.Transformers
{

    public class CSVToDictionary : Transformer<CSV,MappedCSV>
    {
        private Dictionary<string, string> _columnMappings = new Dictionary<string, string>();

        public void AddMapping(string from, string to)
        {
            _columnMappings[from] = to;
        }

        protected override MappedCSV Transform(CSV source)
        {
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

            var columnNames = source.First().ToArray();
            foreach(var row in source.Skip(1))
            {
                Dictionary<string, string> cells = new Dictionary<string, string>();
                int index = 0;
                foreach(var column in row)
                    cells[GetMappedColumnName(columnNames[index++])] = column;

                ret.Add(cells);
            }

            return new MappedCSV(ret);
        }

        private string GetMappedColumnName(string unmappedName)
        {
            string mappedName;
            if (_columnMappings.TryGetValue(unmappedName, out mappedName))
                return mappedName;
            else
                return unmappedName;
        }
    }
}
