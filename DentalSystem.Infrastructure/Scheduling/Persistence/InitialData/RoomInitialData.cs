using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DentalSystem.Entities.Scheduling;
using DentalSystem.Interfaces.Infrastructure.Common.Persistence;

namespace DentalSystem.Infrastructure.Scheduling.Persistence.InitialData
{
    public class RoomInitialData : IInitialData
    {
        public Type EntityType => typeof(Room);

        public int Priority => 4;

        public IEnumerable<object> GetData()
            => new List<Room>
            {
                new Room() { Id = new Guid("75f5f7fa-0ca1-41b9-aab9-fa631bd2bf55"), Name = "Room 01" },
                new Room() { Id = new Guid("1b25130c-d75c-4daa-a408-45f1dd890ec9"), Name = "Room 02" },
            };

        public Task<bool> InitData()
        {
            return Task.FromResult(true);
        }
    }
}