namespace DragqnLD.Core.Abstraction.Query
{
    public enum QueryStatus
    {
        ReadyToRun = 0,
        LoadingSelectResult = 1,
        LoadingDocuments = 2,
        Loaded = 3
    }
}