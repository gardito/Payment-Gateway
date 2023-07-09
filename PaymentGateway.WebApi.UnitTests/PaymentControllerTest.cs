using System.Globalization;
using Cache.Common;
using Common;
using Domain.Services;
using Domain.UnitTests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Payment.Service;
using PaymentGateway.WebApi.Controllers;
using PaymentGateway.WebApi.DTOs;
using Service.Banking.CkoBankSimulator.Errors;
using Service.Common;

namespace PaymentGateway.WebApi.UnitTests;

public class PaymentControllerTest
{
    public class GetPaymentById
    {
        public class OnInvalidInput
        {
            private static readonly Mock<IPaymentService> _paymentService = new Mock<IPaymentService>();
            private static readonly Mock<IIdempotencyCachingSystem> _idempotencyCheckSystem = new Mock<IIdempotencyCachingSystem>();
            private static readonly PaymentController _paymentController = new PaymentController(_paymentService.Object, _idempotencyCheckSystem.Object);

            [Fact]
            public async Task ReturnsBadRequestOnEmptyString()
            {
                var res = await _paymentController.GetPaymentById("");
                Assert.IsType<BadRequestObjectResult>(res.Result);
                Assert.Equal("Empty id", ((BadRequestObjectResult)res.Result).Value);
            }
            
            [Fact]
            public async Task ReturnsBadRequestOnInvalidGuid()
            {
                var res = await _paymentController.GetPaymentById("invalid-guid");
                Assert.IsType<BadRequestObjectResult>(res.Result);
                Assert.Equal("Invalid id format", ((BadRequestObjectResult)res.Result).Value);
            }
        }

        public class OnMissingPayment
        {
            private readonly Guid _guid = Guid.NewGuid();
            private static readonly Mock<IPaymentService> _paymentService = new Mock<IPaymentService>();
            private static readonly Mock<IIdempotencyCachingSystem> _idempotencyCheckSystem = new Mock<IIdempotencyCachingSystem>();
            private static readonly PaymentController _paymentController = new PaymentController(_paymentService.Object, _idempotencyCheckSystem.Object);

            public OnMissingPayment()
            {
                
                _paymentService
                    .Setup(service => service.GetPaymentAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(new ErrorResult<Domain.Payment>(new PaymentError("payment not found")));
            }
            
            [Fact]
            public async Task ReturnsBadRequestResult()
            {
                var res = await _paymentController.GetPaymentById(_guid.ToString());

                _paymentService.Verify(service => service.GetPaymentAsync(It.IsAny<Guid>()), Times.Once);
                Assert.IsType<NotFoundObjectResult>(res.Result);
                Assert.Equal("payment not found", ((NotFoundObjectResult)res.Result).Value);
            }
        }
        
        public class OnException
        {
            private readonly Guid _guid = Guid.NewGuid();
            private static readonly Mock<IPaymentService> _paymentService = new Mock<IPaymentService>();
            private static readonly Mock<IIdempotencyCachingSystem> _idempotencyCheckSystem = new Mock<IIdempotencyCachingSystem>();
            private static readonly PaymentController _paymentController = new PaymentController(_paymentService.Object, _idempotencyCheckSystem.Object);

            public OnException()
            {
                _paymentService
                    .Setup(service => service.GetPaymentAsync(It.IsAny<Guid>()))
                    .Throws(new Exception("Something went wrong"));
            }
            
            [Fact]
            public async Task ReturnsBadRequestResult()
            {
                var res = await _paymentController.GetPaymentById(_guid.ToString());

                _paymentService.Verify(service => service.GetPaymentAsync(It.IsAny<Guid>()), Times.Once);
                Assert.IsType<BadRequestObjectResult>(res.Result);
                Assert.Equal("Unknown error", ((BadRequestObjectResult)res.Result).Value);
            }
        }
        
        public class OnExistingPayment
        {
            private readonly Guid _guid = Guid.NewGuid();
            private Domain.Payment _paymentResult = PaymentHelper.MockPaymentResult(DateTime.Now).Data;
            private static readonly Mock<IPaymentService> _paymentService = new Mock<IPaymentService>();
            private static readonly Mock<IIdempotencyCachingSystem> _idempotencyCheckSystem = new Mock<IIdempotencyCachingSystem>();
            private static readonly PaymentController _paymentController = new PaymentController(_paymentService.Object, _idempotencyCheckSystem.Object);

            public OnExistingPayment()
            {
                _paymentService
                    .Setup(service => service.GetPaymentAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(new SuccessfulResult<Domain.Payment>(_paymentResult));
            }
            
            [Fact]
            public async Task ReturnsOkResult()
            {
                var res = await _paymentController.GetPaymentById(_guid.ToString());

                var expectedDto = new PaymentDto()
                {
                    Id = _paymentResult.Id.ToString(),
                    TransactionDate = _paymentResult.Date.ToString(CultureInfo.InvariantCulture),
                    MerchantId = _paymentResult.Merchant.Id,
                    MerchantName = _paymentResult.Merchant.Name,
                    Amount = _paymentResult.Transaction.Amount,
                    Currency = _paymentResult.Transaction.Currency.ToString(),
                    CardNumber = _paymentResult.Card.GetMaskedNumber(),
                    Status = _paymentResult.Status.ToString()
                };
                
                _paymentService.Verify(service => service.GetPaymentAsync(It.IsAny<Guid>()), Times.Once);
                Assert.IsType<OkObjectResult>(res.Result);
                Assert.IsType<PaymentDto>(((OkObjectResult)res.Result).Value);
                Assert.Equivalent(expectedDto, ((OkObjectResult)res.Result).Value);
            }
        }
    }

