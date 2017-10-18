using System;
using System.Collections.Generic;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public interface IUserService {
        int RegisterUser(string name, string email, string password, out string errMessage);

        int RegisterFacebook(string name, string email, string facebookId, out string errMessage);

        bool UpdateUser(int id, string name, string email, string phone, bool? showCustomer, bool? showAccount, bool? showCredit, bool? showCard, bool? showExpense, bool? showCustomerExclusion, out string errMessage);

        bool UpdateUser(int id, string name, string email, string password, string phone, out string errMessage);

        List<UserDataModel> GetUserList(string name, string email, string sort, bool sortDescending, int page, int pageSize, out int total);

        UserDataModel GetUserById(int id);

        UserDataModel GetUserByEmail(string email);

        UserDataModel GetUserByEmailAndPassword(string email, string password);

        void DeleteUser(int id);

        Guid SendPasswordGuid(string email);

        bool CheckPasswordGuid(Guid passwordGuid);

        UserDataModel SetNewPassword(Guid passwordGuid, string newPassword);

        bool SetNewPassword(int id, string newPassword);
    }
}