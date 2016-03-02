using System;
using System.Collections.Generic;
using System.Text;

namespace Nuntius
{
    /// <summary>
    /// Represents a message used by Nuntius library. TODO: thread safety?
    /// </summary>
    public sealed class NuntiusMessage
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        public string this[string key]
        {
            get { return GetProperty(key); }
            set { AddProperty(key, value); }
        }

        /// <summary>
        /// Represents properties of the message. Properties are used to save message information.
        /// </summary>
        public IDictionary<string, string> Properties => _properties;

        /// <summary>
        /// Adds key value pair to the message.
        /// </summary>
        /// <param name="key">Key for the value. Must not be null or empty.</param>
        /// <param name="value">Value to add. Must not be null.</param>
        public void AddProperty(string key, string value)
        {
            ValidateKey(key);
            if (value == null) throw new ArgumentNullException($"{nameof(value)} must not be null.");
            _properties[key] = value;
        }

        /// <summary>
        /// Returns a property of the message saved under the given key. If the key doesn't exist returns null.
        /// </summary>
        /// <param name="key">Key under which to look for property.</param>
        /// <returns>Value of the property. Null if the property is not present.</returns>
        public string GetProperty(string key)
        {
            ValidateKey(key);
            string result;
            if (_properties.TryGetValue(key, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Removes a property of the given key. If the property is not present returns false.
        /// </summary>
        /// <param name="propertyName">Key of the property to remove.</param>
        /// <returns>True if property was present.</returns>
        public bool RemoveProperty(string propertyName)
        {
            return _properties.Remove(propertyName);
        }

        /// <summary>
        /// Binary body of the message.
        /// </summary>
        public byte[] Body { get; set; }

        private void ValidateKey(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentException($"{nameof(key)} must not be null and at least 1 character long.");
        }

        /// <summary>
        /// Returns a clone of this object. All properties are copied but the binary body. In that case, 
        /// only the references only exchanged.
        /// </summary>
        /// <returns></returns>
        public NuntiusMessage Clone()
        {
            var clone = new NuntiusMessage();
            foreach (var pair in Properties)
            {
                clone[pair.Key] = pair.Value;
            }
            clone.Body = Body;
            return clone;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var keyPair in _properties)
            {
                builder.Append("[");
                builder.Append(keyPair.Key);
                builder.Append(" : ");
                builder.Append(keyPair.Value);
                builder.Append("]  ");
            }
            return builder.ToString();
        }
    }
}
