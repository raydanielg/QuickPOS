using QuickproPos.Data.InventoryModel;
using QuickproPos.Data.SettingView;
using SQLite;

namespace QuickproPos.Utilities
{
    public static class CalculationsInfo
    {
        public static async void CalculateRowAmounts(this ProductView product, CompanyView company)
        {
            // Check for valid ProductId
            if (product.ProductId > 0)
            {
                // Ensure non-negative values for essential fields
                product.Qty = Math.Max(product.Qty, 0);
                product.PurchaseRate = Math.Max(product.PurchaseRate, 0);
                product.Discount = Math.Max(product.Discount, 0);
                product.FlatDiscount = Math.Max(product.FlatDiscount, 0);

                // Calculate base discounted rate
                var discountedRate = Math.Round(product.PurchaseRate * (1 - product.Discount / 100), company.NoofDecimal);
                discountedRate = Math.Round(discountedRate * (1 - product.FlatDiscount / 100), company.NoofDecimal);

                // Perform tax calculation based on "Including" or "Excluding" mode
                if (product.Includingpurchasetax == "Excluding")
                {
                    // Tax is excluded; calculate gross and add tax
                    product.GrossAmount = Math.Round(discountedRate * product.Qty, company.NoofDecimal);
                    product.TaxAmount = Math.Round(product.GrossAmount * product.TaxRate / 100, company.NoofDecimal);
                    product.NetAmount = Math.Round(product.GrossAmount + product.TaxAmount, company.NoofDecimal);
                }
                else if (product.Includingpurchasetax == "Including")
                {
                    // Tax is included; reverse-calculate gross and separate tax
                    var grossIncludingTax = Math.Round(discountedRate * product.Qty, company.NoofDecimal);
                    product.TaxAmount = Math.Round(grossIncludingTax * product.TaxRate / (100 + product.TaxRate), company.NoofDecimal);
                    product.GrossAmount = Math.Round(grossIncludingTax - product.TaxAmount, company.NoofDecimal);
                    product.NetAmount = grossIncludingTax;  // Net amount includes tax already
                }

                // Calculate total discount amount for transparency in accounting
                product.DiscountAmount = Math.Round((product.PurchaseRate - discountedRate) * product.Qty, company.NoofDecimal);

                // Calculate total amount pre-tax for records and accounting audits
                product.Amount = Math.Round(discountedRate * product.Qty, company.NoofDecimal);
            }
            else
            {
                // Reset amounts if ProductId is invalid (standard accounting reset)
                product.Qty = 0;
                product.DiscountAmount = 0;
                product.GrossAmount = 0;
                product.TaxAmount = 0;
                product.NetAmount = 0;
                product.Amount = 0;
            }
        }


