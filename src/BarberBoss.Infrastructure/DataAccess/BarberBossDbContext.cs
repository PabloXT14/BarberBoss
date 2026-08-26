using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess;

internal class BarberBossDbContext : DbContext
{
    public BarberBossDbContext(DbContextOptions options) : base(options)
    {
    }

    // OBS: O nome da propriedade DbSet deve ser o mesmo nome da tabela no banco de dados (não precisa ser case-sensitive), caso contrário, será necessário configurar o mapeamento no OnModelCreating.
    public DbSet<Billing> Billings { get; set; }
}