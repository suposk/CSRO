using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;

namespace CSRO.Server.Services
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        //private readonly IRepository<Ticket> _repository;
        private AppVersionContext _context;

        public TicketRepository(AppVersionContext context) : base(context)
        {
            //_repository = repository;
            //_context = _repository.DatabaseContext as AppVersionContext;

            _context = context;            
        }
    }
}
