﻿/* Copyright 2010-2012 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Bson;

namespace MongoDB.Driver
{
    /// <summary>
    /// The settings used to access a database.
    /// </summary>
    public class MongoDatabaseSettings
    {
        // private fields
        private Setting<MongoCredentials> _credentials;
        private Setting<GuidRepresentation> _guidRepresentation;
        private Setting<ReadPreference> _readPreference;
        private Setting<WriteConcern> _writeConcern;

        // the following fields are set when Freeze is called
        private bool _isFrozen;
        private int _frozenHashCode;
        private string _frozenStringRepresentation;

        // constructors
        /// <summary>
        /// Creates a new instance of MongoDatabaseSettings.
        /// </summary>
        public MongoDatabaseSettings()
        {
        }

        // public properties
        /// <summary>
        /// Gets or sets the credentials to access the database.
        /// </summary>
        public MongoCredentials Credentials
        {
            get { return _credentials.Value; }
            set
            {
                if (_isFrozen) { throw new InvalidOperationException("MongoDatabaseSettings is frozen."); }
                _credentials.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the representation to use for Guids.
        /// </summary>
        public GuidRepresentation GuidRepresentation
        {
            get { return _guidRepresentation.Value; }
            set
            {
                if (_isFrozen) { throw new InvalidOperationException("MongoDatabaseSettings is frozen."); }
                _guidRepresentation.Value = value;
            }
        }

        /// <summary>
        /// Gets whether the settings have been frozen to prevent further changes.
        /// </summary>
        public bool IsFrozen
        {
            get { return _isFrozen; }
        }

        /// <summary>
        /// Gets or sets the read preference.
        /// </summary>
        public ReadPreference ReadPreference
        {
            get { return _readPreference.Value; }
            set
            {
                if (_isFrozen) { throw new InvalidOperationException("MongoDatabaseSettings is frozen."); }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _readPreference.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the SafeMode to use.
        /// </summary>
        [Obsolete("Use WriteConcern instead.")]
        public SafeMode SafeMode
        {
            get { return (_writeConcern.Value == null) ? null : new SafeMode(_writeConcern.Value); }
            set
            {
                if (_isFrozen) { throw new InvalidOperationException("MongoDatabaseSettings is frozen."); }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _writeConcern.Value = value.WriteConcern;
            }
        }

        /// <summary>
        /// Gets or sets the WriteConcern to use.
        /// </summary>
        public WriteConcern WriteConcern
        {
            get { return _writeConcern.Value; }
            set
            {
                if (_isFrozen) { throw new InvalidOperationException("MongoDatabaseSettings is frozen."); }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _writeConcern.Value = value;
            }
        }

        // public methods
        /// <summary>
        /// Creates a clone of the settings.
        /// </summary>
        /// <returns>A clone of the settings.</returns>
        public MongoDatabaseSettings Clone()
        {
            var clone =  new MongoDatabaseSettings();
            clone._credentials = _credentials.Clone();
            clone._guidRepresentation = _guidRepresentation.Clone();
            clone._readPreference = _readPreference.Clone();
            clone._writeConcern = _writeConcern.Clone();
            return clone;
        }

        /// <summary>
        /// Compares two MongoDatabaseSettings instances.
        /// </summary>
        /// <param name="obj">The other instance.</param>
        /// <returns>True if the two instances are equal.</returns>
        public override bool Equals(object obj)
        {
            var rhs = obj as MongoDatabaseSettings;
            if (rhs == null)
            {
                return false;
            }
            else
            {
                if (_isFrozen && rhs._isFrozen)
                {
                    return _frozenStringRepresentation == rhs._frozenStringRepresentation;
                }
                else
                {
                    return
                        _credentials.Value == rhs._credentials.Value &&
                        _guidRepresentation.Value == rhs._guidRepresentation.Value &&
                        _readPreference.Value == rhs._readPreference.Value &&
                        _writeConcern.Value == rhs._writeConcern.Value;
                }
            }
        }

        /// <summary>
        /// Freezes the settings.
        /// </summary>
        /// <returns>The frozen settings.</returns>
        public MongoDatabaseSettings Freeze()
        {
            if (!_isFrozen)
            {
                if (_readPreference.Value != null) { _readPreference.Value = _readPreference.Value.FrozenCopy(); }
                if (_writeConcern.Value != null) { _writeConcern.Value = _writeConcern.Value.FrozenCopy(); }
                _frozenHashCode = GetHashCode();
                _frozenStringRepresentation = ToString();
                _isFrozen = true;
            }
            return this;
        }

        /// <summary>
        /// Returns a frozen copy of the settings.
        /// </summary>
        /// <returns>A frozen copy of the settings.</returns>
        public MongoDatabaseSettings FrozenCopy()
        {
            if (_isFrozen)
            {
                return this;
            }
            else
            {
                return Clone().Freeze();
            }
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            if (_isFrozen)
            {
                return _frozenHashCode;
            }

            // see Effective Java by Joshua Bloch
            int hash = 17;
            hash = 37 * hash + ((_credentials.Value == null) ? 0 : _credentials.Value.GetHashCode());
            hash = 37 * hash + _guidRepresentation.Value.GetHashCode();
            hash = 37 * hash + ((_readPreference.Value == null) ? 0 : _readPreference.Value.GetHashCode());
            hash = 37 * hash + ((_writeConcern.Value == null) ? 0 : _writeConcern.Value.GetHashCode());
            return hash;
        }

        /// <summary>
        /// Returns a string representation of the settings.
        /// </summary>
        /// <returns>A string representation of the settings.</returns>
        public override string ToString()
        {
            if (_isFrozen)
            {
                return _frozenStringRepresentation;
            }

            return string.Format(
                "Credentials={0};GuidRepresentation={1};ReadPreference={2};WriteConcern={3}",
                _credentials, _guidRepresentation, _readPreference, _writeConcern);
        }

        // internal methods
        internal void ApplyDefaultValues(MongoServerSettings serverSettings)
        {
            if (!_credentials.HasBeenSet)
            {
                Credentials = serverSettings.DefaultCredentials;
            }
            if (!_guidRepresentation.HasBeenSet)
            {
                GuidRepresentation = serverSettings.GuidRepresentation;
            }
            if (!_readPreference.HasBeenSet)
            {
                ReadPreference = serverSettings.ReadPreference;
            }
            if (!_writeConcern.HasBeenSet)
            {
                WriteConcern  = serverSettings.WriteConcern;
            }
        }
    }
}
