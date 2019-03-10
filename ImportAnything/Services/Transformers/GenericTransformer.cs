using System;
using ImportAnything.Services.Interfaces;

namespace ImportAnything.Services.Transformers
{
    public class GenericTransformer<TFrom, TTo> : Transformer<TFrom, TTo>
    {
        private Func<TFrom, TTo> _doTransform;
        private Func<TFrom, bool> _canTransform;


        public GenericTransformer(Func<TFrom, TTo> doTransform, Func<TFrom, bool> canTransform=null)
        {
            _doTransform = doTransform;
            _canTransform = canTransform ?? (p=>true);
        }


        protected override bool CanTransform(TFrom source)
        {
            return _canTransform(source);
        }

        protected override TTo Transform(TFrom source)
        {
            return _doTransform(source);
        }
    }
}
