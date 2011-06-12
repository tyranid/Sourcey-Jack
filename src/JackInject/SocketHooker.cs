/***********************************************************************

Sourcey Jack - A simple SOCKSifying application for Windows
Copyright (C) 2011 James Forshaw

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

***********************************************************************/

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using EasyHook;
using System.Threading;
using System.Diagnostics;
using System.Net;

namespace JackInject
{
    public class SocketHooker : EasyHook.IEntryPoint
    {
        class SocketInstance
        {
            public Socket socket;
            public bool nonBlocking;
            public bool ipv6;
        }

        Dictionary<IntPtr, SocketInstance> _sockets;
        LocalHook _connectHook;
        LocalHook _socketHook;
        LocalHook _ioctlHook;
        IPEndPoint _serverEndp;

        public SocketHooker(
            RemoteHooking.IContext InContext,
            IPEndPoint serverEndp)
        {
            _sockets = new Dictionary<IntPtr, SocketInstance>();
            _serverEndp = serverEndp;

            System.Diagnostics.Debug.WriteLine(String.Format("Server {0}", _serverEndp));
        }

        public void Run(
                RemoteHooking.IContext InContext,
                IPEndPoint serverEndp)
        {           
            try
            {
                _connectHook = LocalHook.Create(LocalHook.GetProcAddress("Ws2_32.dll", "connect"), new Dconnect(connect_Hooked), this);
                _connectHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
                _socketHook = LocalHook.Create(LocalHook.GetProcAddress("Ws2_32.dll", "socket"), new Dsocket(socket_Hooked), this);
                _socketHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
                _ioctlHook = LocalHook.Create(LocalHook.GetProcAddress("Ws2_32.dll", "WSAIoctl"), new DWSAIoctl(WSAIoctl_Hooked), this);
                _ioctlHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                RemoteHooking.WakeUpProcess();
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ExtInfo)
            {
                System.Diagnostics.Debug.WriteLine(ExtInfo.ToString());

                return;
            }

            System.Diagnostics.Debug.WriteLine("Run Succeeded");

            RemoteHooking.WakeUpProcess();           
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            SetLastError = true)]
        delegate int Dconnect(IntPtr s, IntPtr name, int namelen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            SetLastError = true)]
        delegate IntPtr Dsocket(int af, int type, int protocol);

         [UnmanagedFunctionPointer(CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            SetLastError = true)]
        delegate int DWSAIoctl(IntPtr s, uint dwIoControlCode, IntPtr lpvInBuffer, uint cbInBuffer,
            IntPtr lpvOutBuffer, uint cbOutBuffer, IntPtr lpcbBytesReturned, IntPtr lpOverlapped, IntPtr lpCompletionRoutine);
        
        [DllImport("Ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern int connect(IntPtr s, IntPtr name, int namelen);
        [DllImport("Ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr socket(int af, int type, int protocol);
        [DllImport("Ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern int WSAIoctl(IntPtr s, uint dwIoControlCode, IntPtr lpvInBuffer, uint cbInBuffer,
            IntPtr lpvOutBuffer, uint cbOutBuffer, IntPtr lpcbBytesReturned, IntPtr lpOverlapped, IntPtr lpCompletionRoutine);
        [DllImport("Ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern void WSASetLastError(int iError);

        [StructLayout(LayoutKind.Sequential)]
        struct SockAddrIn
        {
            public short sin_family;
            public ushort sin_port;
            public uint sin_addr;
            public uint dummy1;
            public uint dummy2;
        }

        static int SwapPortEndian(ushort port)
        {
            return (((port & 0xFF) << 8) | ((port & 0xFF00) >> 8));
        }

        private SocketInstance GetSocket(IntPtr handle)
        {
            SocketInstance ret = null;

            lock (_sockets)
            {
                if (_sockets.ContainsKey(handle))
                {
                    ret = _sockets[handle];
                }
            }

            return ret;
        }

        private SocketInstance SetSocket(bool ipv6)
        {
            SocketInstance i = new SocketInstance();
            i.socket = new Socket(ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            i.ipv6 = ipv6;

            lock (_sockets)
            {                            
                _sockets[i.socket.Handle] = i;
            }

            GC.SuppressFinalize(i.socket);

            return i;
        }
        
        static int connect_Hooked(IntPtr s, IntPtr name, int namelen)
        {
            System.Diagnostics.Debug.WriteLine("Connect called");

            SocketHooker socketHooker = (SocketHooker)HookRuntimeInfo.Callback;
            SocketInstance sock = socketHooker.GetSocket(s);

            if(sock != null)    
            {
                Debug.WriteLine("One of our sockets");

                try
                {
                    if (sock.socket.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (namelen == 16)
                        {
                            SockAddrIn sockAddrIn = (SockAddrIn)Marshal.PtrToStructure(name, typeof(SockAddrIn));
                            byte[] sockopt = new byte[4];
                            IPAddress addr = new IPAddress(BitConverter.GetBytes(sockAddrIn.sin_addr));                                

                            Debug.WriteLine(String.Format("{0} {1}", addr, SwapPortEndian(sockAddrIn.sin_port)));

                            try
                            {                                
                                sock.socket.Connect(socketHooker._serverEndp);
                                byte[] socksReq = new byte[9];
                                byte[] socksResp = new byte[8];
                                socksReq[0] = 4; // SOCKS v4
                                socksReq[1] = 1; // Make connection
                                Buffer.BlockCopy(BitConverter.GetBytes(sockAddrIn.sin_port), 0, socksReq, 2, 2);
                                Buffer.BlockCopy(BitConverter.GetBytes(sockAddrIn.sin_addr), 0, socksReq, 4, 4);

                                sock.socket.Send(socksReq);
                                if ((sock.socket.Receive(socksResp) != socksResp.Length) || (socksResp[1] != 0x5a))
                                {
                                    Debug.WriteLine("Invalid length or response");
                                    throw new SocketException(-1);
                                }

                                if (sock.nonBlocking)
                                {
                                    sock.socket.Blocking = false;                                         
                                }

                                return 0;
                            }
                            catch (SocketException se)
                            {
                                Debug.WriteLine(se.ToString());            
                                WSASetLastError(se.ErrorCode);
                                return -1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }  
            }
            else
            {
                Debug.WriteLine("Not one of our sockets");
            }            

            int ret = connect(s, name, namelen);

            Debug.WriteLine(String.Format("Connect returned {0}", ret));

            return ret;
        }

        const int AF_INET = 2;
        const int AF_INET6 = 23;
        const int SOCK_STREAM = 1;
        const int IPPROTO_TCP = 6;

        static IntPtr socket_Hooked(int af, int type, int protocol)
        {
            IntPtr ret = new IntPtr(-1);
            SocketHooker socketHooker = (SocketHooker)HookRuntimeInfo.Callback;

            try
            {
                if ((af == AF_INET) && (type == SOCK_STREAM) && ((protocol == 0) || (protocol == IPPROTO_TCP)))
                {
                    Debug.WriteLine("IPv4 Socket Created");
                    ret = socketHooker.SetSocket(false).socket.Handle;
                }
                else if ((af == AF_INET6) && (type == SOCK_STREAM) && ((protocol == 0) || (protocol == IPPROTO_TCP)))
                {
                    Debug.WriteLine("IPv6 Socket Created");
                    ret = ret = socketHooker.SetSocket(true).socket.Handle;
                }
                else
                {
                    ret = socket(af, type, protocol);
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex.ToString());
                WSASetLastError(ex.ErrorCode);
            }

            return ret;
        }

        static int WSAIoctl_Hooked(IntPtr s, uint dwIoControlCode, IntPtr lpvInBuffer, uint cbInBuffer,
            IntPtr lpvOutBuffer, uint cbOutBuffer, IntPtr lpcbBytesReturned, IntPtr lpOverlapped, IntPtr lpCompletionRoutine)
        {
            SocketHooker socketHooker = (SocketHooker)HookRuntimeInfo.Callback;
            SocketInstance sock = socketHooker.GetSocket(s);
            
            if((dwIoControlCode == 0x8004667E) && (sock != null) && (sock.nonBlocking == false))
            {
                // If this is a ioctl to change blocking mode we don't do it until the connection has been established
                if ((cbInBuffer == 4) && (lpvInBuffer != IntPtr.Zero))
                {
                    sock.nonBlocking = Marshal.ReadInt32(lpvInBuffer) != 0;

                    Debug.WriteLine(String.Format("Setting non-blocking {0}", sock.nonBlocking));
                }

                return 0;
            }
            
            return WSAIoctl(s, dwIoControlCode, lpvInBuffer, cbInBuffer, lpvOutBuffer, cbOutBuffer, lpcbBytesReturned, lpOverlapped, lpCompletionRoutine);
        }
    }
}
