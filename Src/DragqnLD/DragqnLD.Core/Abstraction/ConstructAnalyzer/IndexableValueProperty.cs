namespace DragqnLD.Core.Abstraction.ConstructAnalyzer
{
    public class IndexableValueProperty : IIndexableProperty
    {
        //can be filled only by inspecting data (the predicate doesn't contain array info)
        public ValuePropertyType? Type { get; set; }
    }
}