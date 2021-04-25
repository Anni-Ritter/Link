using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Link
{
    public class SecondThread
    {

        private Semaphore _sendSemaphore;
        private Semaphore _receiveSemaphore;
        private BitArray _receivedMessage;
		private Encoding _encoding;

		private static List<bool> bitArray = new List<bool>();

		public SecondThread(ref Semaphore sendSemaphore, ref Semaphore receiveSemaphore, Encoding encoding)
        {
            _sendSemaphore = sendSemaphore;
            _receiveSemaphore = receiveSemaphore;
            _encoding = encoding;
        }

        public void SecondThreadMain(Object obj)
        {
            ConsoleHelper.WriteToConsole("2 поток", "Начинаю работу.Жду передачи данных.");
            _receiveSemaphore.WaitOne();
            SetData();

        }

		private void SetData()
		{
			Pack item = (Pack)StaticFunction.DeserializeObject(StaticFunction.BitArrayToByteArray(_receivedMessage));

			ConsoleHelper.WriteToConsole("2 поток", $"Получен кадр #{item.Id}");

			var length = StaticFunction.BitArrayToByteArray(item.Data);
			
			if (BitConverter.ToInt32(length, 0) != 400)
			{
				
				var checkSum = 0;

				for (int i = 0; i < item.UsefulData; i++)
				{
					if (i % 5 == 0)
					{
						checkSum += item.Data[i] == false ? 0 : 1;
					}
				}

				if (checkSum == item.CheckSum)
				{
					for (int i = 0; i < item.UsefulData; i++)
						bitArray.Add(item.Data[i]);
				}
				else
				{
					ConsoleHelper.WriteToConsole("2 поток", "Ошибка. Завершаю работу");
				}
				_sendSemaphore.Release();
				_receiveSemaphore.WaitOne();
				SetData();

			}
			else
			{
				ConsoleHelper.WriteToConsole("2 поток", $"Полученные данные:  {_encoding.GetString(StaticFunction.BitArrayToByteArray(new BitArray(bitArray.ToArray())))}");
				ConsoleHelper.WriteToConsole("2 поток", "Завершаю работу");
			}
		}
		public void ReceiveData(BitArray array)
        {
            _receivedMessage = array;
        }
    }
}
