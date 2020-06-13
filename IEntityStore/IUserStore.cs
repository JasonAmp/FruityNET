using System;
using System.Collections.Generic;
using FruityNET.Entities;
using FruityNET.Models;
using Microsoft.AspNetCore.Identity;

namespace FruityNET.IEntityStore
{
    public interface IUserStore
    {
        UserAccount CreateAccount(UserAccount UserAccount);
        UserAccount GetAccountDetails(Guid UserId);

        UserAccount Edit(UserAccount userAccount, EditProfileViewModel model);

        UserAccount GetById(Guid UserId);

        UserAccount GetByIdentityUserId(string UserId);
        UserAccount GetByUsername(string Username);

        List<IdentityUser> GetAll();




    }
}