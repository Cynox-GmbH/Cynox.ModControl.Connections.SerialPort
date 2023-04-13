# Cynox.ModControl.Connections.SerialPort
Provides a serial port connection for the [ModControl library](https://github.com/Cynox-GmbH/Cynox.ModControl).

## Example

```c#
var device = new ModControlDevice();
bool success = device.Connect(new SerialPortConnection("COM1", 9600));
```