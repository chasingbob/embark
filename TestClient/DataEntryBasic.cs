﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestClient;
using TestClient.IO.TestData;
using TestClient.TestData;

namespace TestClient
{
    [TestClass]
    public class DataEntryBasic
    {
        [TestMethod]
        public void Insert_SetsID()
        {
            // arrange
            var created = TestEntities.GetTestShoe();

            // act
            var saved = Cache.DataEntryCollection.Insert(created);

            // assert
            Assert.AreEqual(created.ID, saved.ID);
            Assert.AreEqual(created.Timestamp, saved.Timestamp);
        }
        
        [TestMethod]
        public void Get_HasInsertedValues()
        {
            // arrange
            var created = TestEntities.GetTestShoe();
            Cache.DataEntryCollection.Insert(created);

            // act
            var loaded = Cache.DataEntryCollection.Get(created.ID);

            // assert
            Assert.AreEqual(created.ID, loaded.ID);;
            Assert.AreEqual(created.Timestamp, loaded.Timestamp);
            Assert.AreEqual(created.Sound, loaded.Sound);
            Assert.AreEqual(created.Quality, loaded.Quality);
        }
        
        [TestMethod]
        public void Update_ChangesValues()
        {
            // arrange
            var created = TestEntities.GetTestShoe();
            Cache.DataEntryCollection.Insert(created);
            var saved = Cache.DataEntryCollection.Get(created.ID);

            // act
            saved.Quality = created.Quality + 10;
            Cache.DataEntryCollection.Update(saved);
            var loaded = Cache.DataEntryCollection.Get(created.ID);

            // assert
            Assert.AreEqual(loaded.Quality, saved.Quality);
            Assert.AreNotEqual(created.Quality, saved.Quality);
        }

        [TestMethod]
        public void Delete_RemovesEntry()
        {
            // arrange
            var created = TestEntities.GetTestShoe();
            long initialID = created.ID;
            var inserted = Cache.DataEntryCollection.Insert(created);
            long insertedID = inserted.ID;
            var loaded = Cache.DataEntryCollection.Get(created.ID);

            // act
            bool isDeleted = Cache.DataEntryCollection.Delete(loaded);
            var notFound = Cache.DataEntryCollection.Get(insertedID);
            
            // assert
            Assert.IsTrue(isDeleted);
            Assert.IsNull(notFound);
            Assert.AreNotEqual(initialID, insertedID);
            Assert.AreEqual(created.ID, inserted.ID);
        }

    }
}
