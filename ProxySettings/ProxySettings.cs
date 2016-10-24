using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace NetworkSettings
{
    /// <summary>
    /// ProxySettings class
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("0AFCC300-D606-4CEE-A4CF-0DCF4A3A9475")]
    [ProgId("ProxyWidgetActiveX.ProxySettings")]
    [ComDefaultInterface(typeof(IProxySettings))]
    public class ProxySettings : UserControl, IProxySettings, IObjectSafety
    {
        #region IProxySettings Members

        private const string AUTHENTICATION_TYPE_DIGEST = "Digest";

        public bool EnableProxy(string proxy, string exceptions)
        {
            return EnableProxy(proxy, exceptions.Split(';'));
        }

        public bool EnableProxyWithAuthentication(string proxy, string exceptions,
            string user, string pass)
        {
            return EnableProxy(proxy, exceptions.Split(';'), user, pass);
        }

        public bool EnableProxy(string proxy, string[] exceptions)
        {
            IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
            IWebProxy systemWebProxy = WebRequest.GetSystemWebProxy();

            IWebProxy myProxy = new WebProxy(proxy, true, exceptions);
            WebRequest.DefaultWebProxy = myProxy;

            return SystemProxySettings.SetProxy(proxy,
                String.Join(";", exceptions));
        }
        
        public bool EnableProxy(string proxy, string[] exceptions,
            string user, string pass)
        {
            IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
            IWebProxy systemWebProxy = WebRequest.GetSystemWebProxy();

            CredentialCache credentials = new CredentialCache();

            SecureString securePass = new SecureString();
            foreach (char c in pass)
            {
                securePass.AppendChar(c);
            }
            securePass.MakeReadOnly();

            NetworkCredential networkCredential = new NetworkCredential(user, securePass);
            credentials.Add(new Uri(proxy), AUTHENTICATION_TYPE_DIGEST, networkCredential);

            IWebProxy myProxy = new WebProxy(proxy, true, exceptions, credentials);
            WebRequest.DefaultWebProxy = myProxy;
            systemWebProxy.Credentials = myProxy.Credentials;

            return SystemProxySettings.SetProxy(proxy,
                String.Join(";", exceptions));
        }

        public bool DisableProxy()
        {
            WebRequest.GetSystemWebProxy().Credentials = null;
            WebRequest.DefaultWebProxy = null;
            return SystemProxySettings.UnsetProxy();
        }

        public bool IsProxyEnabled()
        {
            return SystemProxySettings.GetProxyEnabled();
        }

        public void DisplayNetworkInterfaces()
        {
            NetworkInformation.DisplayNetworkInterfaces(Console.Out);
        }

        public void DisplayTcpStatistics()
        {
            DisplayTcpStatistics(4);
            DisplayTcpStatistics(6);
        }

        public void DisplayTcpStatistics(int ipVersion)
        {
            switch (ipVersion)
            {
                case 4:
                    NetworkInformation.DisplayTcpStatistics(
                        NetworkInterfaceComponent.IPv4, Console.Out);
                    break;
                case 6:
                    NetworkInformation.DisplayTcpStatistics(
                        NetworkInterfaceComponent.IPv6, Console.Out);
                    break;
                default:
                    throw new ArgumentException("IP version");
            }
        }

        public void Ping(string host)
        {
            Connectivity.Ping(host, Console.Out);
        }

        public string Md5Digest(string pass)
        {
            byte[] passStream = MD5CryptoServiceProvider.Create().ComputeHash(
                new ASCIIEncoding().GetBytes(pass));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in passStream)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        #endregion

        #region IObjectSafety Members

        public enum ObjectSafetyOptions
        {
            INTERFACE_SAFE_FOR_UNTRUSTED_CALLER = 0x00000001,
            INTERFACE_SAFE_FOR_UNTRUSTED_DATA = 0x00000002,
            INTERFACE_USES_DISPEX = 0X00000004,
            INTERFACE_USES_SECURITY_MANAGER = 0x00000008
        }

        public int GetInterfaceSafetyOptions(ref Guid riid, out int pdwSupportedOptions, out int pdwEnabledOptions)
        {
            ObjectSafetyOptions m_options = ObjectSafetyOptions.INTERFACE_SAFE_FOR_UNTRUSTED_CALLER | ObjectSafetyOptions.INTERFACE_SAFE_FOR_UNTRUSTED_DATA;
            pdwSupportedOptions = (int)m_options;
            pdwEnabledOptions = (int)m_options;
            return 0;
        }

        public int SetInterfaceSafetyOptions(ref Guid rid, int dwOptionSetMask, int dwEnabledOptions)
        {
            return 0;
        }

        #endregion
    }
}
