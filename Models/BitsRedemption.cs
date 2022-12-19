using System;

namespace Twitchmata.Models {
    class BitsRedemption {
        int BitsUsed;
        int TotalBitsUsed;
        User? User; //can be null if anonymous
        DateTime RedeemedAt;
        string Message;
    }
}

