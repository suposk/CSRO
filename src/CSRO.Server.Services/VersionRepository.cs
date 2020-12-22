using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;

namespace CSRO.Server.Services
{

    public class VersionRepository : IVersionRepository
    {
        private readonly IRepository<AppVersion> _repository;
        private AppVersionContext _context;

        public VersionRepository(IRepository<AppVersion> repository)
        {
            _repository = repository;
            _context = _repository.DatabaseContext as AppVersionContext;
        }


        public async Task<AppVersion> GetVersion(string version)
        {
            var exist = await _repository.GetByFilter(a => a.VersionFull == version);
            var q = _context.AppVersions.Where(e => !_context.AppVersions.Any(e2 => e2.VersionValue > e.VersionValue));
            var max = await q.FirstOrDefaultAsync();
            return max;
        }

    }
}
