using System;
using System.Messaging;
using System.Threading;

namespace Reece.Exammple.MSMQ.Subscriber
{
    class Program
    {
        const string QueuePath = @".\Private$\ExampleQueue";

        private static volatile bool _running;
        private static MessageQueue _queue;

        static void Main(string[] args)
        {
            _running = true;

            _queue = new MessageQueue(QueuePath);

            ThreadPool.QueueUserWorkItem((state) =>
                                             {
                                                 while (_running)
                                                 {
                                                     try
                                                     {
                                                         var message = _queue.Receive(new TimeSpan(0, 0, 0, 1));
                                                         message.Formatter = new XmlMessageFormatter(new String[] {"System.String,mscorlib"});
                                                         Console.WriteLine(message.Body.ToString());
                                                     }
                                                     catch (Exception)
                                                     {
                                                     }
                                                 }
                                             }
                );

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();
            _running = false;
        }
    }
}
