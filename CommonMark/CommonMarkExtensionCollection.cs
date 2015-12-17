using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Extension collection.
    /// </summary>
    public class CommonMarkExtensionCollection : IEnumerable<ICommonMarkExtension>
    {
        private List<ICommonMarkExtension> _list;

        internal CommonMarkExtensionCollection(CommonMarkSettings settings)
        {
            this.Settings = settings;
        }

        internal CommonMarkExtensionCollection(CommonMarkSettings settings, CommonMarkExtensionCollection extensions)
            : this(settings)
        {
            this._list = extensions._list != null ? new List<ICommonMarkExtension>(extensions._list) : null;
        }

        private List<ICommonMarkExtension> List
        {
            get
            {
                if (_list == null)
                {
                    _list = new List<ICommonMarkExtension>();
                }
                return _list;
            }
        }

        /// <summary>
        /// Registers an extension. Extensions must not retain references to the settings object.
        /// </summary>
        /// <param name="extension">The extension to register.</param>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Extension is already registered.</exception>
        public void Register(ICommonMarkExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            if (_list != null && _list.Contains(extension))
            {
                var message = string.Format("Extension is already registered: {0}.", extension.ToString());
                throw new InvalidOperationException(message);
            }

            List.Add(extension);
            Settings.Reset();
        }

        /// <summary>
        /// Registers multiple extensions. Extensions must not retain references to the settings object.
        /// </summary>
        /// <param name="extensions">The extensions to register.</param>
        /// <exception cref="ArgumentNullException"><paramref name="extensions"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Extension is already registered.</exception>
        public void Register(IEnumerable<ICommonMarkExtension> extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException(nameof(extensions));

            if (_list != null)
            {
                foreach (var extension in extensions)
                {
                    if (_list.Contains(extension))
                    {
                        var message = string.Format("Extension is already registered: {0}.", extension.ToString());
                        throw new InvalidOperationException(message);
                    }
                }
            }

            List.AddRange(extensions);
            Settings.Reset();
        }

        /// <summary>
        /// Registers all built-in extensions with all their features enabled.
        /// This may be useful in benchmarking.
        /// </summary>
        public void RegisterAll()
        {
            Register(new Extension.Strikeout(Settings));
            Register(new Extension.ReferenceCase(Settings, new Extension.ReferenceCaseSettings(Extension.ReferenceCaseType.UpperInvariant)));
            Register(new Extension.DefinitionLists(Settings, new Extension.DefinitionListsSettings(Extension.DefinitionListsFeatures.All)));
            Register(new Extension.PipeTables(Settings, new Extension.PipeTablesSettings(Extension.PipeTablesFeatures.All)));
            Register(new Extension.TableCaptions(Settings, new Extension.TableCaptionsSettings
            {
                Features = Extension.TableCaptionsFeatures.All,
                Leads = new[] { "Table" },
            }));
            Settings.Reset();
        }

        /// <summary>
        /// Unregisters an extension.
        /// </summary>
        /// <param name="extension">The extension to unregister.</param>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Extension is not registered.</exception>
        public void Unregister(ICommonMarkExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            if (_list == null || !_list.Remove(extension))
            {
                var message = string.Format("Extension is not registered: {0}.", extension.ToString());
                throw new InvalidOperationException(message);
            }

            Settings.Reset();
        }

        /// <summary>
        /// Unregisters all extensions.
        /// </summary>
        public void UnregisterAll()
        {
            if (_list != null)
            {
                _list.Clear();
                Settings.Reset();
            }
        }

        /// <summary>
        /// Determines whether an extension is registered.
        /// </summary>
        /// <param name="extension">The extension to locate.</param>
        /// <returns><c>true</c> if the extension is registered.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/> is <c>null</c>.</exception>
        public bool IsRegistered(ICommonMarkExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            return _list != null && _list.Contains(extension);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="CommonMarkExtensionCollection"/>.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<ICommonMarkExtension> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal TValue[] GetItems<TKey, TValue>(Func<int, TValue[]> itemsFactory,
            Func<ICommonMarkExtension, IDictionary<TKey, TValue>> itemMapFactory,
            Func<TKey, int> keyFactory, Func<TValue, TValue, TValue> valueFactory)
            where TKey : struct
        {
            if (_list == null)
                return itemsFactory(0);

            var dictionary = new Dictionary<TKey, TValue>();
            var maxKey = 0;

            TValue inner;
            foreach (var extension in _list)
            {
                var extensionItems = itemMapFactory(extension);
                if (extensionItems != null)
                {
                    foreach (var kvp in extensionItems)
                    {
                        var value = kvp.Value;
                        if (value == null || value.Equals(default(TValue)))
                        {
                            var message = string.Format("{0} value cannot be {1}: {2}.",
                                typeof(TValue).Name,
                                value == null ? "null" : "0",
                                extension.ToString());
                            throw new InvalidOperationException(message);
                        }
                        dictionary.TryGetValue(kvp.Key, out inner);
                        if (inner != null && !inner.Equals(default(TValue)))
                            value = valueFactory(inner, value);
                        dictionary[kvp.Key] = value;
                        var key = keyFactory(kvp.Key);
                        maxKey = maxKey > key ? maxKey : key;
                    }
                }
            }

            var items = itemsFactory(maxKey + 1);

            foreach (var kvp in dictionary)
            {
                var key = keyFactory(kvp.Key);
                items[key] = valueFactory(items[key], kvp.Value);
            }

            return items;
        }

        internal TValue[] GetItems<TKey, TValue>(TValue[] items,
            Func<ICommonMarkExtension, IDictionary<TKey, TValue>> itemMapFactory,
            Func<TKey, int> keyFactory,
            Func<TValue, TValue, TValue> valueFactory)
            where TKey : struct
        {
            if (_list == null)
                return items;

            foreach (var extension in _list)
            {
                var extensionItems = itemMapFactory(extension);
                if (extensionItems != null)
                {
                    foreach (var kvp in extensionItems)
                    {
                        var value = kvp.Value;
                        if (value == null || value.Equals(default(TValue)))
                        {
                            var message = string.Format("{0} value cannot be {1}: {2}.",
                                typeof(TValue).Name,
                                value == null ? "null" : "0",
                                extension.ToString());
                            throw new InvalidOperationException(message);
                        }
                        var key = keyFactory(kvp.Key);
                        var inner = items[key];
                        if (inner != null && !inner.Equals(default(TValue)))
                            value = valueFactory(inner, value);
                        items[key] = value;
                    }
                }
            }

            return items;
        }

        private CommonMarkSettings Settings { get; }
    }
}
