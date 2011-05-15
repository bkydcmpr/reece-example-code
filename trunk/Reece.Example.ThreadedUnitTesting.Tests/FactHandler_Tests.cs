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
    }
}
