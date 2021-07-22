using GroceryStoreAPI.Application;
using GroceryStoreAPI.Contracts.Enums;
using GroceryStoreAPI.Contracts.Interfaces;
using GroceryStoreAPI.Contracts.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repositoryMock = new Mock<ICustomerRepository>();

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Invocations.Clear();
        }

        [Test]
        public async Task WhenGetAllCustomers_CustomersFoundExpected()
        {
            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(DataFactory.GetTestCustomers());

            var count = DataFactory.GetTestCustomers().Content.Count();

            var service = new CustomerService(_repositoryMock.Object);
            var customers = await service.GetAllCustomersAsync();

            Assert.AreEqual(count, customers.Content.Count());
        }

        [Test]
        public async Task WhenGetCustomer_CustomerFoundExpected()
        {
            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(DataFactory.GetTestCustomers());

            var customer = DataFactory.GetTestCustomers().Content.First();

            var service = new CustomerService(_repositoryMock.Object);
            var customers = await service.GetCustomerAsync(customer.Id);

            Assert.NotNull(customers.Content);
        }

        [Test]
        public async Task WhenGetCustomer_NotFoundExpected()
        {
            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(DataFactory.GetTestCustomers());

            var lastCustomer = DataFactory.GetTestCustomers().Content.Last();

            var service = new CustomerService(_repositoryMock.Object);
            var result = await service.GetCustomerAsync(lastCustomer.Id + 1);

            Assert.IsNull(result.Content);
            Assert.IsFalse(result.Succeeded());
            Assert.AreEqual(ResultStatus.MissingResource, result.Status);
        }

        [Test]
        public async Task WhenGetCustomer_ErrorFromDB_ErrorStatusExpected()
        {
            var error = Result<IEnumerable<Customer>>.Errored(ResultStatus.InternalError, "Test");

            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(error);
            
            var service = new CustomerService(_repositoryMock.Object);
            var result = await service.GetCustomerAsync(1);

            Assert.IsNull(result.Content);
            Assert.IsFalse(result.Succeeded());
            Assert.AreEqual(error.Status, result.Status);
            Assert.AreEqual(error.ErrorMessage, result.ErrorMessage);
        }

        [Test]
        public async Task WhenAddCustomer_ErrorFromDB_ErrorStatusExpected()
        {
            var error = Result<IEnumerable<Customer>>.Errored(ResultStatus.InternalError, "Test");

            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(error);

            var service = new CustomerService(_repositoryMock.Object);
            var result = await service.AddCustomer(It.IsAny<string>());

            Assert.IsNull(result.Content);
            Assert.IsFalse(result.Succeeded());
            Assert.AreEqual(error.Status, result.Status);
            Assert.AreEqual(error.ErrorMessage, result.ErrorMessage);
            
            _repositoryMock.Verify(r => r.AddCustomerAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task WhenAddCustomer_CustomerExists_ErrorStatusExpected()
        {
            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(DataFactory.GetTestCustomers);

            var service = new CustomerService(_repositoryMock.Object);

            var result = await service.AddCustomer(DataFactory.GetTestCustomers().Content.First().Name);

            Assert.IsNull(result.Content);
            Assert.IsFalse(result.Succeeded());
            Assert.AreEqual(ResultStatus.BadRequest, result.Status);

            _repositoryMock.Verify(r => r.AddCustomerAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task WhenAddCustomer_SuccessExpected()
        {
            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(Result<IEnumerable<Customer>>.Success(new List<Customer>()));

            _repositoryMock.Setup(r => r.AddCustomerAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<Customer>.Success(new Customer()));

            var service = new CustomerService(_repositoryMock.Object);

            var result = await service.AddCustomer("Victor");

            Assert.NotNull(result.Content);
            Assert.IsTrue(result.Succeeded());
            Assert.AreEqual(ResultStatus.Succeeded, result.Status);

            _repositoryMock.Verify(r => r.AddCustomerAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task WhenUpdateCustomer_SuccessExpected()
        {
            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(DataFactory.GetTestCustomers);

            var service = new CustomerService(_repositoryMock.Object);

            var newCustomer = DataFactory.GetTestCustomers().Content.First();
            newCustomer.Name = "NewName";

            var result = await service.UpdateCustomer(newCustomer);

            Assert.IsTrue(result.Succeeded());
            Assert.AreEqual(ResultStatus.Succeeded, result.Status);

            _repositoryMock.Verify(r => r.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Test]
        public async Task WhenUpdateCustomer_CustomerNotFound_ErrorStatusExpected()
        {
            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(Result<IEnumerable<Customer>>.Success(new List<Customer>()));

            var service = new CustomerService(_repositoryMock.Object);

            var newCustomer = DataFactory.GetTestCustomers().Content.First();
            newCustomer.Name = "NewName";

            var result = await service.UpdateCustomer(newCustomer);

            Assert.IsFalse(result.Succeeded());
            Assert.AreEqual(ResultStatus.MissingResource, result.Status);

            _repositoryMock.Verify(r => r.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Test]
        public async Task WhenUpdateCustomer_ErroFromDB_ErrorStatusExpected()
        {
            var error = Result<IEnumerable<Customer>>.Errored(ResultStatus.InternalError, "Test");

            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(error);

            var service = new CustomerService(_repositoryMock.Object);

            var newCustomer = DataFactory.GetTestCustomers().Content.First();
            newCustomer.Name = "NewName";

            var result = await service.UpdateCustomer(newCustomer);

            Assert.IsFalse(result.Succeeded());
            Assert.AreEqual(error.Status, result.Status);
            Assert.AreEqual(error.ErrorMessage, result.ErrorMessage);

            _repositoryMock.Verify(r => r.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Test]
        public async Task WhenRemoveCustomer_SuccessExpected()
        {
            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(DataFactory.GetTestCustomers);

            var service = new CustomerService(_repositoryMock.Object);

            var customer = DataFactory.GetTestCustomers().Content.First();

            var result = await service.RemoveCustomerAsync(customer.Id);

            Assert.IsTrue(result.Succeeded());
            Assert.AreEqual(ResultStatus.Succeeded, result.Status);

            _repositoryMock.Verify(r => r.RemoveCustomerAsync(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task WhenRemoveCustomer_CustomerNotFound_ErrorStatusExpected()
        {
            var service = new CustomerService(_repositoryMock.Object);

            var lastCustomer = DataFactory.GetTestCustomers().Content.LastOrDefault();

            var result = await service.RemoveCustomerAsync(lastCustomer.Id + 1);

            Assert.IsFalse(result.Succeeded());
            Assert.AreEqual(ResultStatus.MissingResource, result.Status);

            _repositoryMock.Verify(r => r.RemoveCustomerAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task WhenRemoveCustomer_ErroFromDB_ErrorStatusExpected()
        {
            var error = Result<IEnumerable<Customer>>.Errored(ResultStatus.InternalError, "Test");

            _repositoryMock.Setup(r => r.GetAllCustomersAsync())
                .ReturnsAsync(error);

            var service = new CustomerService(_repositoryMock.Object);
            
            var result = await service.RemoveCustomerAsync(1);

            Assert.IsFalse(result.Succeeded());
            Assert.AreEqual(error.Status, result.Status);
            Assert.AreEqual(error.ErrorMessage, result.ErrorMessage);

            _repositoryMock.Verify(r => r.RemoveCustomerAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
