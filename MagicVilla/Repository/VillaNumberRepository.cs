﻿using MagicVilla.Data;
using MagicVilla.Models;
using MagicVilla.Repository.IRepository;

namespace MagicVilla.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _context;
        public VillaNumberRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public VillaNumber Update(VillaNumber entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.VillaNumbers.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
