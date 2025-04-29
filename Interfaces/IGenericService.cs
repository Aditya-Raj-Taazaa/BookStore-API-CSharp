using Microsoft.AspNetCore.Mvc;

namespace Test_API.Interfaces
{
    public interface IGenericService<TEntity, TGetDTO, TCreateDTO, TUpdateDTO>
        where TEntity : class
    {
        Task<IEnumerable<TGetDTO>> ListAsync(int page, int pageSize);
        Task<ActionResult<TGetDTO>> GetByIdAsync(int id);
        Task<ActionResult<TGetDTO>> CreateAsync(TCreateDTO createDTO);
        Task<ActionResult<TGetDTO>> UpdateAsync(int id, TUpdateDTO updateDTO);
        Task<IActionResult> DeleteAsync(int id);
    }
}