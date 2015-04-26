﻿using Embark;
using Embark.Conversion;
using Embark.Interaction;
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

            long id = Cache.localClient.Basic.Insert(saved);

            // act
            var loaded = Cache.localClient.Basic.Get<Dictionary<string, object>>(id);
            var blob = loaded["blob"];
            byte[] loadedData = TypeConversion.ToByteArray(blob);

            // assert
            Assert.IsTrue(Enumerable.SequenceEqual(savedData, loadedData));
        }

        [TestMethod]
        public void SaveNonPoco_HandlesComparison()
        {
            // arrange
            var io = Cache.localClient.GetCollection<string>("nonPOCO");
            string input = "string";
            string inserted;

            // act & assert
            RunAllCommands<string>(io, input, out inserted);
            Assert.AreEqual(input, inserted);            
        }

        [TestMethod]
        public void MixedTypeCollection_CanSave()
        {
            // arrange
            var io = Cache.localClient.GetCollection<object>("MixedDataObjects");
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

        [TestMethod]
        public void GetWhere_MatchesSubProperties()
        {
            // arrange
            var oldWooly = new Sheep { Name = "Wooly", Age = 100, FavouriteIceCream = IceCream.Chocolate };
            var oldDusty = new Sheep { Name = "Dusty", Age = 100, FavouriteIceCream = IceCream.Chocolate, OnTable = new Table { Legs = 2 } };
            var youngLassy = new Sheep { Name = "Lassy", Age = 1, FavouriteIceCream = IceCream.Bubblegum, OnTable = new Table { IsSquare = true } };
            var youngBilly = new Sheep { Name = "Billy", Age = 3, OnTable = new Table { Legs = 2 } };

            var io = Cache.localClient.GetCollection<Sheep>("subMatch");

            long id = io.Insert(oldWooly);
            long id2 = io.Insert(oldDusty);
            long id3 = io.Insert(youngLassy);

            // act            

            var anonymousTable = new { Legs = 2 };
            IEnumerable<Sheep> matchQuery = io.GetWhere(new { Age = 100, OnTable = anonymousTable }).Unwrap();

            var oldSheepOnTables = matchQuery.ToList();

            // assert
            Assert.AreEqual(1, oldSheepOnTables.Count);

            Assert.IsFalse(oldSheepOnTables.Any(s => s.Age != 100));
            Assert.IsFalse(oldSheepOnTables.Any(s => s.OnTable.Legs != 2));

            Assert.IsFalse(oldSheepOnTables.Any(s => s.Name == "Lassy"));
            Assert.IsFalse(oldSheepOnTables.Any(s => s.Name == "Wooly"));
            Assert.IsFalse(oldSheepOnTables.Any(s => s.Name == "Wooly"));
            
            Assert.IsTrue(oldSheepOnTables.Any(s => s.Name == "Dusty"));
        }

        private static void RunAllCommands<T>(Collection<T> io, T input, out T inserted) where T : class
        {
            // act & assert
            var id = io.Insert(input);

            var all = io.GetAll().ToArray();

            var like = io.GetWhere("string").ToArray();

            var between = io.GetBetween("str", "qqqqing").ToArray();

            inserted = io.Get(id);
        }

        //[TestMethod]
        public void UpdateNonExisting_ReturnsFalse()
        {
            throw new NotImplementedException();
        }
    }
}
