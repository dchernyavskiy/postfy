using BuildingBlocks.Persistence.EfCore.Postgres;

namespace Postfy.Services.Network.Shared.Data;

public class OrdersDbContextDesignFactory : DbContextDesignFactoryBase<NetworkDbContext>
{
    public OrdersDbContextDesignFactory()
        : base("PostgresOptions:ConnectionString") { }
}
