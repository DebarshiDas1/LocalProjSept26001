using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LocalProjSept26001.Helpers;
using LocalProjSept26001.Services;

namespace LocalProjSept26001.Controllers
{
    /// <summary>
    /// The FileController class provides API endpoints for uploading and downloading files.
    /// </summary>
    [Route("api/file")]
    [Authorize]
    public class FileController : BaseApiController
    {
        private readonly IFileService _fileService;

        /// <summary>
        /// Initializes a new instance of the FileController class.
        /// </summary>
        /// <param name="fileService">fileService value to set.</param>
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Saves file
        /// </summary>
        /// <returns>Returns list of file path of saved files.</returns>
        [HttpPost]
        [Route("{entityName}/field/{fieldName}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> SaveFiles(string entityName, string fieldName, List<IFormFile> files)
        {
            var response = await _fileService.SaveFiles(TenantId, entityName, fieldName, files);
            return Ok(response);
        }
    }
}