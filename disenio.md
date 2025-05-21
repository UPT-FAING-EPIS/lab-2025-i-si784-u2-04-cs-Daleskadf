```mermaid
classDiagram
  class IAddressInfo {
    <<Interface>>
    Street : string
    Address : string
    City : string
    PostalCode : string
    PhoneNumber : string
  }

  class ICard {
    <<Interface>>
    CardNumber : string
    Name : string
    ValidTo : DateTime
  }

  class ICartItem {
    <<Interface>>
    ProductId : string
    Quantity : int
    Price : double
  }

  class ICartService {
    <<Interface>>
    Total() double
    Items() IEnumerable~ICartItem~
  }

  class IPaymentService {
    <<Interface>>
    Charge(double total, ICard card) bool
  }

  class IShipmentService {
    <<Interface>>
    Ship(IAddressInfo info, IEnumerable~ICartItem~ items) void
  }

  class IDiscountService {
    <<Interface>>
    ApplyDiscount(double totalAmount) double
  }

  class CartController {
    _cartService : ICartService
    _paymentService : IPaymentService
    _shipmentService : IShipmentService
    _discountService : IDiscountService
    CartController(ICartService cartService, IPaymentService paymentService, IShipmentService shipmentService, IDiscountService discountService)
    CheckOut(ICard card, IAddressInfo addressInfo) string
  }

  %% Dependencias directas visibles en el código:

  %% CartController depende de estas interfaces a través de campos (inyección de dependencias)
  CartController --> ICartService
  CartController --> IPaymentService
  CartController --> IShipmentService
  CartController --> IDiscountService

  %% El método CheckOut de CartController usa ICard e IAddressInfo como parámetros
  CartController ..> ICard : CheckOut parameter
  CartController ..> IAddressInfo : CheckOut parameter

  %% ICartService tiene un método Items que retorna IEnumerable<ICartItem>
  ICartService ..> ICartItem : Items() return type

  %% IPaymentService tiene un método Charge que usa ICard como parámetro
  IPaymentService ..> ICard : Charge() parameter

  %% IShipmentService tiene un método Ship que usa IAddressInfo e IEnumerable<ICartItem> como parámetros
  IShipmentService ..> IAddressInfo : Ship() parameter
  IShipmentService ..> ICartItem : Ship() parameter

  ```