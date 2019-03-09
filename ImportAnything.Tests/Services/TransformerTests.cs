using ImportAnything.Services;
using ImportAnything.Services.Transformers;
using ImportAnything.Tests.Models;
using NUnit.Framework;

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
            var transformed = transformer.Transform(model);
            Assert.AreEqual(100, transformed.Value);
        }

        [TestCase]
        public void CanTransformObjectBetweenThreeTypes()
        {
            var model = new SampleClassA { Value = "100" };
            var transformer1 = new GenericTransformer<SampleClassA, SampleClassB>(a => new SampleClassB { Value = int.Parse(a.Value) });
            var transformer2 = new GenericTransformer<SampleClassB, SampleClassC>(b => new SampleClassC { StringValue = b.Value.ToString(), IntValue = b.Value });

            var transformerService = new TransformerService(transformer1,transformer2);

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
    }
}
