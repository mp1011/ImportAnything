using ImportAnything.Models;
using ImportAnything.Services;
using ImportAnything.Services.Transformers;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImportAnything.Tests.Services
{
    [TestFixture]
    class CSVTransformerTests
    {
        [TestCase]
        public void CanTranslateCSVToDictionary()
        {
            var transformer = new TransformerService(new FileToCSV(), new CSVToDictionary());

            var file = new FileInfo(TestContext.CurrentContext.TestDirectory + @"\SampleFiles\SampleCSV1.csv");

            var dictionary = transformer.Transform<FileInfo, MappedCSV>(file);

            Assert.AreEqual("100 Comma Avenue, NotADelimiter Town USA", dictionary.ElementAt(2)["Address"]);
        }     

        [TestCase]
        public void CanConsolidateMultipleCSVs()
        {
            var folder = new DirectoryInfo(TestContext.CurrentContext.TestDirectory + @"\SampleFiles\");

            var csvImporter = new CSVToDictionary();
            csvImporter.AddMapping("First Name","First Name");
            csvImporter.AddMapping("Last Name", "Last Name");
            csvImporter.AddMapping("Address", "Address");
            csvImporter.AddMapping("Name", "First Name");
            csvImporter.AddMapping("SurName", "Last Name");

            var transformer = new TransformerService(new FolderToFiles("*.csv", recurse: false),
                new FileToCSV(),
                csvImporter);
            
            var results = transformer.Transform<DirectoryInfo, MappedCSV[]>(folder);
            
            Assert.AreEqual("Jones", results[1].First()["Last Name"]);
        }
    }
}
