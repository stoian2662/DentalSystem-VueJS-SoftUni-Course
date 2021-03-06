using System.Threading.Tasks;
using DentalSystem.UseCases.Common.Dto.Output;
using DentalSystem.Entities.Scheduling;
using DentalSystem.Interfaces.Infrastructure.Common.Persistence;
using DentalSystem.Interfaces.UseCases.Scheduling.Dto.Input;
using DentalSystem.Interfaces.UseCases.Common.Dto.Output;
using DentalSystem.Interfaces.UseCases.Scheduling.Commands;
using DentalSystem.Interfaces.UseCases.Common.Validation;

namespace DentalSystem.UseCases.Scheduling.Commands
{
    public class AddTreatmentSessionCommand : IAddTreatmentSessionCommand
    {
        public AddTreatmentSessionCommand(
            IApplicationValidator<ITreatmentSessionInput> validator,
            IGenericRepository<TreatmentSession> treatmentSessionRepository,
            IGenericRepository<Patient> patientRepository,
            IGenericRepository<DentalTeam> dentalTeamRepository,
            IGenericRepository<Treatment> treatmentRepository,
            IUnitOfWork uoW)
        {
            Validator = validator;
            TreatmentSessionRepository = treatmentSessionRepository;
            PatientRepository = patientRepository;
            DentalTeamRepository = dentalTeamRepository;
            TreatmentRepository = treatmentRepository;
            UoW = uoW;
        }

        public IApplicationValidator<ITreatmentSessionInput> Validator { get; }

        public IGenericRepository<TreatmentSession> TreatmentSessionRepository { get; }

        public IGenericRepository<Patient> PatientRepository { get; }

        public IGenericRepository<DentalTeam> DentalTeamRepository { get; }

        public IGenericRepository<Treatment> TreatmentRepository { get; }

        public IUnitOfWork UoW { get; }

        public async Task<IResult<IMessageOutput>> ExecuteAsync(ITreatmentSessionInput input)
        {
            var validationResult = Validator.Validate(input);
            if (validationResult.Errors.Count > 0)
            {
                return new Result<IMessageOutput>(validationResult.Errors);
            }

            var patient = await PatientRepository.SingleOrDefaultAsync(p => p.ReferenceId == input.PatientReferenceId);
            var dentalTeam = await DentalTeamRepository.SingleOrDefaultAsync(dt => dt.ReferenceId == input.DentalTeamReferenceId);
            var treatment = await TreatmentRepository.SingleOrDefaultAsync(t => t.ReferenceId == input.TreatmentReferenceId);
            var treatmentSession = new TreatmentSession()
            {
                PatientId = patient.Id,
                DentalTeamId = dentalTeam.Id,
                TreatmentId = treatment.Id,
                Start = input.Start.Value,
                End = input.End.Value
            };

            await TreatmentSessionRepository.AddAsync(treatmentSession);

            await UoW.SaveAsync();

            return new Result<IMessageOutput>(
                value: new MessageOutput("Treatment Session successfully created."),
                type: ResultType.Created
            );
        }
    }
}