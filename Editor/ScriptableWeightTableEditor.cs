using LordBreakerX.EditorUtilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LordBreakerX.Tables
{
    public class WeightedEntryPanel<T> : ListPanelV2<WeightedEntry<T>> where T : class
    {
        public ListView EntriesListView { get; private set; }

        public ScriptableWeightTable<T> Table { get; private set;  }

        public WeightedEntryPanel(string title, ScriptableWeightTable<T> table) : base(title)
        {
            EntriesListView = this.Q<ListView>();
            Table = table;
        }

        public override WeightedEntry<T> CopyElement(WeightedEntry<T> toCopy)
        {
            return new WeightedEntry<T>(toCopy.Value, toCopy.Weight);
        }

        protected override WeightedEntry<T> CreateDefaultElement()
        {
            return new WeightedEntry<T>();
        }

        public override void AddElement(WeightedEntry<T> item)
        {
            base.AddElement(item);

            Table.UpdateTotalWeight();
            EntriesListView.Rebuild();

            EditorUtility.SetDirty(Table);
        }

        public override void RemoveElement(WeightedEntry<T> item)
        {
            base.RemoveElement(item);

            Table.UpdateTotalWeight();
            EntriesListView.Rebuild();

            EditorUtility.SetDirty(Table);
        }

        protected override string GetElementName(WeightedEntry<T> element, int index)
        {
            float percentage = (float)element.Weight / (float)Table.TotalWeight * 100;

            if (element.Value is Object elementObject)
            {
                return $"{elementObject.name} --- {percentage:F2}%";
            }
            else
            {
                return $"Element {index} --- {percentage:F2}%";
            }
        }
    }


    public abstract class ScriptableWeightTableEditor<T> : Editor where T : class
    {
        private int _entriesIndex = -1;

        private WeightedEntryPanel<T> _weightedEntries;

        private EditorPanel _entryProperties;

        public virtual string EntriesHeader { get => "Weighted Entries"; }

        public virtual string EntryPropertiesHeader { get => "Weighted Entry Properties"; }

        public virtual string ValueLabel { get => "Entry Value"; }

        public virtual string GeneralPropertiesLabel { get => "General Properties"; }

        public override VisualElement CreateInspectorGUI()
        {
            ScriptableWeightTable<T> table = (ScriptableWeightTable<T>)target;

            VisualElement root = new VisualElement();
            root.style.flexGrow = 1;
            root.style.flexDirection = FlexDirection.Row;

            _weightedEntries = new WeightedEntryPanel<T>(EntriesHeader, table);
            _weightedEntries.style.flexGrow = 1;
            _weightedEntries.style.minWidth = 200;

            _weightedEntries.SetItemsSource(table.Entries);

            _entryProperties = new EditorPanel(EntryPropertiesHeader);
            _entryProperties.style.flexGrow = 1;

            TwoPaneSplitView splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
            splitView.style.flexGrow = 1;
            splitView.style.minHeight = 700;

            splitView.Add(_weightedEntries);
            splitView.Add(_entryProperties);

            root.Add(splitView);

            _weightedEntries.EntriesListView.selectionChanged += OnEntrySelected;

            UpdateProperties();

            table.UpdateTotalWeight();
            _weightedEntries.EntriesListView.Rebuild();

            return root;
        }

        private void OnEntrySelected(System.Collections.Generic.IEnumerable<object> obj)
        {
            _entriesIndex = _weightedEntries.EntriesListView.selectedIndex;
            UpdateProperties();
        }

        public void UpdateProperties()
        {
            int entriesCount = _weightedEntries.EntriesListView.itemsSource.Count;

            _entriesIndex = Mathf.Clamp(_weightedEntries.EntriesListView.selectedIndex, -1, entriesCount);

            _entryProperties.Clear();

            Foldout generalProperties = _entryProperties.CreateDecoratedFoldout(GeneralPropertiesLabel);
            generalProperties.style.marginLeft = 20;
            generalProperties.style.marginRight = 20;

            Foldout valueFoldout = _entryProperties.CreateDecoratedFoldout(ValueLabel);
            valueFoldout.style.marginLeft = 20;
            valueFoldout.style.marginRight = 20;

            if (_entriesIndex >= 0)
            {
                _entryProperties.SetEnabled(true);

                SerializedProperty tableProperty = serializedObject.FindProperty("_table");
                SerializedProperty listProperty = tableProperty.FindPropertyRelative("_weightedEntries");
                SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(_entriesIndex);

                SerializedProperty valueProperty = elementProperty.FindPropertyRelative("_value");
                SerializedProperty weightProperty = elementProperty.FindPropertyRelative("_weight");

                PropertyField valueField = new PropertyField(valueProperty, "Value");
                valueField.RegisterValueChangeCallback((evt) =>
                {
                    _weightedEntries.EntriesListView.Rebuild();
                    EditorUtility.SetDirty(target);
                });

                PropertyField weightField = new PropertyField(weightProperty, "Weight");
                weightField.RegisterValueChangeCallback((evt) =>
                {
                    _weightedEntries.Table.UpdateTotalWeight();
                    _weightedEntries.EntriesListView.Rebuild();
                    EditorUtility.SetDirty(target);
                });

                valueField.Bind(serializedObject);
                weightField.Bind(serializedObject);

                valueFoldout.Add(valueField);
                generalProperties.Add(weightField);
            }
            else
            {
                _entryProperties.SetEnabled(false);
            }
        }
    }
}
