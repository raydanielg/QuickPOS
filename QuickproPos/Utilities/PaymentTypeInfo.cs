using QuickproPos.Data.Setting;

namespace QuickproPos.Utilities
{
    public class PaymentTypeInfo
    {
        public List<PaymentType> GetPaymentTypeInfo()
        {
            var info = new List<PaymentType>
            {
                new PaymentType()
                {
                    PaymentTypeId = 1,
                    Name = "Cash"
                },
                new PaymentType()
                {
                    PaymentTypeId = 2,
                    Name = "Card"
                },
                new PaymentType()
                {
                    PaymentTypeId = 3,
                    Name = "UPI"
                }
            };

            return info;
        }
    }
}
