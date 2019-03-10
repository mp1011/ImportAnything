using ImportAnything.Services;
using ImportAnything.Services.Transformers;
using ImportAnything.Tests.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace ImportAnything.Tests.Services
{
    [TestFixture]
    class TransformerTests
    {
        [TestCase]
        public void CanTransformObjectBetweenTwoTypes()
        {
            var model = new SampleClassA { Value = "100" };
            var transformer = new GenericTransformer<SampleClassA, SampleClassB>(a => new SampleClassB { Value = int.Parse(a.Value) });
            var transformed = transformer.TryTransform(model);
            Assert.AreEqual(100, transformed.Item1.Value);
        }

        [TestCase]
        public void CanTransformObjectBetweenThreeTypes()
        {
            var model = new SampleClassA { Value = "100" };
            var transformer1 = new GenericTransformer<SampleClassA, SampleClassB>(a => new SampleClassB { Value = int.Parse(a.Value) });
            var transformer2 = new GenericTransformer<SampleClassB, SampleClassC>(b => new SampleClassC { StringValue = b.Value.ToString(), IntValue = b.Value });

            var transformerService = new TransformerService(transformer1, transformer2);

            var model2 = transformerService.Transform<SampleClassA, SampleClassB>(model);
            var model3 = transformerService.Transform<SampleClassB, SampleClassC>(model2);

            Assert.AreEqual("100", model3.StringValue);
            Assert.AreEqual(100, model3.IntValue);
        }


        [TestCase]
        public void CanTransformObjectBetweenThreeTypesAtOnce()
        {
            var model = new SampleClassA { Value = "100" };
            var transformer1 = new GenericTransformer<SampleClassA, SampleClassB>(a => new SampleClassB { Value = int.Parse(a.Value) });
            var transformer2 = new GenericTransformer<SampleClassB, SampleClassC>(b => new SampleClassC { StringValue = b.Value.ToString(), IntValue = b.Value });

            var transformerService = new TransformerService(transformer1, transformer2);
            var transformed = transformerService.Transform<SampleClassA, SampleClassC>(model);

            Assert.AreEqual("100", transformed.StringValue);
            Assert.AreEqual(100, transformed.IntValue);
        }

        [TestCase]
        public void CanTransformMany()
        {
            List<SampleClassA> list = new List<SampleClassA>();
            list.Add(new SampleClassA { Value = "10" });
            list.Add(new SampleClassA { Value = "20" });

            var transformer = new GenericTransformer<SampleClassA, SampleClassB>(a => new SampleClassB { Value = int.Parse(a.Value) });
            var transformerService = new TransformerService(transformer);

            var result = transformerService.Transform<SampleClassA[], SampleClassB[]>(list.ToArray());
            Assert.AreEqual(20, result[1].Value);
        }

        [TestCase]
        public void CanDescribeTransformer()
        {
            var transformer = new GenericTransformer<List<SampleClassA>, SampleClassA[]>(a => a.ToArray());
            Assert.AreEqual("List<SampleClassA> -> SampleClassA[]", transformer.ToString());

            var transformer2 = new GenericTransformer<List<SampleClassA>, List<SampleClassA>[]>(a => null);
            Assert.AreEqual("List<SampleClassA> -> List<SampleClassA>[]", transformer2.ToString());

            var transformer3 = new GenericTransformer<Dictionary<string,SampleClassA>, List<SampleClassA>[]>(a => null);
            Assert.AreEqual("Dictionary<String,SampleClassA> -> List<SampleClassA>[]", transformer3.ToString());
        }

        [TestCase]
        public void CanHandleCircularGraph()
        {
            var transformer1 = new GenericTransformer<SampleClassA, SampleClassB>(a => new SampleClassB { Value = int.Parse(a.Value) });
            var transformer2 = new GenericTransformer<SampleClassB, SampleClassC>(b => new SampleClassC { StringValue = b.Value.ToString(), IntValue = b.Value });
            var transformer3 = new GenericTransformer<SampleClassC, SampleClassA>(c => new SampleClassA { Value = c.StringValue });

            var service = new TransformerService(transformer1, transformer2, transformer3);
            var model = new SampleClassA { Value = "100" };

            var circularTransform = service.Transform<SampleClassA, SampleClassA>(model);
            Assert.AreEqual("100", circularTransform.Value);
        }

        [TestCase]
        public void CanHandleBranchingGraphWithLoop()
        {
            var transformer1 = new GenericTransformer<SampleClassA, SampleClassB>(a => new SampleClassB { Value = int.Parse(a.Value) });
            var transformer2a = new GenericTransformer<SampleClassB, SampleClassC>(b => new SampleClassC { StringValue = b.Value.ToString(), IntValue = b.Value });
            var transformer2b = new GenericTransformer<SampleClassB, SampleClassC>(b => new SampleClassC { StringValue = b.Value.ToString(), IntValue = b.Value }, x => false);
            var transformer3 = new GenericTransformer<SampleClassC, SampleClassA>(c => new SampleClassA { Value = c.StringValue });

            var service = new TransformerService(transformer1, transformer2a, transformer2b, transformer3);
            var model = new SampleClassA { Value = "100" };

            var circularTransform = service.Transform<SampleClassA, SampleClassA>(model);
            Assert.AreEqual("100", circularTransform.Value);
        }

        [TestCase]
        public void CanHandleTransformingTypeToItself()
        {
            var transformer = new GenericTransformer<SampleClassA, SampleClassA>(a => new SampleClassA { Value = a.Value });
            var service = new TransformerService(transformer);
            var model = new SampleClassA { Value = "100" };
            var circularTransform = service.Transform<SampleClassA, SampleClassA>(model);
            Assert.AreEqual("100", circularTransform.Value);
        }

        [TestCase]
        public void CanUseMultipleTransformersOnSameArray()
        {
            var transformer1 = new GenericTransformer<SampleClassA, SampleClassB>(a => new SampleClassB { Value = int.Parse(a.Value) }, a=> a.Value.StartsWith("1"));
            var transformer2 = new GenericTransformer<SampleClassA, SampleClassB>(a => new SampleClassB { Value = 10 * int.Parse(a.Value) }, a => !a.Value.StartsWith("1"));

            var models = new SampleClassA[] { new SampleClassA { Value = "100" }, new SampleClassA { Value = "200" } };

            var service = new TransformerService(transformer1, transformer2);

            var result = service.Transform<SampleClassA[],SampleClassB[]>(models);

            Assert.AreEqual(100, result[0].Value);
            Assert.AreEqual(2000, result[1].Value);
        }
    }
}