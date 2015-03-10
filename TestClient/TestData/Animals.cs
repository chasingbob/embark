﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient.IO
{
    internal class Animals
    {
        internal static Sheep GetTestSheep()
        {
            var r = rnd.Value;
            return new Sheep
            {
                Name = petNames[r.Next(0, petNames.Length)],
                Age = r.Next(1, 15),
                FavouriteIceCream = (IceCream)r.Next(0, 4)
            };
        }

        internal List<Sheep> GetTestHerd(int count = 10)
        {
            return Enumerable.Range(0, count)
                .Select(i => GetTestSheep())
                .ToList();
        }

        static ThreadLocal<Random> rnd = new ThreadLocal<Random>(() => new Random());
        static string[] petNames = new string[] { "Candyfloss", "Dimples", " Fluffy", " Jingles", " Monkey", " Rambo", " Snowball", " Tumble", " Zebra" };
    }

    public class Sheep
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public IceCream FavouriteIceCream { get; set; }
    }

    public class Cat
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double FurDensity { get; set; }
        public bool HasMeme { get; set; }
    }

    public enum IceCream
    {
        Bubblegum,
        Chocolate,
        Strawberry,
        Vanilla
    }
}
