namespace ALViN.Data.Utility;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ALViN.Data.Objects;
// Include your models and any other necessary namespaces

[Route("api/[controller]")]
[ApiController]
public class DatabaseController : ControllerBase
{
    public DatabaseController()
    {

    }
    // GET: api/devices
    [HttpGet]
    public ActionResult<List<Device>> GetAllDevices()
    {
        return StorageManager.GetDevices();
    }
    // Add other API methods (POST, PUT, DELETE) as needed
}