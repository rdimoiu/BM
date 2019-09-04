using Antlr.Runtime;
using BuildingManagement.Models;
using BuildingManagement.Utils;
using System;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public class MainContextInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<MainContext>
    {
        protected override void Seed(MainContext context)
        {
            var client1 = new Client
            {
                Address = "Gheorge Lazar",
                Contact = "Pavel Bucur",
                Email = "pavelbucur@gmail.com",
                Name = "Fructus",
                Phone = "0732456789"
            };
            var client2 = new Client
            {
                Address = "Piata 700",
                Contact = "George Stanciu",
                Email = "gergestanciu@gmail.com",
                Name = "700",
                Phone = "0732456000"
            };
            var clients = new List<Client>
            {
                client1,
                client2
            };
            clients.ForEach(c => context.Clients.Add(c));
            context.SaveChanges();

            var subClient1 = new SubClient
            {
                CNP = "1750913233445",
                FiscalCode = "111",
                Bank = "BRD",
                IBAN = "111112222233333",
                Country = "Romania",
                State = "Timis",
                City = "Timisoara",
                Street = "Amforei",
                Contact = "Titi Aur",
                Email = "titiaur@gmail.com",
                Name = "SC1",
                Phone = "0722333444",
                Client = client1
            };
            var subClient2 = new SubClient
            {
                CNP = "1760214243546",
                FiscalCode = "222",
                Bank = "BRD",
                IBAN = "222223333344444",
                Country = "Romania",
                State = "Cluj",
                City = "Cluj",
                Street = "Gheorge Lazar",
                Contact = "Titi Argint",
                Email = "titiargint@gmail.com",
                Name = "SC2",
                Phone = "0722444555",
                Client = client1
            };
            var subClient3 = new SubClient
            {
                CNP = "1850714253647",
                FiscalCode = "333",
                Bank = "BRD",
                IBAN = "333334444455555",
                Country = "Romania",
                State = "Timis",
                City = "Timisoara",
                Street = "Teiului",
                Contact = "Titi Bronz",
                Email = "titibronz@gmail.com",
                Name = "SC3",
                Phone = "0722777888",
                Client = client1
            };
            var subClient4 = new SubClient
            {
                CNP = "2",
                FiscalCode = "2",
                Bank = "BCR",
                IBAN = "444445555566666",
                Country = "Romania",
                State = "Timis",
                City = "Timisoara",
                Street = "Gheorge Lazar",
                Contact = "Baba Cloanta",
                Email = "babacloanta@gmail.com",
                Name = "SC4",
                Phone = "0744555666",
                Client = client1
            };
            var subClient5 = new SubClient
            {
                CNP = "3",
                FiscalCode = "3",
                Bank = "BCR",
                IBAN = "555556666677777",
                Country = "Romania",
                State = "Arad",
                City = "Arad",
                Street = "Gheorge Cioban",
                Contact = "Ileana Cosanzeana",
                Email = "ileana@gmail.com",
                Name = "SC5",
                Phone = "0744666777",
                Client = client2
            };
            var subClient6 = new SubClient
            {
                CNP = "4",
                FiscalCode = "4",
                Bank = "BRD",
                IBAN = "777776666655555",
                Country = "Romania",
                State = "Cluj",
                City = "Cluj",
                Street = "Oaie Cioban",
                Contact = "Bomba Tanase",
                Email = "bomba@gmail.com",
                Name = "SC6",
                Phone = "0744666888",
                Client = client2
            };
            var subClients = new List<SubClient>
            {
                subClient1,
                subClient2,
                subClient3,
                subClient4,
                subClient5,
                subClient6
            };
            subClients.ForEach(sc => context.SubClients.Add(sc));
            context.SaveChanges();

            var spaceType1 = new SpaceType
            {
                Type = "Apartament"
            };
            var spaceType2 = new SpaceType
            {
                Type = "Parcare"
            };
            var spaceTypes = new List<SpaceType>
            {
                spaceType1,
                spaceType2
            };
            spaceTypes.ForEach(st => context.SpaceTypes.Add(st));
            context.SaveChanges();

            var section1 = new Section
            {
                Number = "Tronson AP",
                Surface = 80m,
                People = 8,
                Client = client1
            };
            var section2 = new Section
            {
                Number = "Tronson PARC",
                Surface = 25m,
                People = 5,
                Client = client1
            };
            var section3 = new Section
            {
                Number = "Tronson AP 7",
                Surface = 75m,
                People = 6,
                Client = client2
            };
            var section4 = new Section
            {
                Number = "Tronson PARC 7",
                Surface = 12m,
                People = 4,
                Client = client2
            };
            var sections = new List<Section>
            {
                section1,
                section2,
                section3,
                section4
            };
            sections.ForEach(s => context.Sections.Add(s));
            context.SaveChanges();

            var level1 = new Level
            {
                Number = "Nivel 0 (PARC)",
                Surface = 15m,
                People = 3,
                Section = section2
            };
            var level2 = new Level
            {
                Number = "Nivel 1 (PARC)",
                Surface = 10m,
                People = 2,
                Section = section2
            };
            var level3 = new Level
            {
                Number = "Nivel 2 (AP)",
                Surface = 30m,
                People = 3,
                Section = section1
            };
            var level4 = new Level
            {
                Number = "Nivel 3 (AP)",
                Surface = 50m,
                People = 5,
                Section = section1
            };
            var level5 = new Level
            {
                Number = "Nivel 0 (PARC) 7",
                Surface = 12m,
                People = 4,
                Section = section4
            };
            var level6 = new Level
            {
                Number = "Nivel 1 (AP) 7",
                Surface = 40m,
                People = 3,
                Section = section3
            };
            var level7 = new Level
            {
                Number = "Nivel 1.1 (AP) 7",
                Surface = 35m,
                People = 3,
                Section = section3
            };
            var levels = new List<Level>
            {
                level1,
                level2,
                level3,
                level4,
                level5,
                level6,
                level7
            };
            levels.ForEach(l => context.Levels.Add(l));
            context.SaveChanges();

            var space1 = new Space
            {
                Number = "AP 2.1",
                Surface = 10m,
                People = 1,
                SpaceType = spaceType1,
                SubClient = subClient1,
                Level = level3
            };
            var space2 = new Space
            {
                Number = "AP 2.2",
                Surface = 20m,
                People = 2,
                SpaceType = spaceType1,
                SubClient = subClient2,
                Level = level3
            };
            var space3 = new Space
            {
                Number = "AP 3.1",
                Surface = 30m,
                People = 3,
                SpaceType = spaceType1,
                SubClient = subClient3,
                Level = level4
            };
            var space4 = new Space
            {
                Number = "AP 3.2",
                Surface = 20m,
                People = 2,
                SpaceType = spaceType1,
                SubClient = subClient4,
                Level = level4
            };
            var space5 = new Space
            {
                Number = "P 1",
                Surface = 5m,
                People = 1,
                SpaceType = spaceType2,
                SubClient = subClient1,
                Level = level1
            };
            var space6 = new Space
            {
                Number = "P 2",
                Surface = 5m,
                People = 1,
                SpaceType = spaceType2,
                SubClient = subClient2,
                Level = level1
            };
            var space7 = new Space
            {
                Number = "P 3",
                Surface = 5m,
                People = 1,
                SpaceType = spaceType2,
                SubClient = subClient3,
                Level = level1
            };
            var space8 = new Space
            {
                Number = "P 1.1",
                Surface = 5m,
                People = 1,
                SpaceType = spaceType2,
                SubClient = subClient4,
                Level = level2
            };
            var space9 = new Space
            {
                Number = "P 1.2",
                Surface = 5m,
                People = 1,
                SpaceType = spaceType2,
                SubClient = subClient1,
                Level = level2
            };
            var space10 = new Space
            {
                Number = "P 1",
                Surface = 3m,
                People = 1,
                SpaceType = spaceType2,
                SubClient = subClient6,
                Level = level5
            };
            var space11 = new Space
            {
                Number = "P 2",
                Surface = 3m,
                People = 1,
                SpaceType = spaceType2,
                SubClient = subClient5,
                Level = level5
            };
            var space12 = new Space
            {
                Number = "P 3",
                Surface = 3m,
                People = 1,
                SpaceType = spaceType2,
                SubClient = subClient5,
                Level = level5
            };
            var space13 = new Space
            {
                Number = "P 4",
                Surface = 3m,
                People = 1,
                SpaceType = spaceType2,
                SubClient = subClient6,
                Level = level5
            };
            var space14 = new Space
            {
                Number = "AP 1.1",
                Surface = 15m,
                People = 1,
                SpaceType = spaceType1,
                SubClient = subClient6,
                Level = level6
            };
            var space15 = new Space
            {
                Number = "AP 1.2",
                Surface = 25m,
                People = 2,
                SpaceType = spaceType1,
                SubClient = subClient5,
                Level = level6
            };
            var space16 = new Space
            {
                Number = "AP 1.1.1",
                Surface = 10m,
                People = 1,
                SpaceType = spaceType1,
                SubClient = subClient6,
                Level = level7
            };
            var space17 = new Space
            {
                Number = "AP 1.1.2",
                Surface = 25m,
                People = 2,
                SpaceType = spaceType1,
                SubClient = subClient5,
                Level = level7
            };
            var spaces = new List<Space>
            {
                space1,
                space2,
                space3,
                space4,
                space5,
                space6,
                space7,
                space8,
                space9,
                space10,
                space11,
                space12,
                space13,
                space14,
                space15,
                space16,
                space17
            };
            spaces.ForEach(s => context.Spaces.Add(s));
            context.SaveChanges();

            var provider1 = new Provider
            {
                Name = "Enel",
                Address = "Banat",
                Bank = "Transilvania",
                BankAccount = "1",
                Email = "1@enel.com",
                FiscalCode = "1",
                Phone = "0356111111",
                TradeRegister = "1",
                TVAPayer = true
            };
            var provider2 = new Provider
            {
                Name = "Eon",
                Address = "Banat",
                Bank = "BCR",
                BankAccount = "2",
                Email = "2@eon.com",
                FiscalCode = "2",
                Phone = "0356222222",
                TradeRegister = "2",
                TVAPayer = true
            };
            var provider3 = new Provider
            {
                Name = "Luna si bec",
                Address = "Banat",
                Bank = "BRD",
                BankAccount = "3",
                Email = "3@lunasibec.com",
                FiscalCode = "3",
                Phone = "0356333333",
                TradeRegister = "3",
                TVAPayer = false
            };
            var providers = new List<Provider>
            {
                provider1,
                provider2,
                provider3
            };
            providers.ForEach(p => context.Providers.Add(p));
            context.SaveChanges();

            var invoiceType1 = new InvoiceType
            {
                Type = "Utilitati"
            };
            var invoiceType2 = new InvoiceType
            {
                Type = "Mentenanta"
            };
            var invoiceType3 = new InvoiceType
            {
                Type = "Administrativ"
            };
            var invoiceTypes = new List<InvoiceType>
            {
                invoiceType1,
                invoiceType2,
                invoiceType3
            };
            invoiceTypes.ForEach(it => context.InvoiceTypes.Add(it));
            context.SaveChanges();

            var meterType1 = new MeterType { ID = 1, Type = "apa" };
            var meterType2 = new MeterType { ID = 2, Type = "curent" };
            var meterType3 = new MeterType { ID = 3, Type = "gaz" };
            var meterTypes = new List<MeterType> { meterType1, meterType2, meterType3 };
            meterTypes.ForEach(s => context.MeterTypes.Add(s));
            context.SaveChanges();

            var meter1 = new Meter
            {
                Code = "12345",
                Details = "detail 1",
                DistributionModeID = 1,
                ClientID = 1,
                MeterTypes = new List<MeterType> { meterType1, meterType2 },
                Spaces = new List<Space> { space1, space2, space3, space4 }
            };
            var meter2 = new Meter
            {
                Code = "23456",
                DistributionModeID = 2,
                ClientID = 1,
                MeterTypes = new List<MeterType> { meterType2, meterType3 },
                Spaces = new List<Space> { space5, space6, space7, space8, space9 }
            };
            var meters = new List<Meter> { meter1, meter2 };
            meters.ForEach(s => context.Meters.Add(s));
            context.SaveChanges();

            var subMeter1 = new SubMeter
            {
                Code = "23456.1",
                Details = "details",
                DistributionModeID = 1,
                MeterTypes = new List<MeterType> { meterType3 },
                Spaces = new List<Space> { space5, space8 },
                Meter = meter2
            };
            var subMeters = new List<SubMeter> { subMeter1 };
            subMeters.ForEach(sm => context.SubMeters.Add(sm));
            context.SaveChanges();

            var subSubMeter1 = new SubSubMeter
            {
                Code = "23456.1.1",
                Details = "bla bla",
                DistributionModeID = 3,
                MeterTypes = new List<MeterType> { meterType3 },
                Spaces = new List<Space> { space8 },
                SubMeter = subMeter1
            };
            var subSubMeters = new List<SubSubMeter> { subSubMeter1 };
            subSubMeters.ForEach(ssm => context.SubSubMeters.Add(ssm));
            context.SaveChanges();

            //var meterReading1 = new Reading
            //{
            //    //ID = 1,
            //    Initial = true,
            //    Index = 10m,
            //    Date = new DateTime(2019, 01, 01),
            //    DiscountMonth = DateTime.Now,
            //    AbstractMeter = meter1,
            //    MeterType = meterType1
            //};
            //var meterReading2 = new Reading
            //{
            //    //ID = 2,
            //    Index = 20m,
            //    Date = new DateTime(2019, 01, 31),
            //    DiscountMonth = new DateTime(2019, 01, 01),
            //    AbstractMeter = meter1,
            //    MeterType = meterType1
            //};
            //var meterReading3 = new Reading
            //{
            //    //ID = 3,
            //    Index = 30m,
            //    Date = new DateTime(2019, 02, 28),
            //    DiscountMonth = new DateTime(2019, 02, 01),
            //    AbstractMeter = meter1,
            //    MeterType = meterType1
            //};
            //var meterReading4 = new Reading
            //{
            //    //ID = 4,
            //    Index = 50m,
            //    Date = new DateTime(2019, 03, 31),
            //    DiscountMonth = new DateTime(2019, 03, 01),
            //    AbstractMeter = meter1,
            //    MeterType = meterType1
            //};
            //var meterReadings = new List<Reading> { meterReading1, meterReading2, meterReading3, meterReading4 };
            //meterReadings.ForEach(s => context.Readings.Add(s));
            //context.SaveChanges();

            var userSysadmin = new User
            {
                AccountConfirmed = false,
                Email = "sysadmin@bm.com",
                FirstName = "System",
                LastName = "Admin",
                Password = Cryptography.SimpleAes.Encrypt("demo123")
            };
            context.Users.Add(userSysadmin);

            var sysadminRole = new UserRole
            {
                UserId = userSysadmin.Id,
                UserRoleType = (int)Constants.RoleTypes.Sysadmin
            };
            context.UserRoles.Add(sysadminRole);

            try
            {
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw new EarlyExitException();
            }
        }
    }
}
