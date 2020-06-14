using System;
using System.Collections.Generic;
using System.Linq;
using FruityNET.Data;
using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.Enums;
using FruityNET.IEntityStore;
using FruityNET.Models;
using Microsoft.AspNetCore.Identity;

namespace FruityNET.EntityStore
{
    public class UserStore : IUserStore
    {
        private ApplicationDbContext _Context;
        public UserStore(ApplicationDbContext _Context)
        {
            this._Context = _Context;
        }
        public UserAccount CreateAccount(UserAccount UserAccount)
        {
            _Context.Account.Add(UserAccount);
            _Context.SaveChanges();
            return UserAccount;
        }

        public List<IdentityUser> GetAll()
        {
            return _Context.Users.ToList();
        }

        public List<UserAccount> GetAccounts()
        {
            return _Context.Account.ToList();
        }

        public UserAccount Edit(UserAccount userAccount, EditProfileViewModel model)
        {
            userAccount.Email = model.Email;
            userAccount.FirstName = model.FirstName;
            userAccount.LastName = model.LastName;
            userAccount.Username = model.Username;
            userAccount.Location = model.Location;
            userAccount.Occupation = model.Occupation;
            _Context.SaveChanges();
            return userAccount;
        }

        public UserAccount GetAccountDetails(Guid UserId)
        {
            return _Context.Account.Find(UserId);

        }



        public UserAccount GetById(Guid UserId)
        {
            return _Context.Account.Find(UserId);
        }

        public UserAccount GetByIdentityUserId(string UserId)
        {
            return _Context.Account.FirstOrDefault(x => x.UserId == UserId);
        }

        public UserAccount GetByUsername(string Username)
        {
            return _Context.Account.FirstOrDefault(x => x.Username == Username);
        }

        public UserAccount GrantAdmin(UserAccount Account)
        {
            Account.UserType = UserType.Admin;
            _Context.SaveChanges();
            return Account;
        }


    }
}