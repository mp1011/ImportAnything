using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ImportAnything.Models
{
    public class CSV : IEnumerable<string[]>
    {
        private List<string[]> _lines;

        public CSV(List<string[]> lines)
        {
            _lines = lines;
        }

        IEnumerator<string[]> IEnumerable<string[]>.GetEnumerator()
        {
            return _lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _lines.GetEnumerator();
        }
    }

    public class MappedCSV : IEnumerable<Dictionary<string, string>>
    {
        private List<Dictionary<string, string>> _lines;

        public MappedCSV(List<Dictionary<string, string>> lines)
        {
            _lines = lines;
        }

        IEnumerator<Dictionary<string, string>> IEnumerable<Dictionary<string, string>>.GetEnumerator()
        {
            return _lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _lines.GetEnumerator();
        }
    }
}
