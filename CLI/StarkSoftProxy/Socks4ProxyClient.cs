/*
 *  Authors:  Benton Stark
 * 
 *  Copyright (c) 2007-2009 Starksoft, LLC (http://www.starksoft.com) 
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Starksoft.Net.Proxy
{
    /// <summary>
    /// Socks4 connection proxy class. This class implements the Socks4 standard proxy protocol.
    /// </summary>
    /// <remarks>
    /// This class implements the Socks4 proxy protocol standard for TCP communciations.
    /// </remarks>
    public class Socks4ProxyClient : IProxyClient
    {
        const int WAIT_FOR_DATA_INTERVAL = 50;   // 50 ms
        const int WAIT_FOR_DATA_TIMEOUT = 15000; // 15 seconds
        const string PROXY_NAME = "SOCKS4";
        TcpClient? _tcpClient;

        string _proxyHost = "";
        ushort _proxyPort;
        string? _proxyUserId;

        /// <summary>
        /// Default Socks4 proxy port.
        /// </summary>
        internal const ushort SOCKS_PROXY_DEFAULT_PORT = 1080;
        /// <summary>
        /// Socks4 version number.
        /// </summary>
        internal const byte SOCKS4_VERSION_NUMBER = 4;
        /// <summary>
        /// Socks4 connection command value.
        /// </summary>
        internal const byte SOCKS4_CMD_CONNECT = 0x01;
        /// <summary>
        /// Socks4 bind command value.
        /// </summary>
        internal const byte SOCKS4_CMD_BIND = 0x02;
        /// <summary>
        /// Socks4 reply request grant response value.
        /// </summary>
        internal const byte SOCKS4_CMD_REPLY_REQUEST_GRANTED = 90;
        /// <summary>
        /// Socks4 reply request rejected or failed response value.
        /// </summary>
        internal const byte SOCKS4_CMD_REPLY_REQUEST_REJECTED_OR_FAILED = 91;
        /// <summary>
        /// Socks4 reply request rejected becauase the proxy server can not connect to the IDENTD server value.
        /// </summary>
        internal const byte SOCKS4_CMD_REPLY_REQUEST_REJECTED_CANNOT_CONNECT_TO_IDENTD = 92;
        /// <summary>
        /// Socks4 reply request rejected because of a different IDENTD server.
        /// </summary>
        internal const byte SOCKS4_CMD_REPLY_REQUEST_REJECTED_DIFFERENT_IDENTD = 93;

        /// <summary>
        /// Create a Socks4 proxy client object. The default proxy port 1080 is used.
        /// </summary>
        public Socks4ProxyClient() { }

        /// <summary>
        /// Creates a Socks4 proxy client object using the supplied TcpClient object connection.
        /// </summary>
        /// <param name="tcpClient">A TcpClient connection object.</param>
        public Socks4ProxyClient(TcpClient tcpClient)
        {
            ArgumentNullException.ThrowIfNull(tcpClient);
            _tcpClient = tcpClient;
        }

        /// <summary>
        /// Create a Socks4 proxy client object. The default proxy port 1080 is used.
        /// </summary>
        /// <param name="proxyHost">Host name or IP address of the proxy server.</param>
        /// <param name="proxyUserId">Proxy user identification information.</param>
        public Socks4ProxyClient(string proxyHost, string proxyUserId)
        {
            if (string.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException(nameof(proxyHost));
            ArgumentNullException.ThrowIfNull(proxyUserId);

            _proxyHost = proxyHost;
            _proxyPort = SOCKS_PROXY_DEFAULT_PORT;
            _proxyUserId = proxyUserId;
        }

        /// <summary>
        /// Create a Socks4 proxy client object.
        /// </summary>
        /// <param name="proxyHost">Host name or IP address of the proxy server.</param>
        /// <param name="proxyPort">Port used to connect to proxy server.</param>
        /// <param name="proxyUserId">Proxy user identification information.</param>
        public Socks4ProxyClient(string proxyHost, ushort proxyPort, string? proxyUserId)
        {
            if (string.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException(nameof(proxyHost));
            ArgumentNullException.ThrowIfNull(proxyUserId);

            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
            _proxyUserId = proxyUserId;
        }

        /// <summary>
        /// Create a Socks4 proxy client object. The default proxy port 1080 is used.
        /// </summary>
        /// <param name="proxyHost">Host name or IP address of the proxy server.</param>
        public Socks4ProxyClient(string proxyHost)
        {
            if (string.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException(nameof(proxyHost));

            _proxyHost = proxyHost;
            _proxyPort = SOCKS_PROXY_DEFAULT_PORT;
        }

        /// <summary>
        /// Create a Socks4 proxy client object.
        /// </summary>
        /// <param name="proxyHost">Host name or IP address of the proxy server.</param>
        /// <param name="proxyPort">Port used to connect to proxy server.</param>
        public Socks4ProxyClient(string proxyHost, ushort proxyPort)
        {
            if (string.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException(nameof(proxyHost));

            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
        }

        /// <summary>
        /// Gets or sets host name or IP address of the proxy server.
        /// </summary>
        public string ProxyHost
        {
            get => _proxyHost;
            set => _proxyHost = value;
        }

        /// <summary>
        /// Gets or sets port used to connect to proxy server.
        /// </summary>
        public ushort ProxyPort
        {
            get => _proxyPort;
            set => _proxyPort = value;
        }

        /// <summary>
        /// Gets String representing the name of the proxy. 
        /// </summary>
        /// <remarks>This property will always return the value 'SOCKS4'</remarks>
        public virtual string ProxyName => PROXY_NAME;

        /// <summary>
        /// Gets or sets proxy user identification information.
        /// </summary>
        public string? ProxyUserID
        {
            get => _proxyUserId;
            set => _proxyUserId = value;
        }

        /// <summary>
        /// Gets or sets the TcpClient object. 
        /// This property can be set prior to executing CreateConnection to use an existing TcpClient connection.
        /// </summary>
        public TcpClient? TcpClient
        {
            get => _tcpClient;
            set => _tcpClient = value;
        }

        /// <summary>
        /// Creates a TCP connection to the destination host through the proxy server
        /// host.
        /// </summary>
        /// <param name="destinationHost">Destination host name or IP address of the destination server.</param>
        /// <param name="destinationPort">Port number to connect to on the destination server.</param>
        /// <returns>
        /// Returns an open TcpClient object that can be used normally to communicate
        /// with the destination server
        /// </returns>
        /// <remarks>
        /// This method creates a connection to the proxy server and instructs the proxy server
        /// to make a pass through connection to the specified destination host on the specified
        /// port. 
        /// </remarks>
        public TcpClient CreateConnection(string destinationHost, ushort destinationPort)
        {
            if (string.IsNullOrEmpty(destinationHost))
                throw new ArgumentNullException(nameof(destinationHost));
            try
            {
                // if we have no connection, create one
                if (_tcpClient is null)
                {
                    if (string.IsNullOrEmpty(_proxyHost))
                        throw new ProxyException("ProxyHost property must contain a value.");
                    //  create new tcp client object to the proxy server
                    _tcpClient = new TcpClient();
                    // attempt to open the connection
                    _tcpClient.Connect(_proxyHost, _proxyPort);
                }
                // send connection command to proxy host for the specified destination host and port
                SendCommand(_tcpClient.GetStream(), SOCKS4_CMD_CONNECT, destinationHost, destinationPort, _proxyUserId ?? "");
                // return the open proxied tcp client object to the caller for normal use
                return _tcpClient;
            }
            catch (Exception ex)
            {
                throw new ProxyException($"Connection to proxy host {Utils.GetHost(_tcpClient)} on port {Utils.GetPort(_tcpClient)} failed.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<TcpClient> CreateConnectionAsync(string destinationHost, ushort destinationPort, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(destinationHost))
                throw new ArgumentNullException(nameof(destinationHost));
            try
            {
                // if we have no connection, create one
                if (_tcpClient is null)
                {
                    if (string.IsNullOrEmpty(_proxyHost))
                        throw new ProxyException("ProxyHost property must contain a value.");
                    //  create new tcp client object to the proxy server
                    _tcpClient = new TcpClient();
                    // attempt to open the connection
                    await _tcpClient.ConnectAsync(_proxyHost, _proxyPort, cancellationToken);
                }
                // send connection command to proxy host for the specified destination host and port
                await SendCommandAsync(_tcpClient.GetStream(), SOCKS4_CMD_CONNECT, destinationHost, destinationPort, _proxyUserId ?? "", cancellationToken);
                // return the open proxied tcp client object to the caller for normal use
                return _tcpClient;
            }
            catch (Exception ex)
            {
                throw new ProxyException($"Connection to proxy host {Utils.GetHost(_tcpClient)} on port {Utils.GetPort(_tcpClient)} failed.", ex);
            }
        }

        /// <summary>
        /// Sends a command to the proxy server.
        /// </summary>
        /// <param name="proxy">Proxy server data stream.</param>
        /// <param name="command">Proxy byte command to execute.</param>
        /// <param name="destinationHost">Destination host name or IP address.</param>
        /// <param name="destinationPort">Destination port number</param>
        /// <param name="userId">IDENTD user ID value.</param>
        internal virtual void SendCommand(NetworkStream proxy, byte command, string destinationHost, ushort destinationPort, string userId)
        {
            // PROXY SERVER REQUEST
            // The client connects to the SOCKS server and sends a CONNECT request when
            // it wants to establish a connection to an application server. The client
            // includes in the request packet the IP address and the port number of the
            // destination host, and userid, in the following format.
            //
            //        +----+----+----+----+----+----+----+----+----+----+....+----+
            //        | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL|
            //        +----+----+----+----+----+----+----+----+----+----+....+----+
            // # of bytes:	   1    1      2              4           variable       1
            //
            // VN is the SOCKS protocol version number and should be 4. CD is the
            // SOCKS command code and should be 1 for CONNECT request. NULL is a byte
            // of all zero bits.        

            byte[] destIp = GetIPAddressBytes(destinationHost);
            byte[] destPort = GetDestinationPortBytes(destinationPort);
            byte[] userIdBytes = Encoding.ASCII.GetBytes(userId);
            byte[] request = new byte[9 + userIdBytes.Length];

            //  set the bits on the request byte array
            request[0] = SOCKS4_VERSION_NUMBER;
            request[1] = command;
            destPort.CopyTo(request, 2);
            destIp.CopyTo(request, 4);
            userIdBytes.CopyTo(request, 8);
            request[8 + userIdBytes.Length] = 0;  // null (byte with all zeros) terminator for userId

            // send the connect request
            proxy.Write(request, 0, request.Length);

            // wait for the proxy server to respond
            WaitForData(proxy);

            // PROXY SERVER RESPONSE
            // The SOCKS server checks to see whether such a request should be granted
            // based on any combination of source IP address, destination IP address,
            // destination port number, the userid, and information it may obtain by
            // consulting IDENT, cf. RFC 1413. If the request is granted, the SOCKS
            // server makes a connection to the specified port of the destination host.
            // A reply packet is sent to the client when this connection is established,
            // or when the request is rejected or the operation fails. 
            //
            //        +----+----+----+----+----+----+----+----+
            //        | VN | CD | DSTPORT |      DSTIP        |
            //        +----+----+----+----+----+----+----+----+
            // # of bytes:	   1    1      2              4
            //
            // VN is the version of the reply code and should be 0. CD is the result
            // code with one of the following values:
            //
            //    90: request granted
            //    91: request rejected or failed
            //    92: request rejected becuase SOCKS server cannot connect to
            //        identd on the client
            //    93: request rejected because the client program and identd
            //        report different user-ids
            //
            // The remaining fields are ignored.
            //
            // The SOCKS server closes its connection immediately after notifying
            // the client of a failed or rejected request. For a successful request,
            // the SOCKS server gets ready to relay traffic on both directions. This
            // enables the client to do I/O on its connection as if it were directly
            // connected to the application server.

            // create an 8 byte response array  
            byte[] response = new byte[8];

            // read the resonse from the network stream
            proxy.Read(response, 0, 8);

            //  evaluate the reply code for an error condition
            if (response[1] != SOCKS4_CMD_REPLY_REQUEST_GRANTED)
                HandleProxyCommandError(response, destinationHost, destinationPort);
        }

        /// <summary>
        /// Sends a command to the proxy server asynchronously.
        /// </summary>
        /// <param name="proxy">Proxy server data stream.</param>
        /// <param name="command">Proxy byte command to execute.</param>
        /// <param name="destinationHost">Destination host name or IP address.</param>
        /// <param name="destinationPort">Destination port number</param>
        /// <param name="userId">IDENTD user ID value.</param>
        internal virtual async Task SendCommandAsync(NetworkStream proxy, byte command, string destinationHost, ushort destinationPort, string userId, CancellationToken cancellationToken)
        {
            byte[] destIp = GetIPAddressBytes(destinationHost);
            byte[] destPort = GetDestinationPortBytes(destinationPort);
            byte[] userIdBytes = Encoding.ASCII.GetBytes(userId);
            byte[] request = new byte[9 + userIdBytes.Length];
            //  set the bits on the request byte array
            request[0] = SOCKS4_VERSION_NUMBER;
            request[1] = command;
            destPort.CopyTo(request, 2);
            destIp.CopyTo(request, 4);
            userIdBytes.CopyTo(request, 8);
            request[8 + userIdBytes.Length] = 0;  // null (byte with all zeros) terminator for userId
            // send the connect request
            await proxy.WriteAsync(request, 0, request.Length, cancellationToken);
            // create an 8 byte response array  
            byte[] response = new byte[8];
            // read the resonse from the network stream
            await proxy.ReadAsync(response, 0, 8, cancellationToken);
            //  evaluate the reply code for an error condition
            if (response[1] != SOCKS4_CMD_REPLY_REQUEST_GRANTED)
                HandleProxyCommandError(response, destinationHost, destinationPort);
        }

        /// <summary>
        /// Translate the host name or IP address to a byte array.
        /// </summary>
        /// <param name="destinationHost">Host name or IP address.</param>
        /// <returns>Byte array representing IP address in bytes.</returns>
        internal byte[] GetIPAddressBytes(string destinationHost)
        {
            //  if the address doesn't parse then try to resolve with dns
            if (!IPAddress.TryParse(destinationHost, out IPAddress? ipAddr))
            {
                try
                {
                    ipAddr = Dns.GetHostEntry(destinationHost).AddressList[0];
                }
                catch (Exception ex)
                {
                    throw new ProxyException($"An error occurred while attempting to DNS resolve the host name {destinationHost}.", ex);
                }
            }
            // return address bytes
            return ipAddr.GetAddressBytes();
        }

        /// <summary>
        /// Translate the destination port value to a byte array.
        /// </summary>
        /// <param name="value">Destination port.</param>
        /// <returns>Byte array representing an 16 bit port number as two bytes.</returns>
        internal byte[] GetDestinationPortBytes(ushort value) => [(byte)(value / 256), (byte)(value % 256)];
        //internal byte[] GetDestinationPortBytes(ushort value)
        //{
            //byte[] array = [Convert.ToByte(value / 256), Convert.ToByte(value % 256)];
            //return array;
        //}

        /// <summary>
        /// Receive a byte array from the proxy server and determine and handle and errors that may have occurred.
        /// </summary>
        /// <param name="response">Proxy server command response as a byte array.</param>
        /// <param name="destinationHost">Destination host.</param>
        /// <param name="destinationPort">Destination port number.</param>
        internal void HandleProxyCommandError(byte[] response, string destinationHost, int destinationPort)
        {
            ArgumentNullException.ThrowIfNull(response);

            //  extract the reply code
            byte replyCode = response[1];
            //  extract the ip v4 address (4 bytes)
            byte[] ipBytes = new byte[4];
            for (int i = 0; i < 4; i++)
                ipBytes[i] = response[i + 4];
            //  convert the ip address to an IPAddress object
            IPAddress ipAddr = new IPAddress(ipBytes);
            //  extract the port number big endian (2 bytes)
            byte[] portBytes = [response[3], response[2]];
            ushort port = BitConverter.ToUInt16(portBytes, 0);
            // translate the reply code error number to human readable text
            string proxyErrorText = replyCode switch
            {
                SOCKS4_CMD_REPLY_REQUEST_REJECTED_OR_FAILED => "connection request was rejected or failed",
                SOCKS4_CMD_REPLY_REQUEST_REJECTED_CANNOT_CONNECT_TO_IDENTD => "connection request was rejected because SOCKS destination cannot connect to identd on the client",
                SOCKS4_CMD_REPLY_REQUEST_REJECTED_DIFFERENT_IDENTD => "connection request rejected because the client program and identd report different user-ids",
                _ => $"proxy client received an unknown reply with the code value '{replyCode}' from the proxy destination",
            };
            //  build the exeception message string
            string exceptionMsg = $"The {proxyErrorText} concerning destination host {destinationHost} port number {destinationPort}. The destination reported the host as {ipAddr} port {port}.";
            //  throw a new application exception 
            throw new ProxyException(exceptionMsg);
        }

        internal void WaitForData(NetworkStream stream)
        {
            int sleepTime = 0;
            while (!stream.DataAvailable)
            {
                Thread.Sleep(WAIT_FOR_DATA_INTERVAL);
                sleepTime += WAIT_FOR_DATA_INTERVAL;
                if (sleepTime > WAIT_FOR_DATA_TIMEOUT)
                    throw new ProxyException("A timeout while waiting for the proxy destination to respond.");
            }
        }
    }
}
