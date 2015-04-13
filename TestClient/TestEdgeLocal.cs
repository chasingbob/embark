﻿using Embark;
using Embark.Conversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestClient.IO.TestData;
using TestClient.TestData;

namespace TestClient
{
    [TestClass]
    public class TestEdgeLocal
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException), "Only alphanumerical & underscore characters supported in collection names.")]
        public void CollectionName_OnlyAlphanumericAndUnderScoreSupported()
        {
            var na = Client.GetLocalDB()["!?$@\filesystem.*"];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Collection name should be at least one alphanumerical or underscore character.")]
        public void CollectionName_CannotBeEmpty()
        {
            var na = Client.GetLocalDB()[""];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Collection name should be at least one alphanumerical or underscore character.")]
        public void CollectionName_CannotBeNull()
        {
            var na = Client.GetLocalDB()[null];
        }

        [TestMethod]
        public void SaveBlob_CanDeserializeToByteArray()
        {
            // arrange
            byte[] savedData = new byte[64];
            (new Random()).NextBytes(savedData);

            var saved = new { blob = savedData };

            long id = Cache.localCache.Basic.Insert(saved);

            // act
            var loaded = Cache.localCache.Basic.Select<Dictionary<string, object>>(id);
            var blob = loaded["blob"];
            byte[] loadedData = ExtensionMethods.GetByteArray(blob);

            // assert
            Assert.IsTrue(Enumerable.SequenceEqual(savedData, loadedData));
        }

        [TestMethod]
        public void SaveNonPoco_HandlesComparison()
        {
            // arrange
            var io = Cache.localCache.GetCollection<string>("nonPOCO");
            string input = "string";
            string inserted;

            // act & assert
            RunAllCommands<string>(io, input, out inserted);
            Assert.AreEqual(input, inserted);            
        }

        [TestMethod]
        public void MixedTypes_CanSaveInSameDB()
        {
            // arrange
            var io = Cache.localCache.GetCollection<object>("MixedDataObjects");
            Sheep inputSheep = new Sheep { Name = "Mittens" };
            object outputObject;
                        
            // act
            io.Insert("non-sheep");
            io.Insert(123);

            // act & assert
            RunAllCommands(io, inputSheep, out outputObject);

            var outSheepText = io.TextConverter.ToText(outputObject);
            Sheep outputSheep = io.TextConverter.ToObject<Sheep>(outSheepText);

            Assert.AreEqual(inputSheep, outputSheep);
        }

        private static void RunAllCommands<T>(CollectionT<T> io, T input, out T inserted) where T : class
        {
            // act & assert
            var id = io.Insert(input);

            var all = io.SelectAll().ToArray();

            var like = io.SelectLike("string").ToArray();

            var between = io.SelectBetween("str", "qqqqing").ToArray();

            inserted = io.Select(id);
        }

        //[TestMethod]
        public void UpdateNonExisting_ReturnsFalse()
        {
            throw new NotImplementedException();
        }
    }
}
