﻿using Embark.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embark.Cache
{
    public class Repository  
    {
        public Repository(string dataDirectory)
        {
            var collectionsFolder = dataDirectory + @"\Collections\";
            var keysFolder = dataDirectory + @"\Keys\";
            
            this.keyProvider = new KeyProvider(keysFolder);
            this.tagPaths = new CollectionPaths(collectionsFolder);
        }
        
        private KeyProvider keyProvider;
        private CollectionPaths tagPaths;
        private object syncRoot = new object();

        // Basic
        public long Insert(string tag, string jsonText)
        {
            // Get ID from IDGen
            var key = keyProvider.GetKey(tag);
                
            // TODO 3 offload to queue that gets processed by task
            var savePath = tagPaths.GetJsonPath(tag, key);

            // TODO 1 NB get a document only lock, instead of all repositories lock
            lock (syncRoot)
            {
                // Save object to tag dir
                File.WriteAllText(savePath, jsonText);

                //Return ID to client
                return key;
            }
        }
        
        public string Get(string tag, long id)
        {
            var savePath = tagPaths.GetJsonPath(tag, id);

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

        public bool Update(string tag, long id, string jsonText)
        {
            var savePath = tagPaths.GetJsonPath(tag, id);
            
            lock(syncRoot)
            {
                if (!File.Exists(savePath))
                    return false;
                else
                {
                    File.WriteAllText(savePath, jsonText);
                    return true;
                }
            }
        }

        public bool Delete(string tag, long id)
        {
            var savePath = tagPaths.GetJsonPath(tag, id);

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


        // Range

        public List<T> GetWhere<T>(string tag, T newValue, T oldValue, T optionalEndrange = null) where T : class
        {
            throw new NotImplementedException();
        }

        public int UpdateWhere<T>(string tag, T newValue, T oldValue, T optionalEndrange = null) where T : class
        {
            throw new NotImplementedException();
        }
                
        public int DeleteWhere<T>(string tag, T newValue, T oldValue, T optionalEndrange = null) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
