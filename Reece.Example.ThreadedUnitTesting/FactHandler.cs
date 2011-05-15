using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Reece.Example.ThreadedUnitTesting.Interfaces;

namespace Reece.Example.ThreadedUnitTesting
{
    public class FactHandler : IFactHandler
    {
        public IFactReader FactReader { get; set; }
        public IMapWriter MapWriter { get; set; }
        public IFactMapper FactMapper { get; set; }
        public ILogger Log { get; set; }

        /// <summary>
        /// Create thread safe queues.
        /// </summary>
        private Queue _factQueue = Queue.Synchronized(new Queue());
        private Queue _mapQueue = Queue.Synchronized(new Queue());

        private IList<Tuple<Thread, IThread>> _threads = new List<Tuple<Thread, IThread>>();

        public void Stop()
        {
        }

        public void Signal()
        {
            throw new NotImplementedException();
        }

        public void ProcessFacts(object data)
        {
            Thread thread = new Thread(
                    (state) => FactReader.GetAllFactsAsync(_factQueue, Signal)
                );
            _threads.Add(new Tuple<Thread, IThread>(thread, FactReader));
            thread.Start();
            thread = new Thread(
                    (state) => MapWriter.PostProductionStopsAsync(_mapQueue, Signal)
                );
            _threads.Add(new Tuple<Thread, IThread>(thread, MapWriter));
            thread.Start();
            thread = new Thread(
                    (state) => FactMapper.CalcProductionStopsAsync(_factQueue, _mapQueue, Signal)
                );
            _threads.Add(new Tuple<Thread, IThread>(thread, FactMapper));
            thread.Start();
        }
    }
}
