using LordBreakerX.EditorUtilities;
using UnityEditor;
using UnityEngine.UIElements;

namespace LordBreakerX.Tables
{

    public class WeightTableEntriesPanel : ListPanelV2<WeightedEntry<object>>
    {
        private ListView _listView;


        public WeightTableEntriesPanel(string title) : base(title)
        {
            _listView = this.Q<ListView>();
            _listView.itemsChosen += OnEntryChosen;
        }

        private void OnEntryChosen(System.Collections.Generic.IEnumerable<object> chosenItems)
        {
            foreach (var item in chosenItems) 
            {
                if (item is WeightedEntry<object> weightedEntry)
                {

                }
            }
        }

        public override WeightedEntry<object> CopyElement(WeightedEntry<object> toCopy)
        {
            return new WeightedEntry<object>(toCopy.Value, toCopy.Weight);
        }

        protected override WeightedEntry<object> CreateDefaultElement()
        {
            return new WeightedEntry<object>();
        }

        protected override string GetElementName(WeightedEntry<object> element, int index)
        {
            if (element.Value is UnityEngine.Object obj)
            {
                return obj.name;
            }
            else
            {
                return $"Element {index}";
            }
        }
    }

    //[CustomPropertyDrawer(typeof(WeightTable<>), true)]
    public class WeightTableDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            WeightTableEntriesPanel entriesPanel = new WeightTableEntriesPanel("Weighted Entries");

            SerializedProperty listProperty = property.FindPropertyRelative("_weightedEntries");

            root.Add(entriesPanel);

            return root;
        }
    }
}
