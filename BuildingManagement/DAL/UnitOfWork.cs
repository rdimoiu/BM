using System;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly MainContext _context = new MainContext();

        private GenericRepository<MeterType> _meterTypeRepository;
        private GenericRepository<DistributionMode> _distributionModeRepository;
        private GenericRepository<Meter> _meterRepository;
        private GenericRepository<SubMeter> _subMeterRepository; 
        private GenericRepository<MeterReading> _meterReadingRepository;

        private GenericRepository<Client> _clientRepository;
        private GenericRepository<SubClient> _subClientRepository;

        private GenericRepository<SpaceType> _spaceTypeRepository;
        private GenericRepository<Section> _sectionRepository;
        private GenericRepository<Level> _levelRepository;
        private GenericRepository<Space> _spaceRepository;

        private GenericRepository<InvoiceType> _invoiceTypeRepository; 
        private GenericRepository<Provider> _providerRepository;
        private GenericRepository<Service> _serviceRepository;
        private GenericRepository<Invoice> _invoiceRepository;

        private GenericRepository<Cost> _costRepository; 

        public GenericRepository<MeterType> MeterTypeRepository
        {
            get
            {

                if (_meterTypeRepository == null)
                {
                    _meterTypeRepository = new GenericRepository<MeterType>(_context);
                }
                return _meterTypeRepository;
            }
        }

        public GenericRepository<DistributionMode> DistributionModeRepository
        {
            get
            {
                if (_distributionModeRepository == null)
                {
                    _distributionModeRepository = new GenericRepository<DistributionMode>(_context);
                }
                return _distributionModeRepository;
            }
        }

        public GenericRepository<Meter> MeterRepository
        {
            get
            {
                if (_meterRepository == null)
                {
                    _meterRepository = new GenericRepository<Meter>(_context);
                }
                return _meterRepository;
            }
        }

        public GenericRepository<SubMeter> SubMeterRepository
        {
            get
            {
                if (_subMeterRepository == null)
                {
                    _subMeterRepository = new GenericRepository<SubMeter>(_context);
                }
                return _subMeterRepository;
            }
        }

        public GenericRepository<MeterReading> MeterReadingRepository
        {
            get
            {
                if (_meterReadingRepository == null)
                {
                    _meterReadingRepository = new GenericRepository<MeterReading>(_context);
                }
                return _meterReadingRepository;
            }
        }

        public GenericRepository<Client> ClientRepository
        {
            get
            {
                if (_clientRepository == null)
                {
                    _clientRepository = new GenericRepository<Client>(_context);
                }
                return _clientRepository;
            }
        }

        public GenericRepository<SubClient> SubClientRepository
        {
            get
            {
                if (_subClientRepository == null)
                {
                    _subClientRepository = new GenericRepository<SubClient>(_context);
                }
                return _subClientRepository;
            }
        }

        public GenericRepository<SpaceType> SpaceTypeRepository
        {
            get
            {
                if (_spaceTypeRepository == null)
                {
                    _spaceTypeRepository = new GenericRepository<SpaceType>(_context);
                }
                return _spaceTypeRepository;
            }
        }

        public GenericRepository<Section> SectionRepository
        {
            get
            {
                if (_sectionRepository == null)
                {
                    _sectionRepository = new GenericRepository<Section>(_context);
                }
                return _sectionRepository;
            }
        }

        public GenericRepository<Level> LevelRepository
        {
            get
            {
                if (_levelRepository == null)
                {
                    _levelRepository = new GenericRepository<Level>(_context);
                }
                return _levelRepository;
            }
        }

        public GenericRepository<Space> SpaceRepository
        {
            get
            {
                if (_spaceRepository == null)
                {
                    _spaceRepository = new GenericRepository<Space>(_context);
                }
                return _spaceRepository;
            }
        }

        public GenericRepository<InvoiceType> InvoiceTypeRepository
        {
            get
            {
                if (_invoiceTypeRepository == null)
                {
                    _invoiceTypeRepository = new GenericRepository<InvoiceType>(_context);
                }
                return _invoiceTypeRepository;
            }
        }

        public GenericRepository<Provider> ProviderRepository
        {
            get
            {
                if (_providerRepository == null)
                {
                    _providerRepository = new GenericRepository<Provider>(_context);
                }
                return _providerRepository;
            }
        }

        public GenericRepository<Service> ServiceRepository
        {
            get
            {
                if (_serviceRepository == null)
                {
                    _serviceRepository = new GenericRepository<Service>(_context);
                }
                return _serviceRepository;
            }
        }

        public GenericRepository<Invoice> InvoiceRepository
        {
            get
            {
                if (_invoiceRepository == null)
                {
                    _invoiceRepository = new GenericRepository<Invoice>(_context);
                }
                return _invoiceRepository;
            }
        }

        public GenericRepository<Cost> CostRepository
        {
            get
            {
                if (_costRepository == null)
                {
                    _costRepository = new GenericRepository<Cost>(_context);
                }
                return _costRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
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