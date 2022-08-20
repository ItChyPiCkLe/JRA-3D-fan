using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRA
{
    internal class ConnectionMonitor
    {
        public void checkOnline()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                //Do your stuffs when network available

            }
            else
            {
                //Do stuffs when network not available

            }
        }





    }
}
