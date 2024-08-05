using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        [Fact]
        public async void SetPartnerPromoCodeLimitAsyncTests_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            Fixture fixture = new();

            Guid partnerId = fixture.Create<Guid>();
            Partner partner = null;
            SetPartnerPromoCodeLimitRequest request = fixture.Create<SetPartnerPromoCodeLimitRequest>();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async void SetPartnerPromoCodeLimitAsyncTests_PartnerIsActiveFalse_ReturnsBadRequest()
        {
            // Arrange
            Fixture fixture = new();
            //fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            //fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Guid partnerId = fixture.Create<Guid>();
            Partner partner = fixture
                .Build<Partner>()
                .With(x => x.IsActive, false)
                .Without(x => x.PartnerLimits)
                .Create();
            SetPartnerPromoCodeLimitRequest request = fixture.Create<SetPartnerPromoCodeLimitRequest>();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async void SetPartnerPromoCodeLimitAsyncTests_PartnerPromoCodeLimitHasntCancelDate_PartnerNumberIssuedPromoCodesEqual0()
        {
            // Arrange
            Fixture fixture = new();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Guid partnerId = fixture.Create<Guid>();
            Partner partner = fixture
                .Build<Partner>()
                .With(x => x.NumberIssuedPromoCodes, 5)
                .Without(x => x.PartnerLimits)
                .Create();
            List<PartnerPromoCodeLimit> partnerPromoCodeLimits = fixture.CreateMany<PartnerPromoCodeLimit>(1).Select(x =>
            {
                x.Partner = partner;
                x.PartnerId = partnerId;
                x.CancelDate = null;
                return x;
            }).ToList();
            partner.PartnerLimits = partnerPromoCodeLimits;
            SetPartnerPromoCodeLimitRequest request = fixture.Create<SetPartnerPromoCodeLimitRequest>();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Fact]
        public async void SetPartnerPromoCodeLimitAsyncTests_PartnerPromoCodeLimitHasntCancelDate_TheSamePartnerPromoCodeLimitCancelDateNow()
        {
            // Arrange
            Fixture fixture = new();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Guid partnerId = fixture.Create<Guid>();
            Partner partner = fixture
                .Build<Partner>()
                .With(x => x.NumberIssuedPromoCodes, 5)
                .Without(x => x.PartnerLimits)
                .Create();
            List<PartnerPromoCodeLimit> partnerPromoCodeLimits = fixture.CreateMany<PartnerPromoCodeLimit>(1).Select(x =>
            {
                x.Partner = partner;
                x.PartnerId = partnerId;
                x.CancelDate = null;
                return x;
            }).ToList();
            partner.PartnerLimits = partnerPromoCodeLimits;
            SetPartnerPromoCodeLimitRequest request = fixture.Create<SetPartnerPromoCodeLimitRequest>();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            partnerPromoCodeLimits.FirstOrDefault(x => x.CancelDate.HasValue).CancelDate
                .Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async void SetPartnerPromoCodeLimitAsyncTests_SetPartnerPromoCodeLimitRequestLimit0_ReturnsBadRequest()
        {
            // Arrange
            Fixture fixture = new();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Guid partnerId = fixture.Create<Guid>();
            Partner partner = fixture
                .Build<Partner>()
                .Create();
            SetPartnerPromoCodeLimitRequest request = fixture
                .Build<SetPartnerPromoCodeLimitRequest>()
                .With(x => x.Limit, 0)
                .Create();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async void SetPartnerPromoCodeLimitAsyncTests_PartnerPromoCodeLimitHasntCancelDate_ReturnsNewPartnerPromoCodeLimit()
        {
            // Arrange
            Fixture fixture = new();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Guid partnerId = fixture.Create<Guid>();
            Partner partner = fixture
                .Build<Partner>()
                .Without(x => x.PartnerLimits)
                .Create();
            List<PartnerPromoCodeLimit> partnerPromoCodeLimits = fixture.CreateMany<PartnerPromoCodeLimit>(1).Select(x =>
            {
                x.Partner = partner;
                x.PartnerId = partnerId;
                x.CancelDate = null;
                return x;
            }).ToList();
            partner.PartnerLimits = partnerPromoCodeLimits;
            SetPartnerPromoCodeLimitRequest request = fixture.Create<SetPartnerPromoCodeLimitRequest>();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(partner), Times.Once());
            partner.PartnerLimits.Count.Should().Be(2);
        }
    }
}
