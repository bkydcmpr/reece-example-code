using System;
using System.Messaging;

namespace Reece.Example.MSMQ.Publisher
{
    class Program
    {
        const string QueuePath = @".\Private$\ExampleQueue";

        static void Main(string[] args)
        {
            var queue = MessageQueue.Exists(QueuePath) ? new MessageQueue(QueuePath) : MessageQueue.Create(QueuePath);

            int id = 0;

            Console.WriteLine("Type QUIT to exit.");
            string input;
            do
            {
                Console.Write("> ");
                input = Console.ReadLine();
                Message message = new Message();
                message.Body = input;
                message.Label = (++id).ToString();
                queue.Send(message);
            } while (input.ToLower() != "quit");
        }
    }
}
