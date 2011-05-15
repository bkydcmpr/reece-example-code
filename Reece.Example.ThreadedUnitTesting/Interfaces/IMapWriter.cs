using System.Collections;
using Reece.Example.ThreadedUnitTesting.Objects;

namespace Reece.Example.ThreadedUnitTesting.Interfaces
{
    public interface IMapWriter : IThread
    {
        void PostProductionStopsAsync(Queue sourceQueue, SignalWork callBack);
    }
}