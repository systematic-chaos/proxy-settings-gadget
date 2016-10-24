using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace NetworkSettings
{
    class IEProxyModifier
    {
        // Notifies the system that the registry settings have been changed so that it
        // verifies the settings on the next call to InternetConnect.
        private const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        // Causes the proxy data to be reread from the registry for a handle.
        // No buffer is required.
        private const int INTERNET_OPTION_REFRESH = 37;
        // Sets or retrieves an INTERNET_PROXY structure that contains the
        // proxy data for an existing InternetOpen handle.
        private const int INTERNET_OPTION_PROXY = 38;
        private const string RegistryKeyPath = "HKEY_CURRENT_USER\\Software\\Microsoft\\"
                                            + "Windows\\CurrentVersion\\InternetSettings";
        // Initializes an application's use of the WinINet functions.
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr InternetOpen(string lpszAgent, int dwAccessType,
                    string lpszProxyName, string lpszProxyBypass, int dwFlags);
        // Closes a single Internet handle.
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool InternetCloseHandle(IntPtr hInternet);
        // Queries an Internet option on the specified handle.
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetQueryOption(IntPtr hInternet, uint dwOption,
                                    IntPtr lpBuffer, ref int lpdwBufferLength);
        // Sets an Internet option.
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetSetOption(IntPtr hInternet, int dwOption,
                                            IntPtr lpBuffer, int dwBufferLength);

        /// <summary>
        /// Access types supported by InternetOpen function.
        /// </summary
        public enum InternetOpenType
        {
            INTERNET_OPEN_TYPE_PRECONFIG = 0,
            INTERNET_OPEN_TYPE_DIRECT = 1,
            INTERNET_OPEN_TYPE_PROXY = 3,
        }

        /// <summary>
        /// Contains information that is supplied with the INTERNET_OPTION_PROXY value
        /// to get or set proxy information on a handle obtained from a call to
        /// the InternetOpen function.
        /// </summary>
        public struct INTERNET_PROXY_INFO
        {
            public InternetOpenType dwAccessType { get; set; }
            public string lpszProxy { get; set; }
            public string lpszProxyBypass { get; set; }
        }

        public static INTERNET_PROXY_INFO GetProxy()
        {
            INTERNET_PROXY_INFO proxyInfo = new INTERNET_PROXY_INFO();
            int bufferLength = 0;
            IntPtr buffer = IntPtr.Zero;
            Exception proxyModifierException = null;

            InternetQueryOption(IntPtr.Zero, INTERNET_OPTION_PROXY, IntPtr.Zero,
                ref bufferLength);
            try
            {
                buffer = Marshal.AllocHGlobal(bufferLength);
                if (InternetQueryOption(IntPtr.Zero, INTERNET_OPTION_PROXY, buffer,
                    ref bufferLength))
                {
                    // Getting the proxy details.
                    proxyInfo = (INTERNET_PROXY_INFO)
                        // Converting structure to IntPtr.
                        Marshal.PtrToStructure(buffer, typeof(INTERNET_PROXY_INFO));
                }
                else
                    throw proxyModifierException = new Win32Exception();
            }
            catch (Exception e)
            {
                proxyModifierException = e;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }

            if (proxyModifierException != null)
            {
                throw proxyModifierException;
            }

            return proxyInfo;
        }

        public static void LoadIEProxyModifier()
        {
            IntPtr hInternet = InternetOpen("Browser",
                (int)InternetOpenType.INTERNET_OPEN_TYPE_DIRECT, null, null, 0);
            if (IntPtr.Zero == hInternet)
            {
                return;
            }
        }

        public static void SetProxy(string proxyServer, bool proxyEnable, string proxyByPass)
        {
            // Setting the proxy details.
            Registry.SetValue(RegistryKeyPath, "ProxyServer", proxyServer);
            Registry.SetValue(RegistryKeyPath, "ProxyEnable",
                proxyEnable ? 1 : 0, RegistryValueKind.DWord);
            Registry.SetValue(RegistryKeyPath, "ProxyOverride", proxyByPass);

            // Forcing the OS to refresh the IE settings to reflect new proxy settings.
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED,
                                IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }
    }
}
