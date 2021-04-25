using System;
using System.Collections;

namespace Link
{
    [Serializable]
    public class Pack
    {
        public Pack() { }

        public Pack(int id, BitArray bitArray, int checkSum, int useful)
        {
            Id = id;
            Data = bitArray;
            CheckSum = checkSum;
            UsefulData = useful;
        }

        public int Id { get; set; }
        public BitArray Data { get; set; }
        public int CheckSum { get; set; }
        public int UsefulData { get; set; }
    }
}
