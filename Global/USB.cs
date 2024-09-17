using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constructor.Global
{
    internal class USB
    {

        //constructor
        public USB(string deviceID, string pnpDeviceID, string description)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
        }

        /// <summary>
        /// Device ID
        /// </summary>
        public string DeviceID { get; private set; }

        /// <summary>
        /// Pnp Device Id
        /// </summary>
        public string PnpDeviceID { get; private set; }

        /// <summary>
        /// Descripción del dispositivo o nombre
        /// </summary>
        public string Description { get; private set; }
    }
}
