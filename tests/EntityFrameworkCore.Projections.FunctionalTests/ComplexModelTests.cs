﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Projections.FunctionalTests.Helpers;
using EntityFrameworkCore.Projections.Services;
using Microsoft.EntityFrameworkCore;
using ScenarioTests;
using VerifyXunit;
using Xunit;

#nullable disable

namespace EntityFrameworkCore.Projections.FunctionalTests
{

    [UsesVerify]
    public class ComplexModelTests
    {
        public class User
        {
            public int Id { get; set; }

            public string DisplayName { get; set; }

            public ICollection<Order> Orders { get; set; }

            // todo: since Order is a nested class, we currently have to fully express the location of this class
            [Projectable]
            public EntityFrameworkCore.Projections.FunctionalTests.ComplexModelTests.Order LastOrder =>
                Orders.OrderByDescending(x => x.RecordDate).FirstOrDefault();

            // todo: since Order is a nested class, we currently have to fully express the location of this class
            [Projectable]
            [NotMapped]
            public IEnumerable<EntityFrameworkCore.Projections.FunctionalTests.ComplexModelTests.Order> Last2Orders =>
                Orders.OrderByDescending(x => x.RecordDate).Take(2);

        }

        public class Order
        {
            public int Id { get; set; }

            public DateTime RecordDate { get; set; }
        } 

        [Fact]
        public Task ProjectOverNavigationProperty()
        {
            using var dbContext = new SampleDbContext<User>();

            var query = dbContext.Set<User>()
                .Select(x => x.LastOrder.RecordDate);

            return Verifier.Verify(query.ToQueryString());
        }

        [Fact]
        public Task ProjectOverCollectionNavigationProperty()
        {
            using var dbContext = new SampleDbContext<User>();

            var query = dbContext.Set<User>()
                .SelectMany(x => x.Last2Orders)
                .Select(x => x.RecordDate);

            return Verifier.Verify(query.ToQueryString());
        }
    }
}
