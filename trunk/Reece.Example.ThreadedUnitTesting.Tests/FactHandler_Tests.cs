using System.Collections;
using System.Threading;
using Moq;
using NUnit.Framework;
using Reece.Example.ThreadedUnitTesting.Interfaces;
using Reece.Example.ThreadedUnitTesting.Objects;

namespace Reece.Example.ThreadedUnitTesting.Tests
{
    [TestFixture]
    public class FactHandler_Tests
    {
        private FactHandler target;
        private Mock<IFactReader> _readerMock;
        private Mock<IMapWriter> _writerMock;
        private Mock<IFactMapper> _mapperMock;
        private Mock<ILogger> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            target = new FactHandler();
            _mapperMock = new Mock<IFactMapper>();
            _loggerMock = new Mock<ILogger>();
            _writerMock = new Mock<IMapWriter>();
            _readerMock = new Mock<IFactReader>();
            target.FactReader = _readerMock.Object;
            target.MapWriter = _writerMock.Object;
            target.Log = _loggerMock.Object;
            target.FactMapper = _mapperMock.Object;
        }

        [Test]
        public void ProcessFacts_StartsThreadedFactReader_Test()
        {
            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            Thread readerThread = null;

            _readerMock.Setup(x => x.GetAllFactsAsync(It.IsAny<Queue>(), It.IsAny<SignalWork>()))
                .Callback(() =>
                {
                    readerThread = Thread.CurrentThread;
                    waitHandle.Set();
                });

            Thread targetThread = new Thread(target.ProcessFacts);
            targetThread.Start();
            waitHandle.WaitOne(4000);
            target.Stop();
            targetThread.Join();
            readerThread.Join();
            Assert.AreNotEqual(targetThread.ManagedThreadId, readerThread.ManagedThreadId);
        }

        [Test]
        public void ProcessFacts_StartsMapWriter_Test()
        {
            EventWaitHandle waitHandle = new EventWaitHandle(false,EventResetMode.AutoReset);
            Thread writerThread = null;

            _writerMock.Setup(x => x.PostProductionStopsAsync(It.IsAny<Queue>(), It.IsAny<SignalWork>()))
                .Callback(() =>
                              {
                                  writerThread = Thread.CurrentThread;
                                  waitHandle.Set();
                              });

            Thread targetThread = new Thread(target.ProcessFacts);
            targetThread.Start();
            waitHandle.WaitOne(4000);
            target.Stop();
            targetThread.Join();
            writerThread.Join();
            Assert.AreNotEqual(targetThread.ManagedThreadId, writerThread.ManagedThreadId);
        }

        [Test]
        public void ProcessFacts_StartsFactMapper_Test()
        {
            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            Thread mapperThread = null;

            _mapperMock.Setup(x => x.CalcProductionStopsAsync(It.IsAny<Queue>(),It.IsAny<Queue>(),It.IsAny<SignalWork>()))
                .Callback(() =>
                {
                    mapperThread = Thread.CurrentThread;
                    waitHandle.Set();
                });

            Thread targetThread = new Thread(target.ProcessFacts);
            targetThread.Start();
            waitHandle.WaitOne(4000);
            target.Stop();
            targetThread.Join();
            mapperThread.Join();
            Assert.AreNotEqual(targetThread.ManagedThreadId, mapperThread.ManagedThreadId);
        }

        [Test]
        public void Stop_StopsReaderThread_Test()
        {
            EventWaitHandle startedHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            EventWaitHandle stopHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            int calledCount = 0;

            _readerMock.Setup(x => x.GetAllFactsAsync(It.IsAny<Queue>(), It.IsAny<SignalWork>()))
                .Callback(() =>
                              {
                                  startedHandle.Set();
                                  stopHandle.WaitOne(5000);
                                  waitHandle.Set();
                              });

            _readerMock.Setup(x => x.Stop())
                .Callback(() =>
                              {
                                  ++calledCount;
                                  stopHandle.Set();
                              });
            Thread targetThread = new Thread(target.ProcessFacts);
            targetThread.Start();
            startedHandle.WaitOne(1000);
            target.Stop();
            waitHandle.WaitOne(4000);
            targetThread.Join();
            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void ProcessFacts_RunsUntilStopIsCalled_Test()
        {
            EventWaitHandle startedHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            EventWaitHandle stopHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

            _readerMock.Setup(x => x.GetAllFactsAsync(It.IsAny<Queue>(), It.IsAny<SignalWork>()))
                .Callback(() =>
                {
                    startedHandle.Set();
                    stopHandle.WaitOne(15000);
                });

            _readerMock.Setup(x => x.Stop())
                .Callback(() => stopHandle.Set());
            Thread targetThread = new Thread(target.ProcessFacts);
            targetThread.Start();
            startedHandle.WaitOne(1000);
            Thread.Sleep(1000);
            Assert.IsTrue(targetThread.IsAlive);
            target.Stop();
            targetThread.Join();
        }

        [Test]
        public void Signal_SignalsAllThreads_Test()
        {
            object locker = new object();
            int counter = 0;

            EventWaitHandle writeStop = new EventWaitHandle(false, EventResetMode.AutoReset);
            _writerMock.Setup(x => x.PostProductionStopsAsync(It.IsAny<Queue>(), It.IsAny<SignalWork>()))
                .Callback(() =>
                              {
                                  lock (locker)
                                  {
                                      ++counter;
                                      Monitor.Pulse(locker);
                                  }
                                  writeStop.WaitOne(30000);
                              });
            _writerMock.Setup(x => x.Stop())
                .Callback(() => writeStop.Set());
            EventWaitHandle mapperStop = new EventWaitHandle(false, EventResetMode.AutoReset);
            _mapperMock.Setup(x => x.CalcProductionStopsAsync(It.IsAny<Queue>(), It.IsAny<Queue>(), It.IsAny<SignalWork>()))
                .Callback(() =>
                {
                    lock (locker)
                    {
                        ++counter;
                        Monitor.Pulse(locker);
                    }
                    mapperStop.WaitOne(30000);
                });
            _mapperMock.Setup(x => x.Stop())
                .Callback(() => mapperStop.Set());
            EventWaitHandle readStop = new EventWaitHandle(false, EventResetMode.AutoReset);
            _readerMock.Setup(x => x.GetAllFactsAsync(It.IsAny<Queue>(), It.IsAny<SignalWork>()))
                .Callback(() =>
                {
                    lock (locker)
                    {
                        ++counter;
                        Monitor.Pulse(locker);
                    }
                    readStop.WaitOne(30000);
                });
            _readerMock.Setup(x => x.Stop())
                .Callback(() => readStop.Set());

            int calledCount = 0;

            _readerMock.Setup(x => x.Signal())
                .Callback(() =>
                              {
                                  lock (locker)
                                  {
                                      calledCount++;
                                      Monitor.Pulse(locker);
                                  }
                              });
            _mapperMock.Setup(x => x.Signal())
                .Callback(() =>
                {
                    lock (locker)
                    {
                        calledCount++;
                        Monitor.Pulse(locker);
                    }
                });
            _writerMock.Setup(x => x.Signal())
                .Callback(() =>
                {
                    lock (locker)
                    {
                        calledCount++;
                        Monitor.Pulse(locker);
                    }
                });

            Thread targetThread = new Thread(target.ProcessFacts);
            targetThread.Start();
            lock (locker)
            {
                while ( counter != 3)
                {
                    Monitor.Wait(locker);
                }
            }

            target.Signal();
            target.Stop();
            targetThread.Join();
            Assert.AreEqual(3, calledCount);
        }
    }
}
