using System;
using System.Linq;

namespace ImportAnything.Services.Interfaces
{
    public interface ITransformer
    {
        object TryTransformObject(object source);
        Tuple<Type, Type> GetTransformerTypes();
        bool CanTransform<T>();
        bool CanTransformInto<T>();
        bool CanTransform<A, B>();
        string DescribeOutput();
        bool CanTransform<T>(T source);
    }

    public interface ITransformer<TFrom, TTo> : ITransformer
    {
        Tuple<TTo, bool> TryTransform(TFrom source);

        bool CanTransform(TFrom source);
    }    

    public abstract class Transformer<TFrom, TTo> : ITransformer<TFrom, TTo>
    {
        private Tuple<Type, Type> _transformerTypes;

        protected Transformer()
        {
            _transformerTypes = GetTransformerTypes();
        }

        public override string ToString()
        {
            var types = GetTransformerTypes();
            return $"{types.Item1.Describe()} -> {types.Item2.Describe()}";
        }

        public string DescribeOutput()
        {
            var types = GetTransformerTypes();
            return $" -> {types.Item2.Describe()}";
        }

        public Tuple<Type, Type> GetTransformerTypes()
        {
            if (_transformerTypes != null)
                return _transformerTypes;

            foreach (var iTransformerInterfaceType in GetType().GetInterfaces()
                .Where(p => typeof(ITransformer).IsAssignableFrom(p)))
            {
                var genericArgs = iTransformerInterfaceType.GenericTypeArguments;
                if (genericArgs.Length == 2)
                    return new Tuple<Type, Type>(genericArgs[0], genericArgs[1]);
            }

            return new Tuple<Type, Type>(typeof(object), typeof(object));
        }

        public bool CanTransform<T>()
        {
            return GetTransformerTypes().Item1 == typeof(T);
        }

        public bool CanTransformInto<T>()
        {
            return GetTransformerTypes().Item2 == typeof(T);
        }

        public bool CanTransform<A,B>()
        {
            var types = GetTransformerTypes();
            return types.Item1 == typeof(A) && types.Item2 == typeof(B); //todo - subclasses
        }

        public Tuple<TTo,bool> TryTransform(TFrom source)
        {
            if (CanTransform(source))
            {
                var result = Transform(source);
                if (ValidateResults(result))
                    return new Tuple<TTo, bool>(result, true);
                else
                    return new Tuple<TTo, bool>(result, false);
            }
            else
                return new Tuple<TTo, bool>(default(TTo), false);
        }

        protected abstract TTo Transform(TFrom source);

        bool ITransformer.CanTransform<T>(T source)
        {
            return CanTransform<T>() && CanTransform((TFrom)((object)source));
        }

        bool ITransformer<TFrom, TTo>.CanTransform(TFrom source)
        {
            return CanTransform<TFrom>() && CanTransform(source);
        }

        protected virtual bool CanTransform(TFrom source)
        {
            return true;
        }

        protected virtual bool ValidateResults(TTo results)
        {
            return true;
        }

        object ITransformer.TryTransformObject(object source)
        {
            if (source is TFrom)
            {
                if (!CanTransform((TFrom)source))
                    return null;

                var result = TryTransform((TFrom)source);
                if (result.Item2)
                    return result.Item1;
                else
                    return null;
            }
            else
                throw new InvalidCastException($"Source cannot be cast to type {typeof(TFrom).Describe()}");
        }
    }
}
