using BuildingManagement.Models;
using System;
using System.Threading.Tasks;

namespace BuildingManagement.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MainContext _context = new MainContext();

        private IMeterTypeRepository _meterTypeRepository;
        private IMeterRepository _meterRepository;
        private IMeterReadingRepository _meterReadingRepository;
        private ISubMeterRepository _subMeterRepository;
        private ISubMeterReadingRepository _subMeterReadingRepository;
        private ISubSubMeterRepository _subSubMeterRepository;
        private ISubSubMeterReadingRepository _subSubMeterReadingRepository;

        private IClientRepository _clientRepository;
        private ISubClientRepository _subClientRepository;

        private ISpaceTypeRepository _spaceTypeRepository;
        private ISectionRepository _sectionRepository;
        private ILevelRepository _levelRepository;
        private ISpaceRepository _spaceRepository;

        private IInvoiceTypeRepository _invoiceTypeRepository;
        private IProviderRepository _providerRepository;
        private IServiceRepository _serviceRepository;
        private IInvoiceRepository _invoiceRepository;

        private ICostRepository _costRepository;

        private IUserRepository _userRepository;
        private GenericRepository<UserRole> _userRoleRepository;

        public IMeterTypeRepository MeterTypeRepository
        {
            get
            {

                if (_meterTypeRepository == null)
                {
                    _meterTypeRepository = new MeterTypeRepository(_context);
                }
                return _meterTypeRepository;
            }
        }

        public IMeterRepository MeterRepository
        {
            get
            {
                if (_meterRepository == null)
                {
                    _meterRepository = new MeterRepository(_context);
                }
                return _meterRepository;
            }
        }

        public IMeterReadingRepository MeterReadingRepository
        {
            get
            {
                if (_meterReadingRepository == null)
                {
                    _meterReadingRepository = new MeterReadingRepository(_context);
                }
                return _meterReadingRepository;
            }
        }

        public ISubMeterRepository SubMeterRepository
        {
            get
            {
                if (_subMeterRepository == null)
                {
                    _subMeterRepository = new SubMeterRepository(_context);
                }
                return _subMeterRepository;
            }
        }

        public ISubMeterReadingRepository SubMeterReadingRepository
        {
            get
            {
                if (_subMeterReadingRepository == null)
                {
                    _subMeterReadingRepository = new SubMeterReadingRepository(_context);
                }
                return _subMeterReadingRepository;
            }
        }

        public ISubSubMeterRepository SubSubMeterRepository
        {
            get
            {
                if (_subSubMeterRepository == null)
                {
                    _subSubMeterRepository = new SubSubMeterRepository(_context);
                }
                return _subSubMeterRepository;
            }
        }

        public ISubSubMeterReadingRepository SubSubMeterReadingRepository
        {
            get
            {
                if (_subSubMeterReadingRepository == null)
                {
                    _subSubMeterReadingRepository = new SubSubMeterReadingRepository(_context);
                }
                return _subSubMeterReadingRepository;
            }
        }

        public IClientRepository ClientRepository
        {
            get
            {
                if (_clientRepository == null)
                {
                    _clientRepository = new ClientRepository(_context);
                }
                return _clientRepository;
            }
        }

        public ISubClientRepository SubClientRepository
        {
            get
            {
                if (_subClientRepository == null)
                {
                    _subClientRepository = new SubClientRepository(_context);
                }
                return _subClientRepository;
            }
        }

        public ISpaceTypeRepository SpaceTypeRepository
        {
            get
            {
                if (_spaceTypeRepository == null)
                {
                    _spaceTypeRepository = new SpaceTypeRepository(_context);
                }
                return _spaceTypeRepository;
            }
        }

        public ISectionRepository SectionRepository
        {
            get
            {
                if (_sectionRepository == null)
                {
                    _sectionRepository = new SectionRepository(_context);
                }
                return _sectionRepository;
            }
        }

        public ILevelRepository LevelRepository
        {
            get
            {
                if (_levelRepository == null)
                {
                    _levelRepository = new LevelRepository(_context);
                }
                return _levelRepository;
            }
        }

        public ISpaceRepository SpaceRepository
        {
            get
            {
                if (_spaceRepository == null)
                {
                    _spaceRepository = new SpaceRepository(_context);
                }
                return _spaceRepository;
            }
        }

        public IInvoiceTypeRepository InvoiceTypeRepository
        {
            get
            {
                if (_invoiceTypeRepository == null)
                {
                    _invoiceTypeRepository = new InvoiceTypeRepository(_context);
                }
                return _invoiceTypeRepository;
            }
        }

        public IProviderRepository ProviderRepository
        {
            get
            {
                if (_providerRepository == null)
                {
                    _providerRepository = new ProviderRepository(_context);
                }
                return _providerRepository;
            }
        }

        public IServiceRepository ServiceRepository
        {
            get
            {
                if (_serviceRepository == null)
                {
                    _serviceRepository = new ServiceRepository(_context);
                }
                return _serviceRepository;
            }
        }

        public IInvoiceRepository InvoiceRepository
        {
            get
            {
                if (_invoiceRepository == null)
                {
                    _invoiceRepository = new InvoiceRepository(_context);
                }
                return _invoiceRepository;
            }
        }

        public ICostRepository CostRepository
        {
            get
            {
                if (_costRepository == null)
                {
                    _costRepository = new CostRepository(_context);
                }
                return _costRepository;
            }
        }

        public IUserRepository UserRepository => _userRepository ?? (_userRepository = new UserRepository(_context));
        public GenericRepository<UserRole> UserRoleRepository => _userRoleRepository ?? (_userRoleRepository = new GenericRepository<UserRole>(_context));

        public void Save()
        {
            _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}