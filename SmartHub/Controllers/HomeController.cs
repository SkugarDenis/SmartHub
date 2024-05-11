using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartHub.Controllers.Api.Request;
using SmartHub.DataContext;
using SmartHub.Models;
using System.Diagnostics;

namespace SmartHub.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataDbContext _dataDbContext;


        public HomeController(ILogger<HomeController> logger, DataDbContext dataDbContext)
        {
            _logger = logger;
            _dataDbContext = dataDbContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize]
        public async Task<IActionResult> Control()
        {
            var model = new ControlViewModel();

            var devices = await _dataDbContext.Devices
                .Include(d => d.GroupDevices)
                    .ThenInclude(gd => gd.GroupEntity)
                .Include(d => d.Interfaces)
                .AsNoTracking()
                .ToListAsync();

            model.Devices = devices;

            var groupItems = await _dataDbContext.GroupEntities
                .Select(g => new GroupItem { id = g.Id, name = g.Name })
                .ToListAsync();

            model.GroupItems = groupItems;

            return View(model);
        }
    }
}