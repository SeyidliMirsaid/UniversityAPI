using Education.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Education.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet("repositories")]
        public IActionResult TestRepositories()
        {
            try
            {
                // UnitOfWork işləyirmi yoxlayaq
                var hasUsersRepo = _unitOfWork.MyUsers != null;
                var hasStudentsRepo = _unitOfWork.Students != null;

                return Ok(new
                {
                    Status = "Success",
                    Message = "All repositories are initialized",
                    Time = DateTime.UtcNow,
                    Repositories = new
                    {
                        Users = hasUsersRepo,
                        Students = hasStudentsRepo,
                        Teachers = _unitOfWork.Teachers != null,
                        Courses = _unitOfWork.Courses != null
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
