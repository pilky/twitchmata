# BitsManager

A `BitsManager` allows you to manage bits events

## Respond to Bits

```
public override void ReceivedBits(Models.BitsRedemption bitsRedemption) {
	Debug.Log($"Received {bitsRedemption.BitsUsed} Bits from {bitsRedemption.User.DisplayName}");
}
```
