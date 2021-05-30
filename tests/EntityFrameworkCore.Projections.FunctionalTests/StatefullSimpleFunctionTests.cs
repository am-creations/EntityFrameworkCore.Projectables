﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Projections.FunctionalTests.Helpers;
using Microsoft.EntityFrameworkCore;
using ScenarioTests;
using VerifyXunit;
using Xunit;

namespace EntityFrameworkCore.Projections.FunctionalTests
{
    [UsesVerify]
    public class StatefullSimpleFunctionTests
    {
        public record Entity
        {
            public int Id { get; set; }
            
            [Projectable]
            public int Computed1() => Id;

            [Projectable]
            public int Computed2() => Id * 2;

            public int Test(int i) => i;
        }

        [Fact]
        public Task FilterOnProjectableProperty()
        {
            using var dbContext = new SampleDbContext<Entity>();

            var query = dbContext.Set<Entity>()
                .Where(x => x.Computed1() == 1);

            return Verifier.Verify(query.ToQueryString());
        }

        [Fact]
        public Task SelectProjectableProperty()
        {
            using var dbContext = new SampleDbContext<Entity>();

            var query = dbContext.Set<Entity>()
                .Select(x => x.Computed1());

            return Verifier.Verify(query.ToQueryString());
        }

        [Fact]
        public Task FilterOnComplexProjectableProperty()
        {
            using var dbContext = new SampleDbContext<Entity>();

            var query = dbContext.Set<Entity>()
                .Where(x => x.Computed2() == 2);

            return Verifier.Verify(query.ToQueryString());
        }

        [Fact]
        public Task SelectComplexProjectableProperty()
        {
            using var dbContext = new SampleDbContext<Entity>();

            var query = dbContext.Set<Entity>()
                .Select(x => x.Computed2());

            return Verifier.Verify(query.ToQueryString());
        }

        [Fact]
        public Task CombineSelectProjectableProperties()
        {
            using var dbContext = new SampleDbContext<Entity>();

            var query = dbContext.Set<Entity>()
                .Select(x => x.Computed1() + x.Computed2());

            return Verifier.Verify(query.ToQueryString());
        }
    }
}
