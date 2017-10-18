using System.Collections.Generic;
using System.Linq;
using eHesabim.Core.Data;
using eHesabim.Data.Domain;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public class CommonService : ICommonService {
        private readonly IRepository<Bank, int> bankRepository;
        private readonly IRepository<City, int> cityRepository;
        private readonly IRepository<County, int> countyRepository;
        private readonly IRepository<Type, int> typeRepository;

        public CommonService(IRepository<Bank, int> bankRepository, IRepository<City, int> cityRepository, IRepository<County, int> countyRepository, IRepository<Type, int> typeRepository) {
            this.bankRepository = bankRepository;
            this.cityRepository = cityRepository;
            this.countyRepository = countyRepository;
            this.typeRepository = typeRepository;
        }

        public List<SelectIntDataModel> GetBankList() {
            return bankRepository
                       .Query()
                       .OrderBy(a => a.Name)
                       .Select(x => new SelectIntDataModel { Id = x.Id, Name = x.Name })
                       .ToListNoLock();
        }

        public List<SelectIntDataModel> GetCityList() {
            return cityRepository
                        .Query()
                        .OrderBy(a => a.Order).ThenBy(a => a.Name)
                        .Select(x => new SelectIntDataModel { Id = x.Id, Name = x.Name })
                        .ToListNoLock();
        }

        public List<SelectIntDataModel> GetCountyList(int cityId) {
            return countyRepository
                        .Query(a => a.CityId == cityId)
                        .OrderBy(a => a.Name)
                        .Select(x => new SelectIntDataModel { Id = x.Id, Name = x.Name })
                        .ToListNoLock();
        }

        public List<SelectIntDataModel> GetTypeList(int parentId) {
            return typeRepository
                       .Query(a => a.ParentId == parentId)
                       .OrderBy(a => a.Order).ThenBy(a => a.Name)
                       .Select(x => new SelectIntDataModel { Id = x.Id, Name = x.Name })
                       .ToListNoLock();
        }
    }
}
