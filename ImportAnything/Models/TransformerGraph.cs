using ImportAnything.Services;
using ImportAnything.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImportAnything.Models
{
    public class TransformerGraph
    {
        public ITransformer Transformer { get; }

        public List<TransformerGraph> NextNodes { get; } = new List<TransformerGraph>();

        public TransformerGraph(ITransformer transformer)
        {
            Transformer = transformer;
        }

        public override string ToString()
        {
            return Transformer.ToString();
        }

        /// <summary>
        /// Adds any nodes from the given collection who's input type matches this node's output type
        /// </summary>
        /// <param name="nodes"></param>
        public void AddEligibleNext(IEnumerable<TransformerGraph> nodes)
        {
            var outputType = Transformer.GetTransformerTypes().Item2;
            NextNodes.AddRange(nodes.Where(p => p.Transformer.GetTransformerTypes().Item1 == outputType));
        }

        public IEnumerable<TransformerPath> FindPaths<TTo>(List<TransformerGraph> exclusions=null)
        {
            if (exclusions == null)
                exclusions = new List<TransformerGraph>();

            exclusions.Add(this);

            if (Transformer.CanTransformInto<TTo>())
                return new TransformerPath[] { new TransformerPath(Transformer) };

            var paths = NextNodes
                .Where(node=> ! exclusions.Contains(node))
                .SelectMany(p => p.FindPaths<TTo>(exclusions.ToList()))
                .ToArray();

            foreach (var path in paths)
                path.AddFirst(Transformer);

            return paths;
        }

      
    }
}

