using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RemoteExecution
{
    class Network
    {
        Socket socket,acp_socket;
        byte[] data;
        public string raw;

        public void init()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 6973));

            socket.Listen(100); 
            acp_socket = socket.Accept();
        }

        public void RecieveData()
        {
            data = new byte[acp_socket.SendBufferSize];
            try{
                int j = acp_socket.Receive(data); 
                byte[] adata = new byte[j];
                for (int i = 0; i < j; i++)         
                    adata[i] = data[i];             
                raw = Encoding.Default.GetString(adata);
            }
            catch (Exception)
            {
                raw = "";
            }
        }

        public void sendACK()
        {
            byte[] buffer = Encoding.ASCII.GetBytes("kk");

            Task.Run(() =>
            {
                acp_socket.Send(buffer);
            }
            );

        }

        public void Release()
        {
            acp_socket.Close();
            socket.Close();
        }

    }
}
