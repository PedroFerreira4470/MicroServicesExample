using PlatformService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.DataAccess
{
    public class PlataformRepository : IPlataformRepository
    {
        private readonly AppDbContext _context;

        public PlataformRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Create(Platform obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            _context.Platforms.Add(obj);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Platform GetById(int id)
        {
            return _context.Platforms.Find(id);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
