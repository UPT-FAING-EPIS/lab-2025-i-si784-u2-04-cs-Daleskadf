namespace EcommerceApp.Api.Services;

public interface IDiscountService
{
    /// <summary>
    /// Aplica un descuento al monto total proporcionado.
    /// </summary>
    /// <param name="totalAmount">El monto total original.</param>
    /// <returns>El monto total después de aplicar el descuento.</returns>
    double ApplyDiscount(double totalAmount);
}