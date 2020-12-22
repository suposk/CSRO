using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;

namespace CSRO.Server.Services
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IRepository<Ticket> _repository;
        private AppVersionContext _context;

        public TicketRepository(IRepository<Ticket> repository)
        {
            _repository = repository;
            _context = _repository.DatabaseContext as AppVersionContext;
        }
    }
}
