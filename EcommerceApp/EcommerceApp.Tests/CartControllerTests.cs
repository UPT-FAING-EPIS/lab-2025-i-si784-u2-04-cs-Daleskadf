// EcommerceApp.Tests/CartControllerTests.cs
using EcommerceApp.Api.Controllers;
using EcommerceApp.Api.Models;
using EcommerceApp.Api.Services;
using Moq;

namespace EcommerceApp.Tests;

public class CartControllerTests
{
    private CartController controller;
    private Mock<IPaymentService> paymentServiceMock;
    private Mock<ICartService> cartServiceMock;
    private Mock<IShipmentService> shipmentServiceMock;
    private Mock<IDiscountService> discountServiceMock;
    private Mock<ICard> cardMock;
    private Mock<IAddressInfo> addressInfoMock;
    private List<ICartItem> items;

    private const double DefaultCartTotal = 100.0;
    private const double DefaultDiscountedTotal = 90.0;

    [SetUp]
    public void Setup()
    {
        cartServiceMock = new Mock<ICartService>();
        paymentServiceMock = new Mock<IPaymentService>();
        shipmentServiceMock = new Mock<IShipmentService>();
        discountServiceMock = new Mock<IDiscountService>();

        cardMock = new Mock<ICard>();
        addressInfoMock = new Mock<IAddressInfo>();

        var cartItemMock = new Mock<ICartItem>();
        cartItemMock.Setup(item => item.Price).Returns(50);
        cartItemMock.Setup(item => item.Quantity).Returns(2);

        items = new List<ICartItem>()
        {
            cartItemMock.Object
        };

        cartServiceMock.Setup(c => c.Items()).Returns(items.AsEnumerable());
        cartServiceMock.Setup(c => c.Total()).Returns(DefaultCartTotal);

        discountServiceMock.Setup(d => d.ApplyDiscount(DefaultCartTotal)).Returns(DefaultDiscountedTotal);

        controller = new CartController(
            cartServiceMock.Object,
            paymentServiceMock.Object,
            shipmentServiceMock.Object,
            discountServiceMock.Object
        );
    }

    [Test]
    public void ShouldReturnCharged()
    {
        string expected = "charged";
        paymentServiceMock.Setup(p => p.Charge(DefaultDiscountedTotal, cardMock.Object)).Returns(true);

        var result = controller.CheckOut(cardMock.Object, addressInfoMock.Object);

        discountServiceMock.Verify(d => d.ApplyDiscount(DefaultCartTotal), Times.Once());
        paymentServiceMock.Verify(p => p.Charge(DefaultDiscountedTotal, cardMock.Object), Times.Once());
        shipmentServiceMock.Verify(s => s.Ship(addressInfoMock.Object, items.AsEnumerable()), Times.Once());
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldReturnNotCharged()
    {
        string expected = "not charged";
        paymentServiceMock.Setup(p => p.Charge(DefaultDiscountedTotal, cardMock.Object)).Returns(false);

        var result = controller.CheckOut(cardMock.Object, addressInfoMock.Object);

        discountServiceMock.Verify(d => d.ApplyDiscount(DefaultCartTotal), Times.Once());
        paymentServiceMock.Verify(p => p.Charge(DefaultDiscountedTotal, cardMock.Object), Times.Once());
        shipmentServiceMock.Verify(s => s.Ship(addressInfoMock.Object, items.AsEnumerable()), Times.Never());
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(true, "charged", 1, TestName = "Checkout_PaymentSucceeds_ReturnsChargedAndShips")] // 1 significa Times.Once()
    [TestCase(false, "not charged", 0, TestName = "Checkout_PaymentFails_ReturnsNotChargedAndDoesNotShip")] // 0 significa Times.Never()
    public void CheckOut_ShouldReturnExpectedResult_WhenPaymentStatusIs(
        bool paymentSuccess,
        string expectedMessage,
        int expectedShipmentCallCount) 
    {
        paymentServiceMock.Setup(p => p.Charge(DefaultDiscountedTotal, cardMock.Object))
                          .Returns(paymentSuccess);

        Times shipmentVerificationTimes;
        if (expectedShipmentCallCount == 1)
        {
            shipmentVerificationTimes = Times.Once();
        }
        else if (expectedShipmentCallCount == 0)
        {
            shipmentVerificationTimes = Times.Never();
        }
        else
        {
            shipmentVerificationTimes = Times.Exactly(expectedShipmentCallCount);
        }


        var result = controller.CheckOut(cardMock.Object, addressInfoMock.Object);

        Assert.That(result, Is.EqualTo(expectedMessage));

        discountServiceMock.Verify(d => d.ApplyDiscount(DefaultCartTotal), Times.Once());
        paymentServiceMock.Verify(p => p.Charge(DefaultDiscountedTotal, cardMock.Object), Times.Once());
        
        shipmentServiceMock.Verify(s => s.Ship(addressInfoMock.Object, items.AsEnumerable()), shipmentVerificationTimes);
    }
}