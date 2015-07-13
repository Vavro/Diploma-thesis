namespace DragqnLD.Core.Abstraction.ConstructAnalyzer
{
    public interface IIndexableProperty
    {
        string AbbreviatedName { get; }
        string FullName { get; }
    }

    public abstract class IndexableProperty : IIndexableProperty
    {
        public IndexableProperty(string abbreviatedName, string fullName)
        {
            FullName = fullName;
            AbbreviatedName = abbreviatedName;
        }

        public string AbbreviatedName { get; private set; }
        public string FullName { get; private set; }
    }
}