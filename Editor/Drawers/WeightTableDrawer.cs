using LordBreakerX.EditorUtilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LordBreakerX.Tables
{
    [CustomPropertyDrawer(typeof(WeightTable<>), true)]
    public class WeightTableDrawer : PropertyDrawer
    {
        private List<SerializedProperty> _entries;

        private SerializedProperty _entriesProperty;

        private SerializedProperty _totalWieghtProperty;

        private ListView _entriesListView;

        private EditorPanel _propertiesPanel;

        protected virtual string ValueLabel { get => "Value"; }

        protected virtual string WeightLabel { get => "Weight"; }

        protected virtual string EntryName { get => "Entry"; }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _entries = new List<SerializedProperty>();
            _entriesProperty = property.FindPropertyRelative("_weightedEntries");
            _totalWieghtProperty = property.FindPropertyRelative("_totalWeight");

            VisualElement root = new VisualElement();

            TwoPaneSplitView splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);

            splitView.Add(CreateEntriesPanel(property));

            _propertiesPanel = new EditorPanel(GetPropertiesHeader());
            splitView.Add(_propertiesPanel);

            root.Add(splitView);

            return root;
        }

        private VisualElement CreateEntriesPanel(SerializedProperty property)
        {
            EditorPanel entriesPanel = new EditorPanel(GetEntriesHeader());

            VisualElement headerContainer = entriesPanel.Q<VisualElement>("header-container");

            Button addButton = new Button(AddEntry);
            addButton.text = "+";

            headerContainer.Add(addButton);

            for (int i = 0; i < _entriesProperty.arraySize; i++)
            {
                SerializedProperty entryProperty = _entriesProperty.GetArrayElementAtIndex(i);
                _entries.Add(entryProperty);
            }

            _entriesListView = new ListView();
            _entriesListView.itemsSource = _entries;
            _entriesListView.makeItem += MakeEntryItem;
            _entriesListView.bindItem += BindEntryItem;
            _entriesListView.selectionChanged += OnSelectionChanged;

            _entriesListView.AddManipulator(new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction($"Add {EntryName}", (a) => AddEntry());
            }));

            _entriesListView.Rebuild();

            entriesPanel.Add(_entriesListView);

            return entriesPanel;
        }

        private void OnSelectionChanged(IEnumerable<object> obj)
        {
            if (_entriesListView.selectedItem is SerializedProperty entryProperty)
            {
                _propertiesPanel.Clear();

                SerializedProperty valueProperty = entryProperty.FindPropertyRelative("_value");

                SerializedProperty weightProperty = entryProperty.FindPropertyRelative("_weight");

                VisualElement propertiesGUI = CreateEntryProperties(weightProperty, valueProperty);

                _propertiesPanel.Add(propertiesGUI);
            } 
        }

        private VisualElement MakeEntryItem()
        {
            Label label = new Label();
            label.style.unityTextAlign = TextAnchor.MiddleCenter;

            label.AddManipulator(new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction($"Remove {EntryName}", (a) =>
                {
                    if (label.userData is int index)
                    {
                        _entriesProperty.DeleteArrayElementAtIndex(index);

                        _entriesProperty.serializedObject.ApplyModifiedProperties();

                        UpdateListView();
                    }
                });
            }));

            return label;
        }

        private void BindEntryItem(VisualElement element, int index)
        {
            if (element is Label label)
            {
                label.text = GetEntryName(_entries[index], index);
            }

            element.userData = index;
        }

        protected virtual string GetEntryName(SerializedProperty entryProperty, int entryIndex)
        {
            SerializedProperty entryValueProperty = entryProperty.FindPropertyRelative("_value");
            SerializedProperty weightProperty = entryProperty.FindPropertyRelative("_weight");

            if (entryValueProperty.propertyType == SerializedPropertyType.ObjectReference && entryValueProperty.objectReferenceValue != null)
            {
                return entryValueProperty.objectReferenceValue.name;
            }
            else
            {
                return $"Element {entryIndex}";
            }
        }

        protected virtual string GetEntriesHeader() => "Weighted Entries"; 

        protected virtual string GetPropertiesHeader() => "Weighted Entry Properties";

        private void AddEntry()
        {
            _entriesProperty.arraySize++;

            _entriesProperty.serializedObject.ApplyModifiedProperties();

            UpdateListView();
        }

        protected void UpdateListView()
        {
            _entries.Clear();

            for (int i = 0; i < _entriesProperty.arraySize; i++)
            {
                SerializedProperty entryProperty = _entriesProperty.GetArrayElementAtIndex(i);
                _entries.Add(entryProperty);
            }

            _entriesListView.Rebuild();
        }

        protected virtual VisualElement CreateEntryProperties(SerializedProperty weightProperty, SerializedProperty valueProperty)
        {
            VisualElement entryRoot = new VisualElement();
            entryRoot.style.flexGrow = 1;

            var weightField = new PropertyField(weightProperty, WeightLabel);
            var valueField = new PropertyField(valueProperty, ValueLabel);

            weightField.RegisterValueChangeCallback((evt) =>
            {
                UpdateListView();
            });

            valueField.RegisterValueChangeCallback((evt) =>
            {
                UpdateListView();
            });

            entryRoot.Add(weightField);
            entryRoot.Add(valueField);

            entryRoot.Bind(weightProperty.serializedObject);

            return entryRoot;
        }


    }
}
