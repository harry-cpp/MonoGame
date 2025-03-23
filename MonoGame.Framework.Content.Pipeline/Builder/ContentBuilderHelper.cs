// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Framework.Content.Pipeline.Builder;

internal class ContentBuilderHelper
{
    private struct ImporterInfo
    {
        public ContentImporterAttribute? attribute;
        public Type type;
    };

    private struct ProcessorInfo
    {
        public ContentProcessorAttribute? attribute;
        public Type type;
    };

    private static readonly List<ImporterInfo> _importers = [];
    private static readonly List<ProcessorInfo> _processors = [];

    static ContentBuilderHelper()
    {
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] exportedTypes = a.GetTypes();
            foreach (var t in exportedTypes)
            {
                if (t.IsAbstract)
                    continue;

                if (t.GetInterface(@"IContentImporter") != null)
                {
                    _importers.Add(new ImporterInfo
                    {
                        attribute = GetImporterAttribute(t),
                        type = t
                    });
                }
                else if (t.GetInterface(@"IContentProcessor") != null)
                {
                    _processors.Add(new ProcessorInfo
                    {
                        attribute = GetProcessorAttribute(t),
                        type = t
                    });
                }
            }
        }
    }

    public static ContentImporterAttribute GetImporterAttribute(Type t)
    {
        var attributes = t.GetCustomAttributes(typeof(ContentImporterAttribute), false);
        for (int i = 0; i < attributes.Length; i++)
        {
            if (attributes[i] is ContentImporterAttribute attribute)
            {
                return attribute;
            }
        }

        return new ContentImporterAttribute(".*")
        {
            DefaultProcessor = "",
            DisplayName = t.Name
        };
    }

    public static ContentProcessorAttribute GetProcessorAttribute(Type t)
    {
        var attributes = t.GetCustomAttributes(typeof(ContentProcessorAttribute), false);
        for (int i = 0; i < attributes.Length; i++)
        {
            if (attributes[i] is ContentProcessorAttribute attribute)
            {
                return attribute;
            }
        }

        return new ContentProcessorAttribute()
        {
            DisplayName = t.Name
        };
    }

    public static bool GetImporter(string relativePath, IContentImporter? inImporter, out IContentImporter outImporter)
    {
        if (inImporter != null)
        {
            outImporter = inImporter;
            return true;
        }

        foreach (var info in _importers)
        {
            string fileExtension = Path.GetExtension(relativePath);
            if (info.attribute?.FileExtensions.Any(e => e.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase)) ?? false)
            {
                outImporter = (IContentImporter)Activator.CreateInstance(info.type)!;
                return true;
            }
        }

        outImporter = null!;
        return false;
    }

    public static bool GetProcessor(IContentImporter inImporter, IContentProcessor? inProcessor, out IContentProcessor outProcessor)
    {
        if (inProcessor != null)
        {
            outProcessor = inProcessor;
            return true;
        }

        var attribute = GetImporterAttribute(inImporter.GetType());

        foreach (var processor in _processors)
        {
            if (processor.type.Name == attribute.DefaultProcessor)
            {
                outProcessor = (IContentProcessor)Activator.CreateInstance(processor.type)!;
                return true;
            }
        }

        outProcessor = null!;
        return false;
    }
}
