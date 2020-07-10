using FruityNET.DTOs;
using FruityNET.Entities;
using FruityNET.Models;

namespace FruityNET.Mapper
{
    public class EntityMapper : AutoMapper.Profile
    {
        public EntityMapper()
        {
            CreateMap<UserAccount, AccountDTO>();
            CreateMap<FriendUser, FriendDTO>();


        }
    }
}