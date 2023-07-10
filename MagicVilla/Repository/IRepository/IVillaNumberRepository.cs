using MagicVilla.Models;

namespace MagicVilla.Repository.IRepository
{
    public interface IVillaNumberRepository:IRepository<VillaNumber>
    {
        VillaNumber Update(VillaNumber entity);
    }
}
