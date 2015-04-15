﻿using Embark.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embark.Conversion
{
    /// <summary>
    /// Type wrapper with timestamp and ID that might be required for subsequent queries.
    /// </summary>
    public class DocumentWrapper<T>
    {
        internal DocumentWrapper(DataEnvelope dataEnvelope, Collection collection) 
            : this(dataEnvelope.ID,dataEnvelope.Text, collection)
        {
        }

        internal DocumentWrapper(long id, string text, Collection collection)
        {
            this.ID = id;            
            this.Value = collection.TextConverter.ToObject<T>(text);
            this.collection = collection;
        }

        /// <summary>
        /// Int64 ID Unique to the document
        /// </summary>
        public long ID { get; internal set; }

        /// <summary>
        /// Deserialized Object contained in the wrapper
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// The creation time of the document
        /// <remarks>
        /// Timestamp depends on the resolution time of the machine the document is created on.
        /// Usually accurate to within 15 milliseconds of actual created date.
        /// </remarks>
        /// </summary>
        public DateTime Timestamp { get { return new DateTime(ID); } }

        /// <summary>
        /// Commit document value to the database
        /// </summary>
        public void Update()
        {
            //TODO 1 Test

            this.collection.Update(this.ID, this.Value);
        }

        //TODO 2 Create delete & other Wrapped document requests/commands.

        private Embark.Conversion.Collection collection;

        // ?
        //public T Unwrap() { return Value; }

        /// <summary>
        /// ToString() of the object within the wrapper
        /// </summary>
        /// <returns>DocumentWrapper.Value.ToString()</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

}
