using MagicVilla.Data;
using MagicVilla.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla.Repository
{
    public class Repository<T>:IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            //_context.VillaNumbers.Include(n => n.Villa).ToList();
            this.dbSet = _context.Set<T>();
        }
        public void Create(T entity)
        {
            dbSet.Add(entity);
            Save();
        }

        public T Get(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeproperties = null)
        {
            IQueryable<T> query = dbSet;
            if (!tracked)
                query = query.AsNoTracking();
            if (filter != null)
                query = query.Where(filter);
            if (includeproperties !=null)
            {
                foreach(var property in includeproperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(property);
                }
            }
            return query.FirstOrDefault();
        }

        public List<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeproperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
                query = query.Where(filter);
			if (includeproperties != null)
			{
				foreach (var property in includeproperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}
			return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
            Save();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
