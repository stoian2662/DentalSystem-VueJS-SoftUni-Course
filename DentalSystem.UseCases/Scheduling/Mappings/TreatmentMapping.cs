using DentalSystem.UseCases.Scheduling.Dto.Output;
using DentalSystem.Entities.Scheduling;
using DentalSystem.Interfaces.UseCases.Scheduling.Dto.Output;
using Mapster;

namespace DentalSystem.UseCases.Scheduling.Mappings
{
    public class TreatmentMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Treatment, ITreatmentOutput>()
                .MapWith((src) => new TreatmentOutput()
                {
                    ReferenceId = src.ReferenceId,
                    Name = src.Name,
                    DurationInMinutes = src.DurationInMinutes
                });
        }
    }
}