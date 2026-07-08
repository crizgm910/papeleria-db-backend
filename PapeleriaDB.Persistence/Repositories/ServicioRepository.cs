using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence.Contexts;

namespace PapeleriaDB.Persistence.Repositories;

public class ServicioRepository : Repository<Servicio>, IServicioRepository
{
    public ServicioRepository(ApplicationDbContext context) : base(context)
    {
    }
}
