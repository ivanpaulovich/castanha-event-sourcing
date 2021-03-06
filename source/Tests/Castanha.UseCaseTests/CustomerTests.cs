namespace Castanha.Domain.UnitTests
{
    using Xunit;
    using NSubstitute;
    using Castanha.Application;
    using Castanha.Infrastructure.Mappings;
    using Castanha.UseCaseTests;
    using System;
    using Castanha.Application.ServiceBus;
    using Castanha.Application.Repositories;

    public class CustomerTests
    {
        public IAccountReadOnlyRepository accountReadOnlyRepository;
        public IAccountWriteOnlyRepository accountWriteOnlyRepository;
        public ICustomerReadOnlyRepository customerReadOnlyRepository;
        public ICustomerWriteOnlyRepository customerWriteOnlyRepository;

        public IPublisher bus;

        public IOutputConverter converter;

        public CustomerTests()
        {
            accountReadOnlyRepository = Substitute.For<IAccountReadOnlyRepository>();
            accountWriteOnlyRepository = Substitute.For<IAccountWriteOnlyRepository>();
            customerReadOnlyRepository = Substitute.For<ICustomerReadOnlyRepository>();
            customerWriteOnlyRepository = Substitute.For<ICustomerWriteOnlyRepository>();

            bus = Substitute.For<IPublisher>();
            converter = new OutputConverter();
        }

        [Theory]
        [InlineData("08724050601", "Ivan Paulovich", 300)]
        [InlineData("08724050601", "Ivan Paulovich Pinheiro Gomes", 100)]
        [InlineData("444", "Ivan Paulovich", 500)]
        [InlineData("08724050", "Ivan Paulovich", 300)]
        public async void Register_Valid_User_Account(string personnummer, string name, double amount)
        {
            var output = Substitute.For<CustomPresenter<Application.UseCases.Register.RegisterOutput>>();

            var registerUseCase = new Application.UseCases.Register.RegisterInteractor(
                customerReadOnlyRepository,
                accountReadOnlyRepository,
                bus,
                output,
                converter
            );

            var request = new Application.UseCases.Register.RegisterInput(
                personnummer,
                name,
                amount
            );

            await registerUseCase.Process(request);

            Assert.Equal(request.PIN, output.Output.Customer.Personnummer);
            Assert.Equal(request.Name, output.Output.Customer.Name);
            Assert.True(output.Output.Customer.CustomerId != Guid.Empty);
            Assert.True(output.Output.Account.AccountId != Guid.Empty);
        }
    }
}
