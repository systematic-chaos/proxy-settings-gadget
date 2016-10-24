using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NetworkSettings
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("FADE2BEA-C64B-40B5-9CFA-2A37A36AF6A0")]
    public interface IProxySettings
    {
        bool EnableProxy(string proxy, string exceptions);

        bool EnableProxyWithAuthentication(string proxy, string exceptions,
            string user, string pass);

        bool DisableProxy();

        bool IsProxyEnabled();

        void DisplayNetworkInterfaces();

        void DisplayTcpStatistics();

        void DisplayTcpStatistics(int ipVersion);

        void Ping(string host);

        string Md5Digest(string pass);
    }
}
