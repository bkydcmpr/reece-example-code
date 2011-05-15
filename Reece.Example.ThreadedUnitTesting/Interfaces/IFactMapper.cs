using System.Collections;
using Reece.Example.ThreadedUnitTesting.Objects;

namespace Reece.Example.ThreadedUnitTesting.Interfaces
{
    public interface IFactMapper : IThread
    {
        void CalcProductionStopsAsync(Queue factQueue, Queue productionQueue, SignalWork callBack);
    }
}