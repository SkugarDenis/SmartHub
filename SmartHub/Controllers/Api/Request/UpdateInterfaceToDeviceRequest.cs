using SmartHub.DataContext.DbModels;

namespace SmartHub.Controllers.Api.Request
{
    public class UpdateInterfaceToDeviceRequest
    {
        public string externalDeviceId { get; set; }

        public List<DeviceIterfaceRequest> interfaces { get; set; }
    }
}


public enum ButtonTypesRemoteTV
{
    power,
    mute,
    volumeup,
    channelup,
    volumedown,
    channeldown,
    menu,
    source,
    arrow_drop_up,
    arrow_drop_down,
    arrow_left,
    arrow_right,
    exit,
    display,
    enter
}
