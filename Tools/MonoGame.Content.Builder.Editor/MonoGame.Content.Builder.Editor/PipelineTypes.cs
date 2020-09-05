// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Builder.Convertors;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace MonoGame.Content.Builder.Editor
{
    public class ImporterTypeDescription
    {
        public string TypeName { get; set; }
        public string DisplayName { get; set; }
        public string DefaultProcessor { get; set; }
        public IEnumerable<string> FileExtensions { get; set; }
        public Type OutputType { get; set; }

        public ImporterTypeDescription()
        {
            DisplayName = TypeName = "Invalid / Missing Importer";
            DefaultProcessor = string.Empty;
            FileExtensions = new List<string>();
            OutputType = typeof(object);
        }

        public override string ToString()
        {
            return TypeName;
        }

        public override int GetHashCode()
        {
            return TypeName == null ? 0 : TypeName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as ImporterTypeDescription;
            if (other == null)
                return false;

            if (string.IsNullOrEmpty(other.TypeName) != string.IsNullOrEmpty(TypeName))
                return false;

            return TypeName.Equals(other.TypeName);
        }
    };

    public class ProcessorTypeDescription
    {
        #region Supporting Types 

        public struct Property
        {
            public string Name;
            public string DisplayName;
            public Type Type;
            public object DefaultValue;
            public bool Browsable;

            public override string ToString()
            {
                return Name;
            }
        }

        public class ProcessorPropertyCollection : IEnumerable<Property>
        {
            private readonly Property[] _properties;

            public ProcessorPropertyCollection(IEnumerable<Property> properties)
            {
                _properties = properties.ToArray();
            }

            public Property this[int index]
            {
                get
                {
                    return _properties[index];
                }
                set
                {
                    _properties[index] = value;
                }
            }

            public Property this[string name]
            {
                get
                {
                    foreach (var p in _properties)
                    {
                        if (p.Name.Equals(name))
                            return p;
                    }

                    throw new IndexOutOfRangeException();
                }

                set
                {
                    for (var i = 0; i < _properties.Length; i++)
                    {
                        var p = _properties[i];
                        if (p.Name.Equals(name))
                        {
                            _properties[i] = value;
                            return;
                        }

                    }

                    throw new IndexOutOfRangeException();
                }
            }

            public bool Contains(string name)
            {
                return _properties.Any(e => e.Name == name);
            }

            public IEnumerator<Property> GetEnumerator()
            {
                return _properties.AsEnumerable().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _properties.GetEnumerator();
            }
        }

        #endregion

        public string TypeName;
        public string DisplayName;
        public ProcessorPropertyCollection Properties;
        public Type InputType;

        public ProcessorTypeDescription()
        {
            TypeName = DisplayName = "Invalid Property";
            Properties = new ProcessorPropertyCollection(new List<Property>());
            InputType = typeof(object);
        }

        public override string ToString()
        {
            return TypeName;
        }
    };

    public class PipelineTypes
    {
        [DebuggerDisplay("ImporterInfo: {Type.Name}")]
        private struct ImporterInfo
        {
            public ContentImporterAttribute Attribute;
            public Type Type;
        }

        [DebuggerDisplay("ProcessorInfo: {Type.Name}")]
        private struct ProcessorInfo
        {
            public ContentProcessorAttribute Attribute;
            public Type Type;
        }

        private static List<ImporterInfo> _importers;
        private static List<ProcessorInfo> _processors;

        public static ImporterTypeDescription[] Importers { get; private set; }
        public static ProcessorTypeDescription[] Processors { get; private set; }

        public static ImporterTypeDescription NullImporter { get; private set; }
        public static ProcessorTypeDescription NullProcessor { get; private set; }

        public static ImporterTypeDescription MissingImporter { get; private set; }
        public static ProcessorTypeDescription MissingProcessor { get; private set; }

        public static TypeConverter.StandardValuesCollection ImportersStandardValuesCollection { get; private set; }
        public static TypeConverter.StandardValuesCollection ProcessorsStandardValuesCollection { get; private set; }

        private static readonly Dictionary<string, string> _oldNameRemap = new Dictionary<string, string>()
            {
                { "MGMaterialProcessor", "MaterialProcessor" },
                { "MGSongProcessor", "SongProcessor" },
                { "MGSoundEffectProcessor", "SoundEffectProcessor" },
                { "MGSpriteFontDescriptionProcessor", "FontDescriptionProcessor" },
                { "MGSpriteFontTextureProcessor", "FontTextureProcessor" },
                { "MGTextureProcessor", "TextureProcessor" },
                { "MGEffectProcessor", "EffectProcessor" },
            };

        private static string RemapOldNames(string name)
        {
            if (_oldNameRemap.ContainsKey(name))
                return _oldNameRemap[name];

            return name;
        }

        static PipelineTypes()
        {
            _importers = new List<ImporterInfo>();
            _processors = new List<ProcessorInfo>();

            Importers = new ImporterTypeDescription[0];
            Processors = new ProcessorTypeDescription[0];

            ImportersStandardValuesCollection = new TypeConverter.StandardValuesCollection(Importers);
            ProcessorsStandardValuesCollection = new TypeConverter.StandardValuesCollection(Processors);

            MissingImporter = new ImporterTypeDescription()
            {
                DisplayName = "Invalid / Missing Importer",
            };

            MissingProcessor = new ProcessorTypeDescription()
            {
                DisplayName = "Invalid / Missing Processor",
                Properties = new ProcessorTypeDescription.ProcessorPropertyCollection(new ProcessorTypeDescription.Property[0]),
            };

            NullImporter = new ImporterTypeDescription()
            {
                DisplayName = "",
            };

            NullProcessor = new ProcessorTypeDescription()
            {
                DisplayName = "",
                Properties = new ProcessorTypeDescription.ProcessorPropertyCollection(new ProcessorTypeDescription.Property[0]),
            };
        }

        public static void Load(PipelineProject project)
        {
            Unload();

            var assemblyPaths = new List<string>();
            var projectRoot = project.Location;

            foreach (var i in project.References)
            {
                var path = Path.Combine(projectRoot, i);

                if (string.IsNullOrEmpty(path))
                    throw new ArgumentException("assemblyFilePath cannot be null!");
                if (!Path.IsPathRooted(path))
                    throw new ArgumentException("assemblyFilePath must be absolute!");

                // Make sure we're not adding the same assembly twice.
                path = PathHelper.Normalize(path);
                if (!assemblyPaths.Contains(path))
                    assemblyPaths.Add(path);
            }

            ResolveAssemblies(assemblyPaths);

            var importerDescriptions = new ImporterTypeDescription[_importers.Count];
            var cur = 0;
            foreach (var item in _importers)
            {
                // Find the abstract base class ContentImporter<T>.
                var baseType = item.Type.BaseType;
                while (!baseType.IsAbstract)
                    baseType = baseType.BaseType;

                var outputType = baseType.GetGenericArguments()[0];
                var name = item.Attribute.DisplayName;
                if (string.IsNullOrEmpty(name))
                    name = item.GetType().Name;
                var desc = new ImporterTypeDescription()
                {
                    TypeName = item.Type.Name,
                    DisplayName = name,
                    DefaultProcessor = item.Attribute.DefaultProcessor,
                    FileExtensions = item.Attribute.FileExtensions,
                    OutputType = outputType,
                };
                importerDescriptions[cur] = desc;
                cur++;
            }

            Importers = importerDescriptions;
            ImportersStandardValuesCollection = new TypeConverter.StandardValuesCollection(Importers);

            var processorDescriptions = new ProcessorTypeDescription[_processors.Count];

            const BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            cur = 0;
            foreach (var item in _processors)
            {
                var obj = Activator.CreateInstance(item.Type);
                var typeProperties = item.Type.GetProperties(bindings);
                var properties = new List<ProcessorTypeDescription.Property>();
                foreach (var i in typeProperties)
                {
                    var attrs = i.GetCustomAttributes(true);
                    var name = i.Name;
                    var browsable = true;
                    var defvalue = i.GetValue(obj, null);

                    foreach (var a in attrs)
                    {
                        if (a is BrowsableAttribute browsableAttribute)
                            browsable = browsableAttribute.Browsable;
                        else if (a is DisplayNameAttribute displayNameAttribute)
                            name = displayNameAttribute.DisplayName;
                    }

                    var p = new ProcessorTypeDescription.Property()
                    {
                        Name = i.Name,
                        DisplayName = name,
                        Type = i.PropertyType,
                        DefaultValue = defvalue,
                        Browsable = browsable
                    };
                    properties.Add(p);
                }

                var inputType = (obj is IContentProcessor iProcessor) ? iProcessor.InputType : typeof(object);
                var desc = new ProcessorTypeDescription()
                {
                    TypeName = item.Type.Name,
                    DisplayName = item.Attribute.DisplayName,
                    Properties = new ProcessorTypeDescription.ProcessorPropertyCollection(properties),
                    InputType = inputType,
                };

                if (string.IsNullOrEmpty(desc.DisplayName))
                    desc.DisplayName = desc.TypeName;

                processorDescriptions[cur] = desc;
                cur++;
            }

            Processors = processorDescriptions;
            ProcessorsStandardValuesCollection = new TypeConverter.StandardValuesCollection(Processors);
        }

        public static void Unload()
        {
            _importers = new List<ImporterInfo>();
            _processors = new List<ProcessorInfo>();

            Importers = new ImporterTypeDescription[0];
            Processors = new ProcessorTypeDescription[0];

            ImportersStandardValuesCollection = new TypeConverter.StandardValuesCollection(Importers);
            ProcessorsStandardValuesCollection = new TypeConverter.StandardValuesCollection(Processors);
        }

        public static TypeConverter FindConverter(Type type)
        {
            if (type == typeof(Color))
                return new StringToColorConverter();

            return TypeDescriptor.GetConverter(type);
        }

        public static ImporterTypeDescription? FindImporter(string name, string fileExtension)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = RemapOldNames(name);

                foreach (var i in Importers)
                {
                    if (i.TypeName.Equals(name))
                        return i;
                }

                foreach (var i in Importers)
                {
                    if (i.DisplayName.Equals(name))
                        return i;
                }

                //Debug.Fail(string.Format("Importer not found! name={0}, ext={1}", name, fileExtension));
                return null;
            }

            var lowerFileExt = fileExtension.ToLowerInvariant();
            foreach (var i in Importers)
            {
                if (i.FileExtensions.Any(e => e.ToLowerInvariant() == lowerFileExt))
                    return i;
            }

            //Debug.Fail(string.Format("Importer not found! name={0}, ext={1}", name, fileExtension));
            return null;
        }

        public static ProcessorTypeDescription? FindProcessor(string name, ImporterTypeDescription importer)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = RemapOldNames(name);

                foreach (var i in Processors)
                {
                    if (i.TypeName.Equals(name))
                        return i;
                }

                //Debug.Fail(string.Format("Processor not found! name={0}, importer={1}", name, importer));
                return null;
            }

            if (importer != null)
            {
                foreach (var i in Processors)
                {
                    if (i.TypeName.Equals(importer.DefaultProcessor))
                        return i;
                }
            }

            //Debug.Fail(string.Format("Processor not found! name={0}, importer={1}", name, importer));
            return null;
        }

        private static void ResolveAssemblies(IEnumerable<string> assemblyPaths)
        {
            _importers = new List<ImporterInfo>();
            _processors = new List<ProcessorInfo>();

            var assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());

            foreach (var asm in assemblies)
            {
#if !DEBUG
                try
#endif
                {
                    if (!asm.ToString().Contains("MonoGame"))
                        continue;

                    var types = asm.GetTypes();
                    ProcessTypes(types);
                }
#if !DEBUG
                catch (Exception e)
                {
                    // ??
                }
#endif
            }

            foreach (var path in assemblyPaths)
            {
                try
                {
                    var a = Assembly.LoadFrom(path);
                    var types = a.GetTypes();
                    ProcessTypes(types);
                }
                catch
                {
                    //Logger.LogWarning(null, null, "Failed to load assembly '{0}': {1}", assemblyPath, e.Message);
                    // The assembly failed to load... nothing
                    // we can do but ignore it.
                    continue;
                }
            }
        }

        private static void ProcessTypes(IEnumerable<Type> types)
        {
            foreach (var t in types)
            {
                if (t.IsAbstract)
                    continue;

                if (t.GetInterface(@"IContentImporter") == typeof(IContentImporter))
                {
                    var attributes = t.GetCustomAttributes(typeof(ContentImporterAttribute), false);
                    if (attributes.Length != 0 && attributes[0] is ContentImporterAttribute importerAttribute)
                    {
                        _importers.Add(new ImporterInfo { Attribute = importerAttribute, Type = t });
                    }
                    else
                    {
                        // If no attribute specify default one
                        var importerAttribute2 = new ContentImporterAttribute(".*");
                        importerAttribute2.DefaultProcessor = "";
                        importerAttribute2.DisplayName = t.Name;
                        _importers.Add(new ImporterInfo { Attribute = importerAttribute2, Type = t });
                    }
                }
                else if (t.GetInterface(@"IContentProcessor") == typeof(IContentProcessor))
                {
                    var attributes = t.GetCustomAttributes(typeof(ContentProcessorAttribute), false);
                    if (attributes.Length != 0 && attributes[0] is ContentProcessorAttribute processorAttribute)
                    {
                        _processors.Add(new ProcessorInfo { Attribute = processorAttribute, Type = t });
                    }
                }
            }
        }
    }
}
