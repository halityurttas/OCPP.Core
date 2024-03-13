using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OCPP.Core.Database;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OCPP.Core.Management.Controllers
{
    public partial class ApiController : BaseController
    {

        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> RemoteStart(string Id)
        {
            if (User != null && !User.IsInRole(Constants.AdminRoleName))
            {
                Logger.LogWarning("Reset: Request by non-administrator: {0}", User?.Identity?.Name);
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            if (!string.IsNullOrEmpty(Id))
            {
                try
                {
                    using (OCPPCoreContext dbContext = new OCPPCoreContext(this.Config))
                    {
                        if (dbContext.Operations.Where(m => m.ChargePointId == Id && m.OpAllowed == 0).Any())
                        {
                            return StatusCode(StatusCodes.Status304NotModified);
                        }

                        dbContext.Operations.Add(new Operation
                        {
                            ChargePointId = Id,
                            OpType = 1
                        });
                        dbContext.SaveChanges();
                    }
                }
                catch (System.Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

                return StatusCode(StatusCodes.Status200OK);
            }

            return StatusCode(StatusCodes.Status400BadRequest);
        }

        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> RemoteStop(string Id)
        {
            if (User != null && !User.IsInRole(Constants.AdminRoleName))
            {
                Logger.LogWarning("Reset: Request by non-administrator: {0}", User?.Identity?.Name);
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            if (!string.IsNullOrEmpty(Id))
            {
                try
                {
                    using (OCPPCoreContext dbContext = new OCPPCoreContext(this.Config))
                    {
                        if (dbContext.Operations.Where(m => m.ChargePointId == Id && m.OpAllowed == 0).Any())
                        {
                            return StatusCode(StatusCodes.Status304NotModified);
                        }

                        dbContext.Operations.Add(new Operation
                        {
                            ChargePointId = Id,
                            OpType = 2
                        });
                        dbContext.SaveChanges();
                    }
                }
                catch (System.Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

                return StatusCode(StatusCodes.Status200OK);
            }

            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }
}
