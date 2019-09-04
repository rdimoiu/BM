using BuildingManagement.Models;
using System;
using System.Threading.Tasks;

namespace BuildingManagement.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IMeterRepository MeterRepository { get; }
        ISubMeterRepository SubMeterRepository { get; }
        ISubSubMeterRepository SubSubMeterRepository { get; }
        IMeterTypeRepository MeterTypeRepository { get; }

        IMeterReadingRepository MeterReadingRepository { get; }
        ISubMeterReadingRepository SubMeterReadingRepository { get; }
        ISubSubMeterReadingRepository SubSubMeterReadingRepository { get; }

        IClientRepository ClientRepository { get; }
        ISubClientRepository SubClientRepository { get; }

        ISectionRepository SectionRepository { get; }
        ILevelRepository LevelRepository { get; }
        ISpaceRepository SpaceRepository { get; }
        ISpaceTypeRepository SpaceTypeRepository { get; }

        IProviderRepository ProviderRepository { get; }

        IServiceRepository ServiceRepository { get; }
        IInvoiceRepository InvoiceRepository { get; }
        IInvoiceTypeRepository InvoiceTypeRepository { get; }

        IUncountedCostRepository UncountedCostRepository { get; }
        ICountedCostRepository CountedCostRepository { get; }

        IUserRepository UserRepository { get; }
        GenericRepository<UserRole> UserRoleRepository { get; }
        void Save();
        Task<int> SaveAsync();
    }
}