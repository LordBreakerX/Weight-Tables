using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LordBreakerX.Tables
{
    public abstract class TableEditorWindow<T> : EditorWindow where T : class
    {
        private ScriptableWeightTable<T> _currentTable;

        private VisualElement _tableGUI;

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            root.Add(CreateToolbar());

            root.Add(CreateTableGUI());
        }

        protected abstract ScriptableWeightTable<T> CreateDefaultTable();

        protected abstract List<ScriptableWeightTable<T>> GetTables();

        private VisualElement CreateToolbar()
        {
            Toolbar toolbar = new Toolbar();

            List<ScriptableWeightTable<T>> tables = GetTables();
            tables.Insert(0, null);

            PopupField<ScriptableWeightTable<T>> selectedTable = new PopupField<ScriptableWeightTable<T>>(tables, 0);
            selectedTable.formatListItemCallback = (item) =>
            {
                if (item == null) return "< None >";
                else return item.name;
            };

            selectedTable.formatSelectedValueCallback = (item) =>
            {
                if (item == null) return "None";
                else return item.name;
            };
            selectedTable.RegisterValueChangedCallback(evt =>
            {
                SetTable(evt.newValue);
            });

            toolbar.Add(selectedTable);

            return toolbar;
        }

        private VisualElement CreateTableGUI()
        {
            _tableGUI = new VisualElement();
            _tableGUI.style.flexGrow = 1;

            SetTable(null);

            return _tableGUI;
        }

        private void SetTable(ScriptableWeightTable<T> table)
        {
            _currentTable = table;

            if (_currentTable == null)
            {
                ScriptableWeightTable<T> defaultTable = CreateDefaultTable();

                UpdateTableGUI(defaultTable);

                _tableGUI.SetEnabled(false);
            }
            else
            {
                UpdateTableGUI(_currentTable);

                _tableGUI.SetEnabled(true);
            }
        }

        private void UpdateTableGUI(ScriptableWeightTable<T> table)
        {
            _tableGUI.Clear();

            SerializedObject tableObject = new SerializedObject(table);
            SerializedProperty tableProperty = tableObject.FindProperty("_table");

            PropertyField tableField = new PropertyField(tableProperty);

            tableField.Bind(tableObject);

            _tableGUI.Add(tableField);
        }
    }
}
