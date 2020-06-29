using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FruityNET.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FruityNET.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Entities.Group> Group { get; set; }
        public DbSet<GroupUser> GroupUser { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<RequestUser> RequestUser { get; set; }
        public DbSet<GroupRequest> GroupRequest { get; set; }
        public DbSet<GroupRequestUser> GroupRequestUser { get; set; }
        public DbSet<GroupOwner> GroupOwner { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<FriendList> FriendList { get; set; }
        public DbSet<FriendUser> FriendUser { get; set; }
        public DbSet<UserAccount> Account { get; set; }
        public DbSet<NotificationBox> NotificationBox { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<AdminApprovalBox> AdminApproval { get; set; }
        public DbSet<SiteOwner> SiteOwner { get; set; }
        public DbSet<AdminRequest> AdminRequest { get; set; }
        public DbSet<AdminRequestor> AdminRequestor { get; set; }
    }
}
