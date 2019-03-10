using System;
using ImportAnything.Services.Interfaces;
using System.Linq;

namespace ImportAnything.Services.Transformers
{
    public static class ArrayTransformer
    {
        public static ITransformer TryMakeArrayTransformer(ITransformer individualTransformer)
        {
            var types = individualTransformer.GetTransformerTypes();
            if (types.Item1.IsArray || types.Item2.IsArray)
                return null;

            var type = typeof(ArrayTransformer<,>).MakeGenericType(types.Item1, types.Item2);
            return (ITransformer)Activator.CreateInstance(type, individualTransformer);
        }
    }

    public class ArrayTransformer<TFrom,TTo> : Transformer<TFrom[],TTo[]>
    {
        private ITransformer<TFrom, TTo> _individualTransformer;

        public override string ToString()
        {
            return "Array of " + _individualTransformer.ToString();
        }

        public ArrayTransformer(ITransformer<TFrom,TTo> individualTransformer)
        {
            _individualTransformer = individualTransformer;
        }

        protected override TTo[] Transform(TFrom[] source)
        {
            return source.Select(src => _individualTransformer.TryTransform(src))
                .Where(p=>p.Item2)
                .Select(p=>p.Item1)
                .ToArray();
        }

        protected override bool CanTransform(TFrom[] source)
        {
            return source.Any(p => _individualTransformer.CanTransform(p));
        }

        protected override bool ValidateResults(TTo[] results)
        {
            return results.Any();
        }
    }
}
