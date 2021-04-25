using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace Link
{
    public delegate void PostToFirstWT(BitArray message);
    public delegate void PostToSecondWT(BitArray message);
    public class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.WriteToConsole("Главный поток", "Введите Ваше сообщение...");
            var data = Console.ReadLine();
            Encoding encoding = Encoding.UTF8;

            Semaphore firstReceiveSemaphore = new Semaphore(0, 1);
            Semaphore secondReceiveSemaphore = new Semaphore(0, 1);

            FirstThread firstThread = new FirstThread(ref secondReceiveSemaphore, ref firstReceiveSemaphore, data, encoding);
            SecondThread secondThread = new SecondThread(ref firstReceiveSemaphore, ref secondReceiveSemaphore, encoding);

            Thread threadFirst = new Thread(new ParameterizedThreadStart(firstThread.FirstThreadMain));
            Thread threadSecond = new Thread(new ParameterizedThreadStart(secondThread.SecondThreadMain));

            PostToFirstWT postToFirstWt = new PostToFirstWT(firstThread.ReceiveData);
            PostToSecondWT postToSecondWt = new PostToSecondWT(secondThread.ReceiveData);

            threadFirst.Start(postToSecondWt);
            threadSecond.Start(postToFirstWt);

            Console.ReadLine();
        }
    }
}
