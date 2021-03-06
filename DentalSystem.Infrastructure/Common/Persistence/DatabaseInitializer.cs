using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DentalSystem.Infrastructure.Migrations;
using DentalSystem.Interfaces.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalSystem.Infrastructure.Common.Persistence
{
    internal class DatabaseInitializer : IInitializer
    {
        private readonly DentalSystemDbContext db;
        private readonly IEnumerable<IInitialData> initialDataProviders;

        public DatabaseInitializer(
            DentalSystemDbContext db,
            IEnumerable<IInitialData> initialDataProviders)
        {
            this.db = db;
            this.initialDataProviders = initialDataProviders;
        }

        public async Task Initialize()
        {
            var appliedMigrations = this.db.Database.GetAppliedMigrations();
            if (!appliedMigrations.Any(m => m.EndsWith(nameof(Initial_Migration))))
            {
                this.db.Database.Migrate();

                foreach (var initialDataProvider in this.initialDataProviders)
                {
                    var applyData = await initialDataProvider.InitData();

                    if (applyData)
                    {
                        var data = initialDataProvider.GetData();
                        this.db.AddRange(data);
                    }
                }

                this.db.SaveChanges();
            }
        }
    }
}