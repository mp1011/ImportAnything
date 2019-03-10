using ImportAnything.Services.Interfaces;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace ImportAnything.Models
{
    public class TransformerPath : IEnumerable<ITransformer>
    {
        private LinkedList<ITransformer> _list = new LinkedList<ITransformer>();

        public TransformerPath(ITransformer start)
        {
            _list.AddFirst(start);
        }

        public void AddFirst(ITransformer transformer)
        {
            _list.AddFirst(transformer);
        }

        IEnumerator<ITransformer> IEnumerable<ITransformer>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_list.First().ToString());

            foreach(var item in _list.Skip(1))
            {
                sb.Append(item.DescribeOutput());
            }

            return sb.ToString();
        }
    }
}
