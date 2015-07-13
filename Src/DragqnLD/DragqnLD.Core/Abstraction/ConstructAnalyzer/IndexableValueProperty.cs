namespace DragqnLD.Core.Abstraction.ConstructAnalyzer
{
    public class IndexableValueProperty : IndexableProperty
    {
        //can be filled only by inspecting data (the predicate doesn't contain array info)
        ValuePropertyType? Type = null;

        public IndexableValueProperty(string abbreviatedName, string fullName) : base(abbreviatedName, fullName)
        {
        }
    }
}