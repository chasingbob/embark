﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestClient.IO;
using System.Collections.Generic;
using TestClient.IO.TestData;
using TestClient.TestData;
using Embark.Conversion;

namespace TestClient
{
    [TestClass]
    public class TestRangeLocal
    {

        [TestMethod]
        public void SelectAll_ReturnsAllItems()
        {
            //Arrange
            var allTestCollection = Cache.localCache["SelectAll"];
            var testHerd = Animals.GetTestHerd(5);

            var wrappedHerd = new List<WrappedSheep>();

            foreach (var sheep in testHerd)
            {
                var id = allTestCollection.Insert(sheep);
                wrappedHerd.Add(new WrappedSheep { ID = id, Sheep = sheep });
            }

            //Act
            var querySheep = allTestCollection.SelectAll<Sheep>();

            var unwrappedHerd = querySheep.Unwrap().ToArray();

            //Assert
            Assert.AreEqual(testHerd.Count, querySheep.Count());

            foreach(var documentWrapper in querySheep)
            {
                var wrappedSheep = wrappedHerd.Where(ws => ws.ID == documentWrapper.ID).Single();

                Assert.IsTrue(documentWrapper.Value.Equals( wrappedSheep.Sheep));
            }

            // Assumption that insert order = fetch order. If this changes, change the unit test and allow unordered insert & query.
            for (int i = 0; i < testHerd.Count; i++)
            {
                Assert.IsTrue(testHerd[i].Equals(unwrappedHerd[i]));
            }
        }

        [TestMethod]
        public void SelectBetween_ReturnsBetweenItems()
        {
            //Arrange
            var allTestCollection = Cache.localCache["SelectBetween"];
            var testHerd = new List<Sheep>();

            var oldWooly = new Sheep { Name = "Wooly", Age = 100, FavouriteIceCream = IceCream.Chocolate };
            var oldDusty = new Sheep { Name = "Dusty", Age = 50, FavouriteIceCream = IceCream.Chocolate };
            var youngLassy = new Sheep { Name = "Lassy", Age = 1, FavouriteIceCream = IceCream.Bubblegum };

            testHerd.Add(oldWooly);
            testHerd.Add(oldDusty);
            testHerd.Add(youngLassy);

            var wrappedHerd = new List<WrappedSheep>();
            foreach (var sheep in testHerd)
            {
                var id = allTestCollection.Insert(sheep);
                wrappedHerd.Add(new WrappedSheep { ID = id, Sheep = sheep });
            }

            //Act
            var betweenSheep = allTestCollection
                .SelectBetween<Sheep>(new { Age = 75 }, new { Age = 25 })
                .Single();

            //Assert
            Assert.IsTrue(betweenSheep.Value.Equals(oldDusty));
        }

        [TestMethod]
        public void GetSelectLike()
        {
            // arrange
            var oldWooly = new Sheep { Name = "Wooly", Age = 100, FavouriteIceCream = IceCream.Chocolate };
            var oldDusty = new Sheep { Name = "Dusty", Age = 100, FavouriteIceCream = IceCream.Chocolate };
            var youngLassy = new Sheep { Name = "Lassy", Age = 1, FavouriteIceCream = IceCream.Bubblegum };
                        
            long id = Cache.localSheep.Insert(oldWooly);
            long id2 = Cache.localSheep.Insert(oldDusty);
            long id3 = Cache.localSheep.Insert(youngLassy);

            // act            

            IEnumerable<Sheep> matchQuery = Cache.localSheep.SelectLike<Sheep>(new { Age = 100 });

            var ancients = matchQuery.ToList();

            // assert
            Assert.AreEqual(2, ancients.Count);

            Assert.IsFalse(ancients.Any(s => s.Age != 100));
            Assert.IsFalse(ancients.Any(s => s.Name == "Lassy"));

            Assert.IsTrue(ancients.Any(s => s.Name == "Wooly"));
            Assert.IsTrue(ancients.Any(s => s.Name == "Dusty"));
        }

        //[TestMethod]
        //public void DeleteSelectLike()
        //{
        //    // arrange
        //    var shakes = new Sheep { Name = "Shakes", Age = 50 };
        //    var shocks = new Sheep { Name = "Shocks", Age = 50 };
        //    var shiny = new Sheep { Name = "Shiny", Age = 40};

        //    long id = Cache.localSheep.Insert(shakes);
        //    long id2 = Cache.localSheep.Insert(shocks);
        //    long id3 = Cache.localSheep.Insert(shiny);

        //    // act            

        //    Cache.localSheep.DeleteLike(new { Age = 50 });

        //    IEnumerable<Sheep> matchQuery = Cache.localSheep.SelectLike<Sheep>(new { Age = 50 });

        //    var halfCenturySheep = matchQuery.ToList();

        //    // assert
        //    Assert.AreEqual(0, halfCenturySheep.Count);

        //    Assert.IsTrue(halfCenturySheep.Any(s => s.Age != 50));
        //    Assert.IsTrue(halfCenturySheep.Any(s => s.Name == "Shiny"));

        //    Assert.IsFalse(halfCenturySheep.Any(s => s.Name == "Shakes"));
        //    Assert.IsFalse(halfCenturySheep.Any(s => s.Name == "Shocks"));
        //}
    }
}
