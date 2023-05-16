using System;
using System.Collections.Generic;
using NetFwTypeLib;
using System.Net;

public enum Action
{ 
    BLOCK, 
    UNBLOCK
}

namespace WindowsDigitalSafety.Content_Filter
{
    public static class FirewallManager
    {
        static List<string> GetIPAddresses(string hostname)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(hostname);
            List<string> addressList = new List<string>();

            foreach (IPAddress address in addresses)
            {
                addressList.Add(address.ToString());
            }

            return addressList;
        }
        //static void Main(string[] args)
        //{
        //    ManageWebsite("www.apple.com", Action.UNBLOCK);

        //    //BlockApp("Notepad");
        //}

        public static void ManageWebsite(string hostname, Action action)
        {
            string website = hostname;
            List<string> ipAddresses = GetIPAddresses(website);
            const int maxAddressesPerRule = 100;

            var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            for (int i = 0; i < ipAddresses.Count; i += maxAddressesPerRule)
            {
                var rule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

                int addressesToAdd = Math.Min(maxAddressesPerRule, ipAddresses.Count - i);
                string addresses = string.Join(",", ipAddresses.GetRange(i, addressesToAdd));

                rule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                rule.Description = $"Block access to {website}";
                rule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
                rule.Enabled = true;
                rule.InterfaceTypes = "All";
                rule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_ANY;
                rule.RemoteAddresses = addresses;
                rule.Name = $"Block access to {website} ({i / maxAddressesPerRule + 1})";

                switch (action)
                {
                    case Action.BLOCK:
                        firewallPolicy.Rules.Add(rule);
                        break;
                    case Action.UNBLOCK:
                        firewallPolicy.Rules.Remove(rule.Name);
                        break;
                }
            }
        }

        static void BlockApp(string appName)
        {
            // Get the Windows Firewall object
            Type typeNetFwMgr = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
            INetFwMgr mgrInstance = (INetFwMgr)Activator.CreateInstance(typeNetFwMgr);

            // Get the current profile
            INetFwProfile currentProfile = mgrInstance.LocalPolicy.CurrentProfile;

            // Get the list of globally blocked applications
            INetFwAuthorizedApplications authorizedApps = currentProfile.AuthorizedApplications;

            // Block the app
            INetFwAuthorizedApplication app = (INetFwAuthorizedApplication)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwAuthorizedApplication"));
            app.Name = appName;
            app.ProcessImageFileName = "C:\\Windows\\System32\\notepad.exe";
            app.Enabled = false; // Set Enabled to false to block the app

            authorizedApps.Add(app);
            int a = 0;
            return;
        }
    }
}


