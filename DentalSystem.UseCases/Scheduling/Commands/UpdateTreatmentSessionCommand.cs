using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DentalSystem.UseCases.Common.Dto.Output;
using DentalSystem.Entities.Scheduling;
using DentalSystem.Interfaces.Infrastructure.Common.Persistence;
using DentalSystem.Interfaces.UseCases.Scheduling.Dto.Input;
using DentalSystem.Interfaces.UseCases.Common.Dto.Output;
using DentalSystem.Interfaces.UseCases.Scheduling.Commands;
using DentalSystem.Interfaces.UseCases.Common.Validation;
using Microsoft.EntityFrameworkCore;

namespace DentalSystem.UseCases.Scheduling.Commands
{
    public class UpdateTreatmentSessionCommand : IUpdateTreatmentSessionCommand
    {
        public UpdateTreatmentSessionCommand(
            IApplicationValidator<ITreatmentSessionInput> validator,
            IGenericRepository<TreatmentSession> treatmentSessionRepository,
            IGenericRepository<Treatment> treatmentRepository,
            IGenericRepository<DentalTeam> dentalTeamRepository,
            IUnitOfWork uoW)
        {
            Validator = validator;
            TreatmentSessionRepository = treatmentSessionRepository;
            TreatmentRepository = treatmentRepository;
            DentalTeamRepository = dentalTeamRepository;
            UoW = uoW;
        }

        public IApplicationValidator<ITreatmentSessionInput> Validator { get; }

        public IGenericRepository<TreatmentSession> TreatmentSessionRepository { get; }

        public IGenericRepository<Treatment> TreatmentRepository { get; }

        public IGenericRepository<DentalTeam> DentalTeamRepository { get; }
        public IUnitOfWork UoW { get; }

        public async Task<IResult<IMessageOutput>> ExecuteAsync(ITreatmentSessionInput input)
        {
            var validationResult = Validator.Validate(input);
            if (validationResult.Errors.Count > 0)
            {
                return new Result<IMessageOutput>(validationResult.Errors);
            }

            var treatmentSession = await TreatmentSessionRepository.Where(
                    ts => ts.ReferenceId == input.ReferenceId
                )
                .Include(ts => ts.Treatment)
                .Include(ts => ts.DentalTeam)
                .SingleOrDefaultAsync();

            if (treatmentSession == null)
            {
                return new Result<IMessageOutput>(
                    new List<IError>
                    {
                        new Error(
                            ErrorType.NotFound,
                            $"Treatment session for the given Patient, Dental Team and period cannot be found."
                        )
                    },
                    ResultType.NotFound
                );
            }

            if (treatmentSession.Treatment.ReferenceId != input.TreatmentReferenceId)
            {
                var treatment = await TreatmentRepository.SingleOrDefaultAsync(
                    t => t.ReferenceId == input.TreatmentReferenceId
                );

                treatmentSession.TreatmentId = treatment.Id;
            }

            if (treatmentSession.DentalTeam.ReferenceId != input.DentalTeamReferenceId)
            {
                var dentalTeam = await DentalTeamRepository.SingleOrDefaultAsync(
                    dt => dt.ReferenceId == input.DentalTeamReferenceId
                );

                treatmentSession.DentalTeamId = dentalTeam.Id;
            }

            treatmentSession.Start = input.Start.Value;
            treatmentSession.End = input.End.Value;
            treatmentSession.Status = Enum.Parse<TreatmentSessionStatus>(input.Status);

            await UoW.SaveAsync();

            return new Result<IMessageOutput>(
                value: new MessageOutput("Treatment Session successfully updated."),
                type: ResultType.Updated
            );
        }
    }
}