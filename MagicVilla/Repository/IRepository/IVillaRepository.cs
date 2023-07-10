using MagicVilla.Models;
using System.Linq.Expressions;

namespace MagicVilla.Repository.IRepository
{
    public interface IVillaRepository:IRepository<Villa>
    {
        Villa Update(Villa entity);
      
    }
}
