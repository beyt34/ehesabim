using System.Collections.Generic;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public interface ICommonService {
        List<SelectIntDataModel> GetBankList();

        List<SelectIntDataModel> GetCityList();

        List<SelectIntDataModel> GetCountyList(int cityId);

        List<SelectIntDataModel> GetTypeList(int parentId);
    }
}
