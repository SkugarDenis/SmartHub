using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHub.Controllers.Api.Request;
using SmartHub.DataContext;
using SmartHub.DataContext.DbModels;
using SmartHub.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SmartHub.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly DataDbContext _dataDbContext;
        public DeviceController(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }

        [HttpPost("CreateNewDevice")]
        public async Task<IActionResult> CreateNewDevice(CreateNewDeviceRequest request)
        {
            var isDeviceExist = await _dataDbContext.Devices
                .AnyAsync(device => device.Name.Equals(request.name.Trim()) || device.ExternalId.Equals(request.externalId));

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
    }
}
