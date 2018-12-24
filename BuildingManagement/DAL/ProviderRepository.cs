using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
    {
        public ProviderRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public IEnumerable<Provider> GetFilteredProviders(string searchString)
        {
            return MainContext.Providers
                .Where(p =>
                    p.Name.ToLower().Contains(searchString) ||
                    p.FiscalCode.ToLower().Contains(searchString) ||
                    p.TradeRegister.ToLower().Contains(searchString) ||
                    p.Address.ToLower().Contains(searchString) ||
                    p.Phone.ToLower().Contains(searchString) ||
                    p.Email.ToLower().Contains(searchString) ||
                    p.BankAccount.ToLower().Contains(searchString) ||
                    p.Bank.ToLower().Contains(searchString));
        }

        public IEnumerable<Provider> OrderProviders(IEnumerable<Provider> providers, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    providers = providers.OrderByDescending(p => p.Name);
                    break;
                case "FiscalCode":
                    providers = providers.OrderBy(p => p.FiscalCode);
                    break;
                case "fiscalCode_desc":
                    providers = providers.OrderByDescending(p => p.FiscalCode);
                    break;
                case "TradeRegister":
                    providers = providers.OrderBy(p => p.TradeRegister);
                    break;
                case "tradeRegister_desc":
                    providers = providers.OrderByDescending(p => p.TradeRegister);
                    break;
                case "Address":
                    providers = providers.OrderBy(p => p.Address);
                    break;
                case "address_desc":
                    providers = providers.OrderByDescending(p => p.Address);
                    break;
                case "Phone":
                    providers = providers.OrderBy(p => p.Phone);
                    break;
                case "phone_desc":
                    providers = providers.OrderByDescending(p => p.Phone);
                    break;
                case "Email":
                    providers = providers.OrderBy(p => p.Email);
                    break;
                case "email_desc":
                    providers = providers.OrderByDescending(p => p.Email);
                    break;
                case "BankAccount":
                    providers = providers.OrderBy(p => p.BankAccount);
                    break;
                case "bankAccount_desc":
                    providers = providers.OrderByDescending(p => p.BankAccount);
                    break;
                case "Bank":
                    providers = providers.OrderBy(p => p.Bank);
                    break;
                case "bank_desc":
                    providers = providers.OrderByDescending(p => p.Bank);
                    break;
                case "TVAPayer":
                    providers = providers.OrderBy(p => p.TVAPayer);
                    break;
                case "tvaPayer_desc":
                    providers = providers.OrderByDescending(p => p.TVAPayer);
                    break;
                default: // Name ascending 
                    providers = providers.OrderBy(p => p.Name);
                    break;
            }
            return providers;
        }
    }
}