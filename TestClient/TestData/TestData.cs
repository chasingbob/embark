﻿using Embark;
using Embark.TextConversion;
using Embark.Interaction;
using Embark.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.TestData
{
    [TestClass]
    public class Cache
    {
        internal static Client localClient;
        internal static Collection localSheep;

        private static string testDir = @"C:\MyTemp\Embark\TestData\";

        [AssemblyInitialize()]
        public static void MyTestInitialize(TestContext testContext)
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, recursive: true);

            Directory.CreateDirectory(testDir);

            //localCache = new Client(testDir);
            localClient = Client.GetLocalDB(testDir);

            localSheep = localClient["sheep"];
            //serverCache = new Client("127.0.0.1", 80);
        }

        [AssemblyCleanup()]
        public static void MytestCleanup()
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, recursive: true);
        }
    }
}