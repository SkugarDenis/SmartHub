using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartHub.Controllers.Api.Request;
using SmartHub.DataContext;
using SmartHub.DataContext.DbModels;
using SmartHub.Extensions;

namespace SmartHub.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly DataDbContext _dataDbContext;
        private readonly IHubContext<NotificationHub> _hubContext;
        public DeviceController(DataDbContext dataDbContext, IHubContext<NotificationHub> hubContext)
        {
            _dataDbContext = dataDbContext;
            _hubContext = hubContext;
        }

        [HttpPost("CreateNewDevice")]
        public async Task<IActionResult> CreateNewDevice(CreateNewDeviceRequest request)
        {
            var isDeviceExist = await _dataDbContext.Devices
                .AnyAsync(device => device.ExternalId.Equals(request.externalId)); //device.Name.Equals(request.name.Trim()) || 

            if (isDeviceExist)
            {
                return BadRequest("Device with the same name or external ID already exists.");
            }

            if (!request.interfaces.Any() || !request.groups.Any())
            {
                return BadRequest("Interfaces or groups are missing.");
            }

            var groups = await _dataDbContext.GroupEntities
                .Where(x => request.groups.Any(g => g.Equals(x.Name)))
                .ToListAsync();

            if (!groups.Any())
            {
                return BadRequest("None of the specified groups exist.");
            }

            var interfaces = request.interfaces.Select(x => new DeviceInterfaceItem()
            {
                Name = x.name,
                DataType = x.type.GetDataTypeForString(),
                Control = "default"
            }).ToList();

            var device = new Device()
            {
                ExternalId = request.externalId,
                Description = "default",
                Name = request.name,
                Interfaces = interfaces,
                Type = request.type,
                GroupDevices = groups.Select(group => new GroupDevice { GroupEntityId = group.Id }).ToList()
            };

            _dataDbContext.Devices.Add(device);
            await _dataDbContext.SaveChangesAsync();

            return Ok("Device created successfully.");
        }

        [HttpPost("DeleteDevice")]
        public IActionResult DeleteDevice(DeleteDeviceRequest request)
        {
            var device = _dataDbContext.Devices
                .Include(d => d.Interfaces)
                .Include(d => d.GroupDevices)
                .SingleOrDefault(d => d.Id == request.id);

            if (device == null)
            {
                return NotFound();
            }
            _dataDbContext.GroupDevices.RemoveRange(device.GroupDevices);
            _dataDbContext.RemoveRange(device.Interfaces);
            _dataDbContext.Devices.Remove(device);

            _dataDbContext.SaveChanges();

            return Ok($"Device with ID {request.id} and all related data has been deleted successfully.");
        }

        [HttpPost("ChangeInterfaceDevice")]
        public async Task<IActionResult> ChangeInterfaceDevice(ChangeInterfaceDeviceRequest request)
        {
            bool IsExistDevice = await _dataDbContext.Devices.AnyAsync(x => x.Id == request.DeviceId);

            if (!IsExistDevice)
            {
                return BadRequest();
            }


            if (request.DeviceType == DeviceType.Switch)
            {
                bool IsExistInterface = await _dataDbContext.Interfaces
                    .AnyAsync(x => x.Id == request.InterfaceId);

                if (!IsExistInterface)
                {
                    return BadRequest();
                }

                var interfaceItem = _dataDbContext.Interfaces.FirstOrDefault(x => x.Id == request.InterfaceId);

                interfaceItem.Control = request.data;

                await _dataDbContext.SaveChangesAsync();

                // отправить сигнал о перемене Interface
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", "hello");
            }
            else if (request.DeviceType == DeviceType.RemoteController)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", "hello");
                // отправить сигнал о перемене Interface
            }


            return Ok();
        }
    }
}
