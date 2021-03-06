﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Embark.Interaction.MVVM
{
    /// <summary>
    /// Basic implementation of INotifyPropertyChanged that Notifies clients that a property value has changed.
    /// </summary>
    public abstract class PropertyChangeBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Set the property and raise PropertyChanged and PropertyChanging events
        /// </summary>
        /// <typeparam name="T">Type of backing field</typeparam>
        /// <param name="backingField">Backing field of the property</param>
        /// <param name="newValue">New value to change to, if it is different.</param>
        /// <param name="propertyName">Name of the public property</param>
        /// <returns>Returns true if the property has changed, otherwise returns false.</returns>
        public bool SetProperty<T>(ref T backingField, T newValue, [CallerMemberName]string propertyName = "")
        {
            if (!Equals(backingField, newValue))
            {
                OnPropertyChanging(propertyName);
                backingField = newValue;
                OnPropertyChanged(propertyName);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Raise the PropertyChangedEvent of a property
        /// </summary>
        /// <param name="propertyName">Name of the public property</param>
        /// <returns>true if any observers listened to changes, otherwise false. </returns>
        public bool RaisePropertyChangedEvent([CallerMemberName]string propertyName = "") => OnPropertyChanged(propertyName);

        /// <summary>
        /// Raise the PropertyChangingEvent of a property
        /// </summary>
        /// <param name="propertyName">Name of the public property</param>
        /// <returns>true if any observers listened to changes, otherwise false. </returns>
        public bool RaisePropertyChangingEvent([CallerMemberName]string propertyName = "") => OnPropertyChanging(propertyName);
          
        private bool OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            else return false;
        }

        private bool OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                return true;
            }
            else return false;
        }        
    }
}
