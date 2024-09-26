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
    /// The writeoffsService responsible for managing writeoffs related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting writeoffs information.
    /// </remarks>
    public interface IWriteOffsService
    {
        /// <summary>Retrieves a specific writeoffs by its primary key</summary>
        /// <param name="id">The primary key of the writeoffs</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The writeoffs data</returns>
        Task<dynamic> GetById(Guid id, string fields);

        /// <summary>Retrieves a list of writeoffss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of writeoffss</returns>
        Task<List<WriteOffs>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new writeoffs</summary>
        /// <param name="model">The writeoffs data to be added</param>
        /// <returns>The result of the operation</returns>
        Task<Guid> Create(WriteOffs model);

        /// <summary>Updates a specific writeoffs by its primary key</summary>
        /// <param name="id">The primary key of the writeoffs</param>
        /// <param name="updatedEntity">The writeoffs data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Update(Guid id, WriteOffs updatedEntity);

        /// <summary>Updates a specific writeoffs by its primary key</summary>
        /// <param name="id">The primary key of the writeoffs</param>
        /// <param name="updatedEntity">The writeoffs data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Patch(Guid id, JsonPatchDocument<WriteOffs> updatedEntity);

        /// <summary>Deletes a specific writeoffs by its primary key</summary>
        /// <param name="id">The primary key of the writeoffs</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Delete(Guid id);
    }

    /// <summary>
    /// The writeoffsService responsible for managing writeoffs related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting writeoffs information.
    /// </remarks>
    public class WriteOffsService : IWriteOffsService
    {
        private readonly LocalProjSept26001Context _dbContext;
        private readonly IFieldMapperService _mapper;

        /// <summary>
        /// Initializes a new instance of the WriteOffs class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        /// <param name="mapper">mapper value to set.</param>
        public WriteOffsService(LocalProjSept26001Context dbContext, IFieldMapperService mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Retrieves a specific writeoffs by its primary key</summary>
        /// <param name="id">The primary key of the writeoffs</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The writeoffs data</returns>
        public async Task<dynamic> GetById(Guid id, string fields)
        {
            var query = _dbContext.WriteOffs.AsQueryable();
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

            string[] navigationProperties = ["StatementId_Statement"];
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

        /// <summary>Retrieves a list of writeoffss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of writeoffss</returns>/// <exception cref="Exception"></exception>
        public async Task<List<WriteOffs>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = await GetWriteOffs(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new writeoffs</summary>
        /// <param name="model">The writeoffs data to be added</param>
        /// <returns>The result of the operation</returns>
        public async Task<Guid> Create(WriteOffs model)
        {
            model.Id = await CreateWriteOffs(model);
            return model.Id;
        }

        /// <summary>Updates a specific writeoffs by its primary key</summary>
        /// <param name="id">The primary key of the writeoffs</param>
        /// <param name="updatedEntity">The writeoffs data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Update(Guid id, WriteOffs updatedEntity)
        {
            await UpdateWriteOffs(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific writeoffs by its primary key</summary>
        /// <param name="id">The primary key of the writeoffs</param>
        /// <param name="updatedEntity">The writeoffs data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Patch(Guid id, JsonPatchDocument<WriteOffs> updatedEntity)
        {
            await PatchWriteOffs(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific writeoffs by its primary key</summary>
        /// <param name="id">The primary key of the writeoffs</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Delete(Guid id)
        {
            await DeleteWriteOffs(id);
            return true;
        }
        #region
        private async Task<List<WriteOffs>> GetWriteOffs(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.WriteOffs.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<WriteOffs>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(WriteOffs), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<WriteOffs, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private async Task<Guid> CreateWriteOffs(WriteOffs model)
        {
            _dbContext.WriteOffs.Add(model);
            await _dbContext.SaveChangesAsync();
            return model.Id;
        }

        private async Task UpdateWriteOffs(Guid id, WriteOffs updatedEntity)
        {
            _dbContext.WriteOffs.Update(updatedEntity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<bool> DeleteWriteOffs(Guid id)
        {
            var entityData = _dbContext.WriteOffs.FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.WriteOffs.Remove(entityData);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task PatchWriteOffs(Guid id, JsonPatchDocument<WriteOffs> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.WriteOffs.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.WriteOffs.Update(existingEntity);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}