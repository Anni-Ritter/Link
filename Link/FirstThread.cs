using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Link
{
    public class FirstThread
    {
        private Semaphore _sendSemaphore;
        private Semaphore _receiveSemaphore;
        private string _sendMessage;
        private PostToSecondWT _post;
        private BitArray _receivedMessage;
        private Encoding _encoding;

        private static int i = 0;
        private static bool[][] result;

        public FirstThread(ref Semaphore sendSemaphore, ref Semaphore receiveSemaphore, string sendMessege, Encoding encoding)
        {
            _sendSemaphore = sendSemaphore;
            _receiveSemaphore = receiveSemaphore;
            _sendMessage = sendMessege;
            _encoding = encoding;
        }
        public void FirstThreadMain(object obj)
        {
            var bits = new BitArray(_encoding.GetBytes(_sendMessage));
            _post = (PostToSecondWT)obj;
            ConsoleHelper.WriteToConsole("1 поток", "Начинаю работу.Готовлю данные для передачи.");

            bool[] values = new bool[bits.Count];
            for (int m = 0; m < bits.Count; m++)
            {
                values[m] = bits[m];
            }
            int j = 0;
            result = values.GroupBy(s => j++ / StaticFunction.PackLength).Select(g => g.ToArray()).ToArray();

            SetData();
        }

        public void ReceiveData(BitArray data) 
        {
            _receivedMessage = data;
        }

        private void SetData()
        {
            Pack pack;

            if (result.Length > i)
            {
                int checkSum = 0;
                for (int p = 0; p < result[i].Length; p++)
                {
                    if (p % 5 == 0)
                    {
                        checkSum += result[i][p] == false ? 0 : 1;
                    }
                }

                pack = new Pack(i, new BitArray(result[i]), checkSum, result[i].Length); 
                _post(new BitArray(StaticFunction.SerializeObject(pack)));
                _sendSemaphore.Release();

                ConsoleHelper.WriteToConsole("1 поток", $"Передан {i} кадр");
                ConsoleHelper.WriteToConsole("1 поток", "Жду результата");
                i += 1;
                _receiveSemaphore.WaitOne(); 
                SetData();
            }
            else
            {
                pack = new Pack(i, new BitArray(BitConverter.GetBytes(400)), 0, -1);
                _post(new BitArray(StaticFunction.SerializeObject(pack)));
                _sendSemaphore.Release();

                ConsoleHelper.WriteToConsole("1 поток", $"Передан завершающий кадр");
                ConsoleHelper.WriteToConsole("1 поток", "Завершаю работу");
            }
        }

    }
}