        public static async void CalculateRowSalesAmounts(this ProductView product, CompanyView company)
        {
            // Check for valid ProductId
            if (product.ProductId > 0)
            {
                // Ensure non-negative values for essential fields
                product.Qty = Math.Max(product.Qty, 0);
                product.SalesRate = Math.Max(product.SalesRate, 0);
                product.Discount = Math.Max(product.Discount, 0);
                product.FlatDiscount = Math.Max(product.FlatDiscount, 0);

                // Calculate base discounted rate
                var discountedRate = Math.Round(product.SalesRate * (1 - product.Discount / 100), company.NoofDecimal);
                discountedRate = Math.Round(discountedRate * (1 - product.FlatDiscount / 100), company.NoofDecimal);

                // Perform tax calculation based on "Including" or "Excluding" mode
                if (product.Includingsalestax == "Excluding")
                {
                    // Tax is excluded; calculate gross and add tax
                    product.GrossAmount = Math.Round(discountedRate * product.Qty, company.NoofDecimal);
                    product.TaxAmount = Math.Round(product.GrossAmount * product.TaxRate / 100, company.NoofDecimal);
                    product.NetAmount = Math.Round(product.GrossAmount + product.TaxAmount, company.NoofDecimal);
                }
                else if (product.Includingsalestax == "Including")
                {
                    // Tax is included; reverse-calculate gross and separate tax
                    var grossIncludingTax = Math.Round(discountedRate * product.Qty, company.NoofDecimal);
                    product.TaxAmount = Math.Round(grossIncludingTax * product.TaxRate / (100 + product.TaxRate), company.NoofDecimal);
                    product.GrossAmount = Math.Round(grossIncludingTax - product.TaxAmount, company.NoofDecimal);
                    product.NetAmount = grossIncludingTax;  // Net amount includes tax already
                }

                // Calculate total discount amount for transparency in accounting
                product.DiscountAmount = Math.Round((product.SalesRate - discountedRate) * product.Qty, company.NoofDecimal);

                // Calculate total amount pre-tax for records and accounting audits
                product.Amount = Math.Round(discountedRate * product.Qty, company.NoofDecimal);
            }
            else
            {
                // Reset amounts if ProductId is invalid (standard accounting reset)
                product.Qty = 0;
                product.DiscountAmount = 0;
                product.GrossAmount = 0;
                product.TaxAmount = 0;
                product.NetAmount = 0;
                product.Amount = 0;
            }
        }
        public static async Task CalculateSales(this SalesMaster master, List<ProductView> products, CompanyView company)
        {
            decimal decTaxAmount = 0;
            decimal decDiscountAmount = 0;
            decimal decTotal = 0;
            decimal decNetAmount = 0;
            decimal decGrossAmount = 0;

            // Calculate sales amounts
            foreach (var product in products)
            {
                decGrossAmount += product.GrossAmount;
                decNetAmount += product.NetAmount;
                decTaxAmount += product.TaxAmount;
                decDiscountAmount += product.DiscountAmount;
                decTotal += product.Amount;
            }

            // Assign calculated values to the master object
            master.TotalAmount = Math.Round(decTotal, company.NoofDecimal);
            master.BillDiscount = Math.Round(decDiscountAmount, company.NoofDecimal);
            master.NetAmounts = Math.Round(decNetAmount, company.NoofDecimal);
            master.TotalTax = Math.Round(decTaxAmount, company.NoofDecimal);

            // Calculate grand total
            var grandTotal = Math.Round(master.NetAmounts + master.TotalTax - master.CouponAmount + master.ShippingAmount, company.NoofDecimal);

            // Handle rounding
            if (company.IsRound)
            {
                master.RoundOff = Math.Round(grandTotal) - grandTotal;
                master.GrandTotal = Math.Round(grandTotal);
            }
            else
            {
                master.RoundOff = 0;
                master.GrandTotal = Math.Round(grandTotal, company.NoofDecimal);
            }
        }
        public static void CalculatePurchase(this PurchaseMaster master, List<ProductView> products, CompanyView company)
        {
            decimal decTaxAmount = 0;
            decimal decDiscountAmount = 0;
            decimal decTotal = 0;
            decimal decNetAmount = 0;
            decimal decGrossAmount = 0;

            foreach (var product in products)
            {
                decGrossAmount += product.GrossAmount;
                decNetAmount += product.NetAmount;
                decTaxAmount += product.TaxAmount;
                decDiscountAmount += product.DiscountAmount;
                decTotal += product.Amount;
            }

            master.TotalAmount = Math.Round(decTotal, company.NoofDecimal);
            master.BillDiscount = Math.Round(decDiscountAmount, company.NoofDecimal);
            master.NetAmounts = Math.Round(decNetAmount, company.NoofDecimal);
            master.TotalTax = Math.Round(decTaxAmount, company.NoofDecimal);
            var grandTotal = Math.Round(master.NetAmounts + master.TotalTax + master.ShippingAmount, company.NoofDecimal);

            if (company.IsRound)
            {
                master.RoundOff = Math.Round(grandTotal) - grandTotal;
                master.GrandTotal = Math.Round(grandTotal);
            }
            else
            {
                master.RoundOff = 0;
                master.GrandTotal = Math.Round(grandTotal, company.NoofDecimal);
            }
        }
        public static void CalculatePurchaseReturn(this PurchaseReturnMaster master, List<ProductView> products, CompanyView company)
        {
            decimal decTaxAmount = 0;
            decimal decDiscountAmount = 0;
            decimal decTotal = 0;
            decimal decNetAmount = 0;
            decimal decGrossAmount = 0;

            foreach (var product in products)
            {
                decGrossAmount += product.GrossAmount;
                decNetAmount += product.NetAmount;
                decTaxAmount += product.TaxAmount;
                decDiscountAmount += product.DiscountAmount;
                decTotal = product.Amount;
            }

            master.TotalAmount = Math.Round(decTotal, company.NoofDecimal);
            master.BillDiscount = Math.Round(decDiscountAmount, company.NoofDecimal);
            master.NetAmounts = Math.Round(decNetAmount, company.NoofDecimal);
            master.TotalTax = Math.Round(decTaxAmount, company.NoofDecimal);
            var grandTotal = Math.Round(master.NetAmounts + master.TotalTax + master.ShippingAmount, company.NoofDecimal);

            if (company.IsRound)
            {
                master.RoundOff = Math.Round(grandTotal) - grandTotal;
                master.GrandTotal = Math.Round(grandTotal);
            }
            else
            {
                master.RoundOff = 0;
                master.GrandTotal = Math.Round(grandTotal, company.NoofDecimal);
            }
        }
        public static void CalculateSalesReturn(this SalesReturnMaster master, List<ProductView> products, CompanyView company)
        {
            decimal decTaxAmount = 0;
            decimal decDiscountAmount = 0;
            decimal decTotal = 0;
            decimal decNetAmount = 0;
            decimal decGrossAmount = 0;

            foreach (var product in products)
            {
                decGrossAmount += product.GrossAmount;
                decNetAmount += product.NetAmount;
                decTaxAmount += product.TaxAmount;
                decDiscountAmount += product.DiscountAmount;
                decTotal = product.Amount;
            }

            master.TotalAmount = Math.Round(decTotal, company.NoofDecimal);
            master.BillDiscount = Math.Round(decDiscountAmount, company.NoofDecimal);
            master.NetAmounts = Math.Round(decNetAmount, company.NoofDecimal);
            master.TotalTax = Math.Round(decTaxAmount, company.NoofDecimal);
            var grandTotal = Math.Round(master.NetAmounts + master.TotalTax + master.ShippingAmount, company.NoofDecimal);

            if (company.IsRound)
            {
                master.RoundOff = Math.Round(grandTotal) - grandTotal;
                master.GrandTotal = Math.Round(grandTotal);
            }
            else
            {
                master.RoundOff = 0;
                master.GrandTotal = Math.Round(grandTotal, company.NoofDecimal);
            }
        }
        public static void CalculateStockadjustmentRow(this ProductView product, CompanyView company)
        {
            if (product.ProductId > 0)
            {
                if (product.Qty <= 0)
                {
                    product.Qty = 0;
                }
                product.Rate = Math.Round(product.PurchaseRate, company.NoofDecimal);
                product.Amount = Math.Round(product.PurchaseRate * product.Qty, company.NoofDecimal);
            }
        }
        public static async void CalculateTileQuotationAmounts(this ProductView product, CompanyView company)
        {
            if (product.ProductId > 0)
            {
                if (product.Qty <= 0)
                {
                    product.Qty = 0;
                }
                if (product.Rate <= 0)
                {
                    product.Rate = 0;
                }
                if (product.SizeLength <= 0)
                {
                    product.SizeWidth = 0;
                }
                if (product.BoxQty <= 0)
                {
                    product.BoxQty = 0;
                }
                decimal decTotalSquarFeet = 0;
                    product.TotalSqFt = Math.Round(product.SizeLength * product.SizeWidth * product.Qty, company.NoofDecimal);
                    product.Amount = Math.Round(product.BoxQty * product.Rate, company.NoofDecimal);
            }
            else
            {
                product.Qty = 0;
                product.ProductId = 0;
                product.UnitId = 0;
                product.SizeLength = 0;
                product.SizeWidth = 0;
                product.TotalSqFt = 0;
                product.BoxQty = 0;
                product.Rate = 0;
                product.Amount = 0;
            }
        }
        public static void CalculateTileQuotation(this TilesQuotationMaster master, List<ProductView> products, CompanyView company)
        {
            decimal decTotalSquarFeet = 0;
            decimal decBoxQty = 0;
            decimal decTotalAmount = 0;
            decimal decDiscountAmount = 0;

            foreach (var product in products)
            {
                decTotalSquarFeet += product.TotalSqFt;
                decBoxQty += product.BoxQty;
                decTotalAmount += product.Amount;
            }
            master.TotalSqft = Math.Round(decTotalSquarFeet, company.NoofDecimal);
            master.TotalBox = Math.Round(decBoxQty, company.NoofDecimal);
            master.TotalAmount = Math.Round(decTotalAmount, company.NoofDecimal);
            if(master.TotalAmount > master.DiscountAmount)
            {
                master.GrandTotal = Math.Round(master.TotalAmount - master.DiscountAmount);
            }
            else
            {
                master.DiscountAmount = 0;
            }
            if(master.GrandTotal > master.AdvanceAmount)
            {
                master.PendingAmount = Math.Round(master.GrandTotal - master.AdvanceAmount, company.NoofDecimal);
            }
            else
            {
                master.PendingAmount = 0;
            }
        }
        public static async void CalculateGraniteQuotationAmounts(this ProductView product, CompanyView company)
        {
            if (product.ProductId > 0)
            {
                if (product.Qty <= 0)
                {
                    product.Qty = 0;
                }
                if (product.Rate <= 0)
                {
                    product.Rate = 0;
                }
                if (product.SizeLength <= 0)
                {
                    product.SizeWidth = 0;
                }
                if (product.TotalSqFt <= 0)
                {
                    product.TotalSqFt = 0;
                }
                decimal decTotalSquarFeet = Math.Round((product.SizeLength + 1)*(product.SizeWidth + 1) * product.Qty / 144 , company.NoofDecimal);
                product.TotalSqFt = Math.Round(decTotalSquarFeet, company.NoofDecimal);
                product.Amount = Math.Round(decTotalSquarFeet * product.Rate, company.NoofDecimal);
            }
            else
            {
                product.Qty = 0;
                product.ProductId = 0;
                product.UnitId = 0;
                product.SizeLength = 0;
                product.SizeWidth = 0;
                product.TotalSqFt = 0;
                product.Rate = 0;
                product.Amount = 0;
            }
        }
        public static void CalculationGraniteQuotation(this GreniteQuotation master, List<ProductView> products, CompanyView company)
        {
            decimal decTotalSquarFeet = 0;
            decimal decBoxQty = 0;
            decimal decTotalAmount = 0;
            decimal decDiscountAmount = 0;

            foreach (var product in products)
            {
                decTotalSquarFeet += product.TotalSqFt;
                decBoxQty += product.Qty;
                decTotalAmount += product.Amount;
            }
            master.TotalSqft = Math.Round(decTotalSquarFeet, company.NoofDecimal);
            master.TotalQty = Math.Round(decBoxQty, company.NoofDecimal);
            master.TotalAmount = Math.Round(decTotalAmount, company.NoofDecimal);
            if (master.TotalAmount > master.DiscountAmount)
            {
                master.GrandTotal = Math.Round(master.TotalAmount - master.DiscountAmount);
            }
            else
            {
                master.DiscountAmount = 0;
            }
            if (master.GrandTotal > master.AdvanceAmount)
            {
                master.PendingAmount = Math.Round(master.GrandTotal - master.AdvanceAmount, company.NoofDecimal);
            }
            else
            {
                master.PendingAmount = 0;
            }
        }
    }
}
