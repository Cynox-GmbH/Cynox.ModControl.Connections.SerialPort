using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace Cynox.ModControl.Connections
{
    /// <summary>
    /// <see cref="IModControlConnection"/> to be used for serial port connections.
    /// </summary>
    public class SerialPortConnection : IModControlConnection
    {
        private readonly SerialPort _Port;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="port">The serial port name (e.g. COM1)</param>
        /// <param name="baud">The desired baud rate (e.g. 9600, 38400, 115200). Default = 9600.</param>
        public SerialPortConnection(string port, int baud = 9600)
        {
            _Port = new SerialPort { Parity = Parity.None, DataBits = 8, StopBits = StopBits.One };
            _Port.PortName = port;
            _Port.BaudRate = baud;
            _Port.DataReceived += PortOnDataReceived;
        }

        /// <summary>
        /// Gets or sets the serial port name.
        /// </summary>
        public string PortName
        {
            get => _Port.PortName;
            set => _Port.PortName = value;
        }

        /// <summary>
        /// Gets or sets the baud rate.
        /// </summary>
        public int BaudRate
        {
            get => _Port.BaudRate;
            set => _Port.BaudRate = value;
        }

        private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            try
            {
                var buf = new byte[_Port.BytesToRead];
                _Port.Read(buf, 0, buf.Length);
                OnDataReceived(new List<byte>(buf));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("_com_DataReceived(): " + ex.Message);
            }
        }

        private void OnDataReceived(List<byte> data)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data));
        }

        #region IModControlConnection

        /// <inheritdoc />
        public event Action<object, DataReceivedEventArgs> DataReceived;

        /// <inheritdoc />
        public bool IsConnected => _Port != null && _Port.IsOpen;

        /// <inheritdoc />
        public bool Connect()
        {
            if (_Port == null)
            {
                return false;
            }

            if (!_Port.IsOpen)
            {
                _Port.Open();
            }

            return true;
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            try
            {
                _Port?.Close();
            }
            catch (IOException)
            {
                // Tritt auf, wenn ein virtueller Port entfernt wurde.
            }
        }

        /// <inheritdoc />
        public bool Send(List<byte> data)
        {
            try
            {
                _Port?.Write(data.ToArray(), 0, data.Count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        #endregion
    }
}
