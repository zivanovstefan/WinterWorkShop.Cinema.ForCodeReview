using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDomainModel>> GetAllAsync();
        Task<TagDomainModel> GetTagByIdAsync(int id);
        Task<CreateTagResultModel> AddTag(TagDomainModel newTag);
        Task<TagDomainModel> UpdateTag(TagDomainModel tagToUpdate);
        Task<TagDomainModel> DeleteTag(int id);
    }
}
