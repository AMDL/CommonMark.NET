﻿using System;
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
            Register(new Extension.Strikeout());
            Register(new Extension.Subscript());
            Register(new Extension.Superscript());
            Register(new Extension.MathDollars());
            Register(new Extension.LegacyLists());
            Register(new Extension.ReferenceCase(new Extension.ReferenceCaseSettings(Extension.ReferenceCaseType.UpperInvariant)));
            Register(new Extension.LooseLists(new Extension.LooseListsSettings()));
            Register(new Extension.FancyLists(new Extension.FancyListsSettings
            {
                Features = Extension.FancyListsFeatures.All,
                NumericListStyles = Extension.NumericListStyles.All,
                AlphaListStyles = Extension.AlphaListStyles.All,
                AdditiveListStyles = Extension.AdditiveListStyles.All,
            }));
            Register(new Extension.DefinitionLists(new Extension.DefinitionListsSettings(Extension.DefinitionListsFeatures.All)));
            Register(new Extension.PipeTables(new Extension.PipeTablesSettings(Extension.PipeTablesFeatures.All)));
            Register(new Extension.TableCaptions(new Extension.TableCaptionsSettings
            {
                Features = Extension.TableCaptionsFeatures.All,
                Leads = new[] { "Table" },
            }));
            Register(new Extension.TrackSourcePositions());
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

        internal void InitializeBlockParsing(Parser.BlockParserParameters parameters)
        {
            if (_list != null)
            {
                foreach (var extension in _list)
                {
                    extension.InitializeBlockParsing(parameters);
                }
            }
        }

        internal void InitializeInlineParsing(Parser.InlineParserParameters parameters)
        {
            if (_list != null)
            {
                foreach (var extension in _list)
                {
                    extension.InitializeInlineParsing(parameters);
                }
            }
        }

        internal void InitializeFormatting(Formatters.FormatterParameters parameters)
        {
            if (_list != null)
            {
                foreach (var extension in _list)
                {
                    extension.InitializeFormatting(parameters);
                }
            }
        }

        internal T[] GetItems<T>(IEnumerable<T> initItems, int count,
            Func<ICommonMarkExtension, IEnumerable<T>> itemsFactory,
            Func<T, int> keyFactory, Func<T, T, T> valueFactory,
            Func<int, T> dummyFactory)
        {
            var items = new T[count];

            foreach (var item in initItems)
            {
                var key = keyFactory(item);
                items[key] = item;
            }

            if (_list == null)
                return items;

            foreach (var extension in _list)
            {
                var extensionItems = itemsFactory(extension);
                if (extensionItems != null)
                {
                    foreach (var item in extensionItems)
                    {
                        var key = keyFactory(item);
                        items[key] = valueFactory(items[key], item);
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                if (items[i] == null)
                    items[i] = dummyFactory(i);
            }

            return items;
        }

        internal TValue[] GetItems<TKey, TValue>(IEnumerable<TKey> initItems,
            Func<ICommonMarkExtension, IEnumerable<TKey>> itemsFactory,
            Func<TKey, int> keyFactory, Func<TKey, TValue> valueFactory, Func<TValue, TValue, TValue> mergeFactory)
        {
            var itemList = new List<TKey>(initItems);

            if (_list != null)
            {
                foreach (var extenison in _list)
                {
                    var extensionItems = itemsFactory(extenison);
                    if (extensionItems != null)
                    {
                        itemList.AddRange(extensionItems);
                    }
                }
            }

            var dictionary = new Dictionary<int, TValue>();
            var maxKey = 0;

            TValue inner;
            foreach (var item in itemList)
            {
                var key = keyFactory(item);
                var value = valueFactory(item);
                dictionary.TryGetValue(key, out inner);
                dictionary[key] = mergeFactory(inner, value);
                maxKey = maxKey > key ? maxKey : key;
            }

            var items = new TValue[maxKey + 1];

            foreach (var kvp in dictionary)
            {
                items[kvp.Key] = kvp.Value;
            }

            return items;
        }

        private CommonMarkSettings Settings { get; }
    }
}
