using ImportAnything.Models;
using ImportAnything.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImportAnything.Services
{
    public class TransformerGraphService
    {
        public IEnumerable<TransformerGraph> BuildGraph(params ITransformer[] transformers)
        {
            List<TransformerGraph> nodes = new List<TransformerGraph>();
            nodes.AddRange(transformers.Select(t => new TransformerGraph(t)));

            foreach (var node in nodes)
                node.AddEligibleNext(nodes);

            return nodes;
        }
        
    }    
}
