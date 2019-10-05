using System;
public class InternetAvailability
{
    public bool? IsConnected { get; private set; }
    private bool Ping()
    {
        System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
        System.Net.NetworkInformation.PingReply reply;
        try
        {
            reply = pingSender.Send("Google.com");
        }
        catch (Exception) { return false; }
        if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
            return true;

        return false;
    }
    public void CheckConnection()
    {
        IsConnected = Ping();
    }
    public InternetAvailability()
    {
        IsConnected = null;
    }
}
