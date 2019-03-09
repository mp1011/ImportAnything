using System;
using ImportAnything.Services.Interfaces;

namespace ImportAnything.Services.Transformers
{
    public class GenericTransformer<TFrom, TTo> : ITransformer<TFrom, TTo>
    {
        public Func<TFrom, TTo> _doTransform;

        public GenericTransformer(Func<TFrom, TTo> doTransform)
        {
            _doTransform = doTransform;
        }

        public TTo Transform(TFrom source)
        {
            return _doTransform(source);
        }

        object ITransformer.TransformObject(object source)
        {
            if (source is TFrom)
                return Transform((TFrom)source);
            else
                throw new InvalidCastException($"Source cannot be cast to type {typeof(TFrom).Name}");
        }
    }
}
