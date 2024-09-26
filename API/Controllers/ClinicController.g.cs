using Microsoft.AspNetCore.Mvc;
using LocalProjSept26001.Models;
using LocalProjSept26001.Services;
using LocalProjSept26001.Entities;
using LocalProjSept26001.Filter;
using LocalProjSept26001.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Task = System.Threading.Tasks.Task;
using LocalProjSept26001.Authorization;

namespace LocalProjSept26001.Controllers
{
    /// <summary>
    /// Controller responsible for managing clinic related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting clinic information.
    /// </remarks>
    [Route("api/clinic")]
    [Authorize]
    public class ClinicController : BaseApiController
    {
        private readonly IClinicService _clinicService;

        /// <summary>
        /// Initializes a new instance of the ClinicController class with the specified context.
        /// </summary>
        /// <param name="iclinicservice">The iclinicservice to be used by the controller.</param>
        public ClinicController(IClinicService iclinicservice)
        {
            _clinicService = iclinicservice;
        }

        /// <summary>Adds a new clinic</summary>
        /// <param name="model">The clinic data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("Clinic", Entitlements.Create)]
        public async Task<IActionResult> Post([FromBody] Clinic model)
        {
            model.TenantId = TenantId;
            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = UserId;
            var id = await _clinicService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of clinics based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of clinics</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("Clinic", Entitlements.Read)]
        public async Task<IActionResult> Get([FromQuery] string filters, string searchTerm, int pageNumber = 1, int pageSize = 10, string sortField = null, string sortOrder = "asc")
        {
            List<FilterCriteria> filterCriteria = null;
            if (pageSize < 1)
            {
                return BadRequest("Page size invalid.");
            }

            if (pageNumber < 1)
            {
                return BadRequest("Page mumber invalid.");
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var result = await _clinicService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific clinic by its primary key</summary>
        /// <param name="id">The primary key of the clinic</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The clinic data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("Clinic", Entitlements.Read)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, string fields = null)
        {
            var result = await _clinicService.GetById( id, fields);
            return Ok(result);
        }

        /// <summary>Deletes a specific clinic by its primary key</summary>
        /// <param name="id">The primary key of the clinic</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        [UserAuthorize("Clinic", Entitlements.Delete)]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
            var status = await _clinicService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific clinic by its primary key</summary>
        /// <param name="id">The primary key of the clinic</param>
        /// <param name="updatedEntity">The clinic data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("Clinic", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] Clinic updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            updatedEntity.TenantId = TenantId;
            updatedEntity.UpdatedOn = DateTime.UtcNow;
            updatedEntity.UpdatedBy = UserId;
            var status = await _clinicService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific clinic by its primary key</summary>
        /// <param name="id">The primary key of the clinic</param>
        /// <param name="updatedEntity">The clinic data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("Clinic", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] JsonPatchDocument<Clinic> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = await _clinicService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}