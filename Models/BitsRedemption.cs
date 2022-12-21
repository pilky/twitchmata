using System;

namespace Twitchmata.Models {
    public struct BitsRedemption {
        public int BitsUsed;
        public int TotalBitsUsed;
        public User? User; //can be null if anonymous
        public DateTime RedeemedAt;
        public string Message;
    }
}

