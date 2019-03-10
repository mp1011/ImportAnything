using ImportAnything.Models;
using ImportAnything.Services.Consolidators;
using ImportAnything.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImportAnything.Services
{
    public class TransformerService
    {
        private TransformerGraph[] _graphs;
        private IConsolidator[] _consolidators;

        public TransformerService(params ITransformer[] transformers) :
            this(new TransformerGraphService().BuildGraph(transformers).ToArray(), null)
        {
        }

        public TransformerService(TransformerGraph[] graphs, IConsolidator[] consolidators)
        {
            _graphs = graphs;
            _consolidators = consolidators ?? new IConsolidator[] { };
        }

        public TTo Transform<TFrom, TTo>(TFrom source)
        {
            List<TTo> ret = new List<TTo>();

            foreach (var node in _graphs.Where(p => p.Transformer.CanTransform(source)))
            {
                var paths = node.FindPaths<TTo>().ToArray();

                var possibleResults = paths.Select(p => TryTransform<TFrom, TTo>(source, p))
                    .Where(p => p.Item2)
                    .Select(p => p.Item1)
                    .ToArray();

                foreach (var result in possibleResults)
                    ret.Add(result);
            }

            if (!ret.Any())
                throw new Exception($"No transformation path was found between {typeof(TFrom).Describe()} and {typeof(TTo).Describe()}");

            if (ret.Count > 1)
                return Consolidate(ret);
            else
                return ret[0];
        }

        private TTo Consolidate<TTo>(IEnumerable<TTo> entries)
        {
            var consolidator = _consolidators.OfType<IConsolidator<TTo>>().FirstOrDefault();
            
            //todo , can structuremap do this automatically? also this is a bad place for this logic
            if(consolidator == null && typeof(TTo).IsArray)
            {
                var consolidatorType = typeof(ArrayConsolidator<,>).MakeGenericType(typeof(TTo), typeof(TTo).GetElementType());
                consolidator = (IConsolidator<TTo>)Activator.CreateInstance(consolidatorType);
            }

            if (consolidator == null)
                throw new Exception($"Multiple transformation paths were found for the type {typeof(TTo).Name} and no IConsolidator interface exists that can combine them");
            else
                return consolidator.Consolidate(entries);
        }

        public Tuple<TTo,bool> TryTransform<TFrom, TTo>(TFrom source, IEnumerable<ITransformer> transformationsToApply)
        {
            object model = source;

            foreach (var transformer in transformationsToApply)
            {
                model = transformer.TryTransformObject(model);
                if (model == null)
                    return new Tuple<TTo, bool>(default(TTo), false);
            }

            return new Tuple<TTo, bool>((TTo)model, true);
        }

      
    }
}
