using LocalProjSept26001.Models;
using LocalProjSept26001.Data;
using LocalProjSept26001.Filter;
using LocalProjSept26001.Entities;
using LocalProjSept26001.Logger;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using Task = System.Threading.Tasks.Task;

namespace LocalProjSept26001.Services
{
    /// <summary>
    /// The treatmentService responsible for managing treatment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting treatment information.
    /// </remarks>
    public interface ITreatmentService
    {
        /// <summary>Retrieves a specific treatment by its primary key</summary>
        /// <param name="id">The primary key of the treatment</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The treatment data</returns>
        Task<dynamic> GetById(Guid id, string fields);

        /// <summary>Retrieves a list of treatments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of treatments</returns>
        Task<List<Treatment>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new treatment</summary>
        /// <param name="model">The treatment data to be added</param>
        /// <returns>The result of the operation</returns>
        Task<Guid> Create(Treatment model);

        /// <summary>Updates a specific treatment by its primary key</summary>
        /// <param name="id">The primary key of the treatment</param>
        /// <param name="updatedEntity">The treatment data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Update(Guid id, Treatment updatedEntity);

        /// <summary>Updates a specific treatment by its primary key</summary>
        /// <param name="id">The primary key of the treatment</param>
        /// <param name="updatedEntity">The treatment data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Patch(Guid id, JsonPatchDocument<Treatment> updatedEntity);

        /// <summary>Deletes a specific treatment by its primary key</summary>
        /// <param name="id">The primary key of the treatment</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Delete(Guid id);
    }

    /// <summary>
    /// The treatmentService responsible for managing treatment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting treatment information.
    /// </remarks>
    public class TreatmentService : ITreatmentService
    {
        private readonly LocalProjSept26001Context _dbContext;
        private readonly IFieldMapperService _mapper;

        /// <summary>
        /// Initializes a new instance of the Treatment class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        /// <param name="mapper">mapper value to set.</param>
        public TreatmentService(LocalProjSept26001Context dbContext, IFieldMapperService mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Retrieves a specific treatment by its primary key</summary>
        /// <param name="id">The primary key of the treatment</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The treatment data</returns>
        public async Task<dynamic> GetById(Guid id, string fields)
        {
            var query = _dbContext.Treatment.AsQueryable();
            List<string> allfields = new List<string>();
            if (!string.IsNullOrEmpty(fields))
            {
                allfields.AddRange(fields.Split(","));
                fields = $"Id,{fields}";
            }
            else
            {
                fields = "Id";
            }

            string[] navigationProperties = ["DiagnosisId_Diagnosis"];
            foreach (var navigationProperty in navigationProperties)
            {
                if (allfields.Any(field => field.StartsWith(navigationProperty + ".", StringComparison.OrdinalIgnoreCase)))
                {
                    query = query.Include(navigationProperty);
                }
            }

            query = query.Where(entity => entity.Id == id);
            return _mapper.MapToFields(await query.FirstOrDefaultAsync(),fields);
        }

        /// <summary>Retrieves a list of treatments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of treatments</returns>/// <exception cref="Exception"></exception>
        public async Task<List<Treatment>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = await GetTreatment(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new treatment</summary>
        /// <param name="model">The treatment data to be added</param>
        /// <returns>The result of the operation</returns>
        public async Task<Guid> Create(Treatment model)
        {
            model.Id = await CreateTreatment(model);
            return model.Id;
        }

        /// <summary>Updates a specific treatment by its primary key</summary>
        /// <param name="id">The primary key of the treatment</param>
        /// <param name="updatedEntity">The treatment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Update(Guid id, Treatment updatedEntity)
        {
            await UpdateTreatment(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific treatment by its primary key</summary>
        /// <param name="id">The primary key of the treatment</param>
        /// <param name="updatedEntity">The treatment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Patch(Guid id, JsonPatchDocument<Treatment> updatedEntity)
        {
            await PatchTreatment(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific treatment by its primary key</summary>
        /// <param name="id">The primary key of the treatment</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Delete(Guid id)
        {
            await DeleteTreatment(id);
            return true;
        }
        #region
        private async Task<List<Treatment>> GetTreatment(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Treatment.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Treatment>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Treatment), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Treatment, object>>(Expression.Convert(property, typeof(object)), parameter);
                if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(lambda);
                }
                else if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderByDescending(lambda);
                }
                else
                {
                    throw new ApplicationException("Invalid sort order. Use 'asc' or 'desc'");
                }
            }

            var paginatedResult = await result.Skip(skip).Take(pageSize).ToListAsync();
            return paginatedResult;
        }

        private async Task<Guid> CreateTreatment(Treatment model)
        {
            _dbContext.Treatment.Add(model);
            await _dbContext.SaveChangesAsync();
            return model.Id;
        }

        private async Task UpdateTreatment(Guid id, Treatment updatedEntity)
        {
            _dbContext.Treatment.Update(updatedEntity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<bool> DeleteTreatment(Guid id)
        {
            var entityData = _dbContext.Treatment.FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Treatment.Remove(entityData);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task PatchTreatment(Guid id, JsonPatchDocument<Treatment> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Treatment.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Treatment.Update(existingEntity);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}