using System.Collections;
using Reece.Example.ThreadedUnitTesting.Objects;

namespace Reece.Example.ThreadedUnitTesting.Interfaces
{
    public interface IFactReader : IThread
    {
        void GetAllFactsAsync(Queue destinationQueue, SignalWork callBack);
    }
}
