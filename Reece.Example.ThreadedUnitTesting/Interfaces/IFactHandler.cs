namespace Reece.Example.ThreadedUnitTesting.Interfaces
{
    public interface IFactHandler : IThread
    {
        void ProcessFacts(object data);
    }
}
