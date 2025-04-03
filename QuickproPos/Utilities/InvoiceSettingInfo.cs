using QuickproPos.Data.Setting;

namespace QuickproPos.Utilities
{
    public class InvoiceSettingInfo
    {
        public List<InvoiceSetting> GetInvoiceSettingInfo()
        {
            var info = new List<InvoiceSetting>();
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 1,
                VoucherTypeName = "Opening Balance",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });

            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 2,
                VoucherTypeName = "Opening Stock",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 3,
                VoucherTypeName = "Receipt Voucher",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 4,
                VoucherTypeName = "Payment Voucher",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 5,
                VoucherTypeName = "Journal Voucher",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 6,
                VoucherTypeName = "Bank Transaction",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 7,
                VoucherTypeName = "Purchase Order",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 8,
                VoucherTypeName = "Purchase Invoice",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 9,
                VoucherTypeName = "Purchase Return",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 10,
                VoucherTypeName = "Sales Quotation",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 11,
                VoucherTypeName = "Sales Order",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 12,
                VoucherTypeName = "Sales Invoice",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 13,
                VoucherTypeName = "Sales Return",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 14,
                VoucherTypeName = "Expense Voucher",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 15,
                VoucherTypeName = "Salary Voucher",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 16,
                VoucherTypeName = "Advance Payment",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 17,
                VoucherTypeName = "DailySalary Voucher",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 18,
                VoucherTypeName = "Stock Transfer",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 19,
                VoucherTypeName = "Stock Adjustment",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 20,
                VoucherTypeName = "Tiles Quotation",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            info.Add(new InvoiceSetting()
            {
                VoucherTypeId = 21,
                VoucherTypeName = "Grentie Quotation",
                Suffix = string.Empty,
                Prefix = string.Empty,
                StartIndex = 1
            });
            return info;
        }
    }
}
