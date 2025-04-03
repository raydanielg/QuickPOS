using System.Net.NetworkInformation;

public class NetworkService
{
    public bool IsInternetAvailable()
    {
        try
        {
            using var ping = new Ping();
            PingReply reply = ping.Send("www.google.com");
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }
}
