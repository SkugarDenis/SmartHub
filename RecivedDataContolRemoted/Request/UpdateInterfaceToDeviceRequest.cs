using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecivedDataContolRemoted.Request
{
    public class UpdateInterfaceToDeviceRequest
    {
        public string externalDeviceId { get; set; }

        public List<DeviceIterfaceRequest> interfaces { get; set; }
    }

    public class DeviceIterfaceRequest
    {
        public string name { get; set; }

        public string data { get; set; }
    }

    public enum DeviceType
    {
        None = 0,
        Switch = 1,
        RemoteController = 2
    }

    public enum DataType
    {
        Boolean,
        String,
        Int
    }

    public enum UserInterfaceType
    {
        SwitchBulbOneTab = 1,
        SwitchBulbTwoTab = 2,
        SwitchBulbThreeTab = 3,
        SwitchBulbFourTab = 4,
        SwitchSockcet = 5,
        RemoteTV = 6,
        RemoteAirConditioner = 7
    }
}
