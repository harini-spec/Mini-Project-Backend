﻿using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories;
using BusBookingAppln.Repositories.Classes;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusBookingUnitTest.RepositoryUnitTest
{
    public class UserRepositoryTest
    {
        IRepository<int, User> UserRepo;
        BusBookingContext context;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("UserRepoDB");
            context = new BusBookingContext(optionsBuilder.Options);
            UserRepo = new MainRepository<int, User>(context);
        }

        [TearDown]
        public void Teardown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Test]
        public async Task GetAllUsersFailTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoItemsFoundException>(async () => await UserRepo.GetAll());
        }

        [Test]
        public async Task AddUserSuccessTest()
        {
            // Action
            var result = await UserRepo.Add(new User
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                Role = "Customer"
            });

            // Assert
            Assert.That(result.Id, Is.EqualTo(1));

            await UserRepo.Delete(1);
        }

        [Test]
        public async Task AddUserInvalidOperationExceptionTest()
        {
            // Arrange
            await UserRepo.Add(new User
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                Role = "Customer"
            });

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationCustomException>(async () => await UserRepo.Add(new User
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                Role = "Customer"

            }));


            // Assert
            Assert.That(exception.Message, Is.EqualTo("Invalid operation : Key already present in DB"));
        }

        [Test]
        public async Task GetByUserIdSuccessTest()
        {
            // Arrange
            await UserRepo.Add(new User
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                Role = "Customer"
            });

            // Action
            var result = await UserRepo.GetById(1);

            // Assert
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByUserIdFailureTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await UserRepo.GetById(100));
        }

        [Test]
        public async Task DeleteByUserIdExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await UserRepo.Delete(100));
        }

        [Test]
        public async Task GetAllUsersSuccessTest()
        {
            // Arrange
            await UserRepo.Add(new User
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                Role = "Customer"
            });

            // Action
            var result = await UserRepo.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));

        }

        [Test]
        public async Task UpdateUserExceptionTest()
        {
            // Arrange
            var User = new User
            {
                Id = 100,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                Role = "Customer"
            };

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await UserRepo.Update(User, 100));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity of type User with ID 100 not found."));
        }

        [Test]
        public async Task DeleteUserSuccessTest()
        {
            // Arrange
            await UserRepo.Add(new User
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                Role = "Customer"
            });

            // Action
            var entity = await UserRepo.Delete(1);

            // Assert
            Assert.That(entity.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateUserSuccessTest()
        {
            // Arrange
            await UserRepo.Add(new User
            {
                Id = 1,
                Name = "Sam",
                Age = 25,
                Email = "sam@gmail.com",
                Phone = "9999999999",
                Role = "Customer"
            });
            var entity = await UserRepo.GetById(1);
            entity.Role = "Admin";

            // Action
            var result = await UserRepo.Update(entity, 1);

            // Assert
            Assert.That(result.Role, Is.EqualTo("Admin"));
        }
    }
}
