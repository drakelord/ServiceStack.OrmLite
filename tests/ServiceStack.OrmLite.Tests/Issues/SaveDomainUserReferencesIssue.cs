﻿using System.Collections.Generic;
using NUnit.Framework;
using ServiceStack.DataAnnotations;
using ServiceStack.Text;

namespace ServiceStack.OrmLite.Tests.Issues
{
    [Alias("ProUser")]
    public class DomainUser
    {
        public int UserType { get; set; }
        public string Id { get; set; }

        [Reference]
        public Address HomeAddress { get; set; }

        [Reference]
        public List<Order> Orders { get; set; }

        [DataAnnotations.Ignore]
        public UserType UserTypeEnum
        {
            get { return (UserType)UserType; }
            set { UserType = (int)value; }
        }
    }

    public class Address
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string ProUserId { get; set; }
        public string StreetName { get; set; }
    }

    public enum UserType
    {
        Normal = 1,
        Domain = 2
    }

    public class Order
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string ProUserId { get; set; }

        public string Details { get; set; }

    }

    [TestFixture]
    public class SaveDomainUserReferencesIssue
        : OrmLiteTestBase
    {
        [Test]
        public void Can_save_DomainUser_references()
        {
            using (var db = OpenDbConnection())
            {
                db.DropAndCreateTable<DomainUser>();
                db.DropAndCreateTable<Order>();
                db.DropAndCreateTable<Address>();

                var user = new DomainUser
                {
                    Id = "UserId",
                    UserType = 1,
                    HomeAddress = new Address
                    {
                        StreetName = "1 Street"
                    },
                    Orders = new List<Order>
                    {
                        new Order { Details = "Order1 Details" },
                        new Order { Details = "Order2 Details" },
                    }
                };

                //Same as below in 1 line
                //db.Save(user, references: true);
                db.Save(user);
                db.SaveReferences(user, user.HomeAddress);
                db.SaveReferences(user, user.Orders);

                user = db.LoadSingleById<DomainUser>("UserId");
                user.PrintDump();
                Assert.That(user.Orders.Count, Is.EqualTo(2));

                user.UserTypeEnum = UserType.Domain;
                user.HomeAddress.StreetName = "Some new street";
                user.Orders[1].Details = "Nestle Chocolates";
                user.Orders.Add(new Order
                {
                    ProUserId = user.Id,
                    Details = "Reese",
                });

                //Same as below in 1 line
                //db.Save(user, references: true);
                db.Save(user);
                db.SaveReferences(user, user.HomeAddress);
                db.SaveReferences(user, user.Orders);

                user = db.LoadSingleById<DomainUser>("UserId");
                user.PrintDump();

                Assert.That(user.Orders.Count, Is.EqualTo(3));
            }
        }
    }
}