using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using eHesabim.Core;
using eHesabim.Core.Data;
using eHesabim.Data.Domain;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public class UserService : IUserService {
        private readonly IRepository<User, int> userRepository;
        private readonly IMessageService messageService;

        public UserService(IRepository<User, int> userRepository, IMessageService messageService) {
            this.userRepository = userRepository;
            this.messageService = messageService;
        }

        public int RegisterUser(string name, string email, string password, out string errMessage) {
            errMessage = string.Empty;

            // check-email
            if (userRepository.Query(a => a.Email == email).Any()) {
                errMessage = "UserExists";
                return 0;
            }

            // register
            var user = new User {
                Name = name,
                Email = email,
                Password = CommonHelper.EncodePassword(password),
                ShowCustomer = true,
                ShowAccount = true,
                ShowCredit = true,
                ShowCard = true,
                ShowExpense = true,
                ShowCustomerExclusion = false
            };

            // add-update
            var id = userRepository.AddUpdate(user);

            // send mail
            messageService.SendUserRegisterMail(name.Split(' ')[0], email);

            // return
            return id;
        }

        public int RegisterFacebook(string name, string email, string facebookId, out string errMessage) {
            errMessage = string.Empty;

            var user = userRepository.Query(a => a.FacebookId == facebookId).FirstOrDefault();
            if (user != null && user.Email != email) {
                errMessage = "FbUserExistsButDifferentUser";
                return 0;
            }

            // check email already registered
            var exists = true;
            user = userRepository.Query(f => f.Email == email).FirstOrDefault();

            // set default values
            if (user == null) {
                exists = false;
                user = new User {
                    Name = name,
                    Email = email,
                    ShowCustomer = true,
                    ShowAccount = true,
                    ShowCredit = true,
                    ShowCard = true,
                    ShowExpense = true,
                    ShowCustomerExclusion = false
                };
            }

            // set facebook id
            user.FacebookId = facebookId;

            // add-update
            var id = userRepository.AddUpdate(user);

            // daha önce kayıtlı değilse mail gönder
            if (!exists) {
                messageService.SendUserRegisterMail(name.Split(' ')[0], email);
            }

            // return
            return id;
        }

        public bool UpdateUser(int id, string name, string email, string phone, bool? showCustomer, bool? showAccount, bool? showCredit, bool? showCard, bool? showExpense, bool? showCustomerExclusion, out string errMessage) {
            errMessage = string.Empty;
            var entity = userRepository.Detail(id);

            if (entity == null) {
                errMessage = "RecordNotFound";
                return false;
            }

            if (userRepository.Query(a => a.Id != id && a.Email == email).Any()) {
                errMessage = "UserExists";
                return false;
            }

            entity.Name = name;
            entity.Email = email;
            entity.ShowCustomer = showCustomer;
            entity.ShowAccount = showAccount;
            entity.ShowCredit = showCredit;
            entity.ShowCard = showCard;
            entity.ShowExpense = showExpense;
            entity.ShowCustomerExclusion = showCustomerExclusion;

            if (!string.IsNullOrEmpty(phone)) {
                entity.Phone = phone;
            }

            userRepository.AddUpdate(entity);

            return true;
        }

        public bool UpdateUser(int id, string name, string email, string password, string phone, out string errMessage) {
            errMessage = string.Empty;
            var entity = userRepository.Detail(id);

            if (entity == null) {
                errMessage = "RecordNotFound";
                return false;
            }

            if (userRepository.Query(a => a.Id != id && a.Email == email).Any()) {
                errMessage = "UserExists";
                return false;
            }

            entity.Name = name;
            entity.Email = email;

            if (!string.IsNullOrEmpty(phone)) {
                entity.Phone = phone;
            }

            if (!string.IsNullOrEmpty(password)) {
                entity.Password = CommonHelper.EncodePassword(password);
            }

            userRepository.AddUpdate(entity);

            return true;
        }

        public List<UserDataModel> GetUserList(string name, string email, string sort, bool sortDescending, int page, int pageSize, out int total) {
            Expression<Func<User, bool>> query = f => !f.IsDeleted;

            if (!string.IsNullOrEmpty(name)) {
                query = query.And(a => a.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(email)) {
                query = query.And(a => a.Email.Contains(email));
            }

            var list = userRepository.Filter(query, page, pageSize, sort, sortDescending, out total).ToListNoLock();
            return AutoMapperConfiguration.Mapper.Map<List<User>, List<UserDataModel>>(list);
        }

        public UserDataModel GetUserById(int id) {
            var data = userRepository.Query(a => a.Id == id).FirstOrDefault();
            return AutoMapperConfiguration.Mapper.Map<User, UserDataModel>(data);
        }

        public UserDataModel GetUserByEmail(string email) {
            var data = userRepository.Query(a => a.Email == email).FirstOrDefault();
            return AutoMapperConfiguration.Mapper.Map<UserDataModel>(data);
        }

        public UserDataModel GetUserByEmailAndPassword(string email, string password) {
            password = CommonHelper.EncodePassword(password);
            var data = userRepository.Query(a => a.Email == email && a.Password == password).FirstOrDefault();
            return AutoMapperConfiguration.Mapper.Map<UserDataModel>(data);
        }

        public void DeleteUser(int id) {
            userRepository.Delete(id);
        }

        public Guid SendPasswordGuid(string email) {
            var user = userRepository.Query(a => a.Email == email).FirstOrDefault();
            if (user != null) {
                // generate guid, update fb
                var passwordGuid = Guid.NewGuid();
                user.PasswordGuid = passwordGuid;
                user.PasswordExpire = DateTime.Now.AddDays(3);
                userRepository.AddUpdate(user);

                // send mail
                messageService.SendUserPasswordRecoveryMail(user.Name.Split(' ')[0], email, passwordGuid);
                return passwordGuid;
            }

            return Guid.Empty;
        }

        public bool CheckPasswordGuid(Guid passwordGuid) {
            var data = userRepository.Query(a => a.PasswordGuid == passwordGuid).FirstOrDefault();
            return data != null && data.PasswordExpire > DateTime.Now;
        }

        public UserDataModel SetNewPassword(Guid passwordGuid, string newPassword) {
            var data = userRepository.Query(a => a.PasswordGuid == passwordGuid).FirstOrDefault();
            if (data != null) {
                data.Password = CommonHelper.EncodePassword(newPassword);
                data.PasswordGuid = null;
                data.PasswordExpire = null;
                userRepository.AddUpdate(data);

                return GetUserById(data.Id);
            }

            return null;
        }

        public bool SetNewPassword(int id, string newPassword) {
            var data = userRepository.Detail(id);
            if (data != null) {
                data.Password = CommonHelper.EncodePassword(newPassword);
                userRepository.AddUpdate(data);

                return true;
            }

            return false;
        }
    }
}