    public class ProcessPayment
    {
        public class OnProcessedPayment
        {
            private static readonly Mock<IPaymentService> _paymentService = new Mock<IPaymentService>();
            private static readonly Mock<IIdempotencyCachingSystem> _idempotencyCheckSystem = new Mock<IIdempotencyCachingSystem>();
            private static readonly PaymentController _paymentController = new PaymentController(_paymentService.Object, _idempotencyCheckSystem.Object);

            public OnProcessedPayment()
            {
                _idempotencyCheckSystem
                    .Setup(system => system.Contains(It.IsAny<Guid>()))
                    .ReturnsAsync(true);
            }

            [Fact]
            public async Task ReturnsBadRequestResult()
            {
                var res = await _paymentController.ProcessPayment(BuildInputPaymentDto(Guid.NewGuid()));

                Assert.IsType<BadRequestObjectResult>(res);
                Assert.Equal("Payment processed already", ((BadRequestObjectResult)res).Value);
            }
        }
        
        public class OnSuccessfulPaymentToProcess
        {
            private static readonly Mock<IPaymentService> _paymentService = new Mock<IPaymentService>();
            private static readonly Mock<IIdempotencyCachingSystem> _idempotencyCheckSystem = new Mock<IIdempotencyCachingSystem>();
            private static readonly PaymentController _paymentController = new PaymentController(_paymentService.Object, _idempotencyCheckSystem.Object);

            public OnSuccessfulPaymentToProcess()
            {
                _idempotencyCheckSystem
                    .Setup(system => system.Contains(It.IsAny<Guid>()))
                    .ReturnsAsync(false);

                _paymentService
                    .Setup(service => service.ProcessPaymentAsync(It.IsAny<Domain.Payment>()))
                    .ReturnsAsync(
                        new SuccessfulResult<BankPaymentResult>(
                            new BankPaymentResult 
                            { 
                                PaymentId = Guid.NewGuid(), 
                                Status = Status.Approved 
                            })
                        );
            }

            [Fact]
            public async Task ReturnsOkRequestResult()
            {
                var inputDto = BuildInputPaymentDto(Guid.NewGuid());
                var res = await _paymentController.ProcessPayment(inputDto);

                _paymentService.Verify(service => service.ProcessPaymentAsync(It.IsAny<Domain.Payment>()), Times.Once);
                _idempotencyCheckSystem.Verify(system => system.Add(It.IsAny<Guid>()), Times.Once);
                Assert.IsType<OkObjectResult>(res);
            }
        }

        public class OnFailedPaymentToProcess
        {
            private static readonly Mock<IPaymentService> _paymentService = new Mock<IPaymentService>();
            private static readonly Mock<IIdempotencyCachingSystem> _idempotencyCheckSystem = new Mock<IIdempotencyCachingSystem>();
            private static readonly PaymentController _paymentController = new PaymentController(_paymentService.Object, _idempotencyCheckSystem.Object);

            public OnFailedPaymentToProcess()
            {
                _idempotencyCheckSystem
                    .Setup(system => system.Contains(It.IsAny<Guid>()))
                    .ReturnsAsync(false);

                _paymentService
                    .Setup(service => service.ProcessPaymentAsync(It.IsAny<Domain.Payment>()))
                    .ReturnsAsync(
                        new ErrorResult<BankPaymentResult>(new BankError("Payment declined")));
            }

            [Fact]
            public async Task ReturnsBadRequestResult()
            {
                var inputDto = BuildInputPaymentDto(Guid.NewGuid());
                var res = await _paymentController.ProcessPayment(inputDto);

                _paymentService.Verify(service => service.ProcessPaymentAsync(It.IsAny<Domain.Payment>()), Times.Once);
                _idempotencyCheckSystem.Verify(system => system.Add(It.IsAny<Guid>()), Times.Once);
                Assert.IsType<BadRequestObjectResult>(res);
            }
        }

        private static InputPaymentDto BuildInputPaymentDto(Guid idempotencyId)
        {
            var card = new InputCardDto
            {
                OwnerFullName = "John Doe",
                CardNumber = "2434 1234 1234 1234",
                Cvv = "123",
                ExpiryMonth = "07",
                ExpiryYear = "2030"
            };
            
            return new InputPaymentDto()
            {
                IdempotencyId = idempotencyId,
                MerchantId = 1,
                MerchantName = "Merchant Name",
                Amount = 50,
                Currency = "GBP",
                Card = card
            };
        }
    }
}