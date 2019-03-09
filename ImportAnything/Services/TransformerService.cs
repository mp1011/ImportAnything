using ImportAnything.Models;
using ImportAnything.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImportAnything.Services
{
    public class TransformerService
    {
        private TransformerGraph[] _graphs;

        public TransformerService(params ITransformer[] transformers)
        {
            _graphs = new TransformerGraphService().BuildGraph(transformers).ToArray();
        }

        public TTo Transform<TFrom, TTo>(TFrom source)
        {
            TTo ret = default(TTo);
            bool foundPath = false;

            foreach (var node in _graphs.Where(p => p.Transformer.CanTransform<TFrom>()))
            {
                var paths = node.FindPaths<TTo>().ToArray();

                if (paths.Any())
                {
                    if (foundPath || paths.Count() > 1)
                        throw new Exception($"More than one transformation path was found between {typeof(TFrom).Name} and {typeof(TTo).Name}");

                    foundPath = true;
                    ret = Transform<TFrom, TTo>(source, paths.First());
                }
            }

            if (!foundPath)
                throw new Exception($"No transformation path was found between {typeof(TFrom).Name} and {typeof(TTo).Name}");

            return ret;

        }

        public TTo Transform<TFrom, TTo>(TFrom source, IEnumerable<ITransformer> transformationsToApply)
        {
            object model = source;

            foreach (var transformer in transformationsToApply)
            {
                model = transformer.TransformObject(model);
            }

            return (TTo)model;
        }

      
    }
}
