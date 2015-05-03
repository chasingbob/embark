﻿using System;
using System.IO;
using System.Linq;
using Embark.DataChannel;

namespace Embark.Storage
{
    internal class FileDataStore
    {
        public FileDataStore(string directory)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";

            var collectionsFolder = directory + @"Collections\";
            var keysFolder = directory + @"Map\";
            //var logfolder = directory + @"Pending\";

            long lastKey = InitializeKeyPath(keysFolder);
                        
            this.keyProvider = new DocumentKeySource(lastKey);
            this.tagPaths = new CollectionPaths(collectionsFolder);
        }
        
        private DocumentKeySource keyProvider;
        private CollectionPaths tagPaths;

        private string keysFile;

        private object syncRoot = new object();

        private long InitializeKeyPath(string keysDirectory)
        {
            if (!Directory.Exists(keysDirectory))
                Directory.CreateDirectory(keysDirectory);

            keysFile = keysDirectory + "LatestKey.txt";

            if (!File.Exists(keysFile))
            {
                File.WriteAllText(keysFile, "0");
                return 0;
            }
            else
            {
                var keyTxt = File.ReadAllText(keysFile);
                return Int64.Parse(keyTxt);
            }
            
            // TODO 1 Append to logfile to return faster
            // 2nd task runs that empties log(s) to text file.
            // Write generically so that log writer/comitter(s)
            // can be re-used for collection insert/update/delete commands also
        }

        public long Insert(string tag, string objectToInsert)
        {
            // Get ID from IDGen
            var key = keyProvider.GetNewKey();
                
            // TODO 3 offload to queue that gets processed by task
            var savePath = tagPaths.GetDocumentPath(tag, key.ToString());

            // TODO 1 NB get a document only lock, instead of all repositories lock
            lock (syncRoot)
            {
                // Save newest key
                File.WriteAllText(keysFile, key.ToString());

                // Save object to tag dir
                File.WriteAllText(savePath, objectToInsert);

                //Return ID to client
                return key;
            }
        }
        
        public bool Update(string tag, string id, string objectToUpdate)
        {
            var savePath = tagPaths.GetDocumentPath(tag, id);
            
            lock(syncRoot)
            {
                if (!File.Exists(savePath))
                    return false;
                else
                {
                    File.WriteAllText(savePath, objectToUpdate);
                    return true;
                }
            }
        }

        public bool Delete(string tag, string id)
        {
            var savePath = tagPaths.GetDocumentPath(tag, id);

            lock (syncRoot)
            {
                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                    return true;
                }
                else return false;
            }
        }

        public string Get(string tag, string id)
        {
            var savePath = tagPaths.GetDocumentPath(tag, id);

            string jsonText;

            // TODO lock row only
            lock (syncRoot)
            {
                if (!File.Exists(savePath))
                    return null;

                jsonText = File.ReadAllText(savePath);
            }
            return jsonText;
        }

        public DataEnvelope[] GetAll(string tag)
        {
            lock (syncRoot)
            {
                var tagDir = tagPaths.GetCollectionDirectory(tag);

                var allFiles = Directory
                    .GetFiles(tagDir)
                    //.EnumerateFiles(tagDir)
                    .Select(filePath => GetDataEnvelope(filePath))
                    .ToArray();

                return allFiles;
            }
        }

        // TODO Try/Catch return error envelope.
        private DataEnvelope GetDataEnvelope(string filePath)
        {
            return new DataEnvelope
            {
                ID = Int64.Parse(Path.GetFileNameWithoutExtension(filePath)),
                Text = File.ReadAllText(filePath)
            };
        }
    }
}
