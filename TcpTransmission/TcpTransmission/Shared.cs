/*
 * Name:    Thomas Bloom
 * Class:   CP-252
 * Project: 252 Final Project
 */

using System;
using System.Collections.Generic;

namespace TcpTransmission
{
    /// <summary>
    /// An immutable event object to pass back data and exceptions from clients.
    /// </summary>
    /// <remarks>
    /// A null instance of this class indicates connection closure or failure.
    /// The properties Error or Message may be null; Check them before direct use.
    /// </remarks>
    public class IncomingMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Contains exception details if any.
        /// </summary>
        public readonly Exception Error;

        /// <summary>
        /// Data received from the remote machine.
        /// </summary>
        public readonly byte[] Message;

        /// <summary>
        /// Type of packet indicated by the sender. Default 90
        /// </summary>
        public readonly byte PacketType = 90;

        public IncomingMessageEventArgs(byte[] message)
        {
            Message = message;
        }
        public IncomingMessageEventArgs(byte[] message, byte packetType)
        {
            Message = message;
            PacketType = packetType;
        }
        public IncomingMessageEventArgs(Exception error)
        {
            Error = error;
        }
        public IncomingMessageEventArgs(byte[] message, Exception error)
        {
            Message = message;
            Error = error;
        }
        private IncomingMessageEventArgs(byte[] message, byte type, Exception error)
        {
            Message = message;
            PacketType = type;
            Error = error;
        }
    }

    /// <summary>
    /// Since Microsoft decided nobody would ever need to create an IPEndPoint from a string.
    /// </summary>
    /// <remarks>
    /// I'm not bitter at all.
    /// </remarks>
    public static class IPEndPointParser
    {

        /// <summary>
        /// Parses the specified endPoint string into an IPEndPoint. Supports IPv4 and IPv6
        /// </summary>
        /// <param name="endPoint">The end point string with the format IP:Port.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException">
        /// Invalid endpoint format. Valid format is IP:Port
        /// or
        /// Malformed IP address
        /// or
        /// Malformed port number
        /// </exception>
        public static System.Net.IPEndPoint Parse(string endPoint)
        {
            string[] endPointArr = endPoint.Split(':');
            if (endPointArr.Length < 2) throw new FormatException("Invalid endpoint format. Valid format is IP:Port");

            // Parse IP portion.
            System.Net.IPAddress ip;
            if (endPointArr.Length > 2)
            {
                // IPv6 parsing.
                if (!System.Net.IPAddress.TryParse(string.Join(":", endPointArr, 0, endPointArr.Length - 1), out ip))
                {
                    throw new FormatException("Malformed IP address");
                }
            }
            else
            {
                // IPv4 parsing.
                if (!System.Net.IPAddress.TryParse(endPointArr[0], out ip))
                {
                    throw new FormatException("Malformed IP address");
                }
            }

            // Parse port portion.
            int port;
            if (!int.TryParse(endPointArr[endPointArr.Length - 1], System.Globalization.NumberStyles.None, System.Globalization.NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Malformed port number");
            }
            return new System.Net.IPEndPoint(ip, port);
        }
    }
}