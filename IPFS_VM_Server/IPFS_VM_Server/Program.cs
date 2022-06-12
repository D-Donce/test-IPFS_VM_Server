using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IPFS_VM_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // Task로 Socket 서버를 만듬(서버가 종료될 때까지 대기)    
            RunServer(10000).Wait();
        }

        static async Task RunServer(int port)
        {
            // Socket EndPoint 설정(서버의 경우는 Any로 설정하고 포트 번호만 설정한다.)  
            var ipep = new IPEndPoint(IPAddress.Any, port);
            // 소켓 인스턴스 생성    
            using (Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // 서버 소켓에 EndPoint 설정     
                server.Bind(ipep);
                // 클라이언트 소켓 대기 버퍼     
                server.Listen(20);
                // 콘솔 출력      
                Console.WriteLine($"Server Start... Listen port {ipep.Port}...");

                // server Accept를 Task로 병렬 처리(즉, 비동기를 만든다.)  
                var task = new Task(() =>
                {
                    while (true)
                    {
                        // 클라이언트로부터 접속 대기     
                        var client = server.Accept();
                        // 접속이 되면 Task로 병렬 처리    
                        new Task(() =>
                        {
                            // 클라이언트 EndPoint 정보 취득  
                            var ip = client.RemoteEndPoint as IPEndPoint;
                            // 콘솔 출력 - 접속 ip와 접속 시간      
                            Console.WriteLine($"Client : (From: {ip.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");
                            // 클라이언트로 접속 메시지를 byte로 변환하여 송신         
                            client.Send(Encoding.ASCII.GetBytes("Welcome server!\r\n>"));
                        });
                    }
                });

                // Task 실행
                task.Start();
                // 대기    
                await task;
            } 
        }

    }
}
