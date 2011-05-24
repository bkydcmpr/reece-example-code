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
        private object _lock = new object();
        private EventWaitHandle _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private volatile bool _running = true;

        public void Stop()
        {
            lock ( _lock )
            {
                while (_threads.Count > 0 )
                {
                    _threads[0].Item2.Stop();
                    _threads[0].Item1.Join();
                    _threads.RemoveAt(0);
                }
                _running = false;
                _waitHandle.Set();
            }
        }

        public void Signal()
        {
            foreach (var thread in _threads)
            {
                thread.Item2.Signal();
            }
        }

        public void ProcessFacts(object data)
        {
            lock (_lock)
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
            while (_running)
            {
                _waitHandle.WaitOne();
            }
        }
    }
}
