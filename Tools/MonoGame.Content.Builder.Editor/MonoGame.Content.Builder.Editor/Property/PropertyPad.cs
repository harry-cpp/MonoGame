// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    public partial class PropertyPad : Pad
    {
        private List<object> _objects;
        private PropertyTable _propertyTable;
        private RadioCommand _cmdSortAbc, _cmdSortGroup;

        public PropertyPad()
        {
            _objects = new List<object>();

            _propertyTable = new PropertyTable(this);

            _cmdSortAbc = new RadioCommand();
            _cmdSortAbc.MenuText = "Sort Alphabetically";
            AddViewItem(_cmdSortAbc);

            _cmdSortGroup = new RadioCommand();
            _cmdSortGroup.Controller = _cmdSortAbc;
            _cmdSortGroup.MenuText = "Sort by Category";
            _cmdSortGroup.Checked = true;
            AddViewItem(_cmdSortGroup);

            _cmdSortAbc.CheckedChanged += (o, e) => Reload();
            _cmdSortGroup.CheckedChanged += (o, e) => Reload();
        }

        public override Control Control => _propertyTable;

        public override string Title => "Properties";

        public List<object> Objects
        {
            get => _objects;
            set
            {
                _objects = value;
                Reload();
            }
        }

        public override void UpdateEnabledCommands(Commands commands)
        {
            
        }

        public void Reload()
        {
            _propertyTable.Clear();

            if (_objects != null && _objects.Count != 0)
                LoadProperties(_objects);
            
            _propertyTable.Update(_cmdSortGroup.Checked);
        }

        private bool CompareVariables(ref object a, object b, PropertyInfo p)
        {
            var prop = b.GetType().GetProperty(p.Name);
            if (prop == null)
                return false;

            if (a == null || !a.Equals(prop.GetValue(b, null)))
                a = null;

            return true;
        }

        private void LoadProperties(List<object> objects)
        {
            var props = objects[0].GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var p in props)
            {
                var attrs = p.GetCustomAttributes(true);
                var name = p.Name;
                var browsable = true;
                var category = "Mics";

                foreach (var a in attrs)
                {
                    if (a is BrowsableAttribute browsableAttribute)
                        browsable = browsableAttribute.Browsable;
                    else if (a is CategoryAttribute categoryAttribute)
                        category = categoryAttribute.Category;
                    else if (a is DisplayNameAttribute displayNameAttribute)
                        name = displayNameAttribute.DisplayName;
                }

                object value = p.GetValue(objects[0], null);
                foreach (object o in objects)
                {
                    if (!CompareVariables(ref value, o, p))
                    {
                        browsable = false;
                        break;
                    }
                }

                if (!browsable)
                    continue;

                _propertyTable.AddEntry(category, name, value, p.PropertyType, p.CanWrite, val =>
                {
                    foreach (var obj in objects)
                        p.SetValue(obj, val, null);
                });

                if (value is ProcessorTypeDescription)
                    LoadProcessorParameters(_objects.Cast<ContentItem>().ToList());
            }
        }

        private void LoadProcessorParameters(List<ContentItem> objects)
        {
            foreach (var p in objects[0].Processor.Properties)
            {
                if (!p.Browsable)
                    continue;

                object value = objects[0].ProcessorParams[p.Name];
                foreach (ContentItem o in objects)
                {
                    if (value == null || !value.Equals(o.ProcessorParams[p.Name]))
                    {
                        value = null;
                        break;
                    }
                }

                _propertyTable.AddEntry("Processor Parameters", p.DisplayName, value, p.Type, true, val =>
                {
                    foreach (var obj in objects)
                        obj.ProcessorParams[p.Name] = val;
                });
            }
        }
    }
}
