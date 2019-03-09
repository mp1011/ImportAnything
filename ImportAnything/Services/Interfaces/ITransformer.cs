using System;
using System.Linq;

namespace ImportAnything.Services.Interfaces
{
    public interface ITransformer
    {
        object TransformObject(object source);
    }

    public interface ITransformer<TFrom, TTo> : ITransformer
    {
        TTo Transform(TFrom source);
    }    

    public static class ITransformerExtensions
    {
        public static string Describe(this ITransformer transformer)
        {
            var types = transformer.GetTransformerTypes();
            return $"{types.Item1.Name} -> {types.Item2.Name}";
        }

        public static string DescribeOutput(this ITransformer transformer)
        {
            var types = transformer.GetTransformerTypes();
            return $" -> {types.Item2.Name}";
        }

        public static Tuple<Type,Type> GetTransformerTypes(this ITransformer transformer)
        {
            var genericArgs = transformer.GetType().GenericTypeArguments;

            return new Tuple<Type, Type>(
                item1: genericArgs.FirstOrDefault() ?? typeof(object),
                item2: genericArgs.LastOrDefault() ?? typeof(object));            
        }

        public static bool CanTransform<T>(this ITransformer transformer)
        {
            return transformer.GetTransformerTypes().Item1 == typeof(T);
        }

        public static bool CanTransformInto<T>(this ITransformer transformer)
        {
            return transformer.GetTransformerTypes().Item2 == typeof(T);
        }

        public static bool CanTransform<TFrom, TTo>(this ITransformer transformer)
        {
            var types = transformer.GetTransformerTypes();
            return types.Item1 == typeof(TFrom) && types.Item2 == typeof(TTo); //todo - subclasses

        }
    }
}
