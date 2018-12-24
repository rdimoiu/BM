using System;
using System.Threading.Tasks;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IMeterTypeRepository MeterTypeRepository { get; }
        IDistributionModeRepository DistributionModeRepository { get; }
        IMeterRepository MeterRepository { get; }
        IMeterReadingRepository MeterReadingRepository { get; }
        ISubMeterRepository SubMeterRepository { get; }
        ISubMeterReadingRepository SubMeterReadingRepository { get; }
        ISubSubMeterRepository SubSubMeterRepository { get; }
        ISubSubMeterReadingRepository SubSubMeterReadingRepository { get; }
        IClientRepository ClientRepository { get; }
        ISubClientRepository SubClientRepository { get; }
        ISpaceTypeRepository SpaceTypeRepository { get; }
        ISectionRepository SectionRepository { get; }
        ILevelRepository LevelRepository { get; }
        ISpaceRepository SpaceRepository { get; }
        IInvoiceTypeRepository InvoiceTypeRepository { get; }
        IProviderRepository ProviderRepository { get; }
        IServiceRepository ServiceRepository { get; }
        IInvoiceRepository InvoiceRepository { get; }
        ICostRepository CostRepository { get; }
        IUserRepository UserRepository { get; }
        GenericRepository<UserRole> UserRoleRepository { get; }
        void Save();
        Task<int> SaveAsync();
    }
}