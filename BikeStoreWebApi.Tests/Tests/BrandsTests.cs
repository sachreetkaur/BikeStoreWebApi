﻿using BikeStores.MSSQL.Controllers;
using BikeStores.MSSQL.Data;
using BikeStores.MSSQL.Models;
using BikeStores.MSSQL.Service;
using BikeStoreWebApi.Tests.Fixtures;
using Microsoft.Build.Tasks;
using Moq;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeStoreWebApi.Tests.Tests
{
    [TestFixture]
    public class BrandsTests
    {
        private BikestoresContext _dataContext;
        private Mock<IRabbitMqSender> _mockRabbitMQ;
        private IContainer _container;

        [SetUp]
        public void Setup()
        {
            _container=new Container();
            _dataContext = new DatabaseFixture().DataContext;
            _mockRabbitMQ = new Mock<IRabbitMqSender>();
        }

        //[SetUp]
        //public void Setup(DatabaseFixture databaseFixture, ContainerFixture containerFixture)
        //{
        //    this._dataContext = databaseFixture.DataContext;
        //    this._container = containerFixture.Container;
        //}



        //public BrandsTests(DatabaseFixture databaseFixture, ContainerFixture containerFixture)
        //{
        //    this._dataContext = databaseFixture.DataContext;
        //    this._container = containerFixture.Container;
        //}

        [Test]
        public async Task GetBrand_Function_Should_Return_BrandValues_According_To_ID_Passed_In_Parameter_Async()
        {
            //Arrange(oh sabh chiza are arranged jo eh test perform krn layi chaidia)
            int brandId = 11;
            var brand = new Brand
            {
                BrandId = brandId,
                BrandName = "Test",
                Products = new List<Product>()
            };
            _dataContext.Brands.Add(brand);
            _dataContext.SaveChanges();

            var brandController = new BrandsController(_dataContext, _mockRabbitMQ.Object);

            //Act(we execute System under test(sut))
            //call the particular method from controller instance and pass request to it
            var systemUnderTest =await brandController.GetBrand(brandId);

            //Assert(verify results)
            Assert.NotNull(systemUnderTest.Value);
            Assert.That(systemUnderTest.Value.BrandId,Is.EqualTo(brandId) );
        }

        [Test]
        public async Task GetBrands_Should_Return_List_Of_brand_in_DbAsync()
        {
            //Arrange
            var listBrannds
                = new List<Brand>
                {
                    new Brand{ BrandId=1,BrandName="Test1",Products=new List<Product>() },
                    new Brand{ BrandId=2,BrandName="Test2",Products=new List<Product>() },
                    new Brand{ BrandId=3,BrandName="Test3",Products=new List<Product>() },
                };
            _dataContext.Brands.AddRange(listBrannds);
            _dataContext.SaveChanges();

            var brandController = new BrandsController(_dataContext, _mockRabbitMQ.Object);
            _mockRabbitMQ.Setup(x => x.PublishToMessageQueue(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            //Act
            var systemUnderTest = await brandController.GetBrands();

            //Arrange
            Assert.NotNull(systemUnderTest.Value);
            //Assert.That(systemUnderTest.Value.Any(x=>x.BrandId==1));
            Assert.That(systemUnderTest.Value, Is.EquivalentTo(listBrannds));


        }
    }
}
