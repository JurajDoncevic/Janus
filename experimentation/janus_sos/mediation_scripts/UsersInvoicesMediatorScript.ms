SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE UsersInvoicesData VERSION "1.0"
    WITH SCHEMA Main
        WITH TABLEAU Employees
            WITH ATTRIBUTES
                EmployeeId,
                LastName,
                FirstName,
                Title,
                ReportsTo,
                BirthDate,
                HireDate,
                Address,
                City,
                State,
                Country,
                PostalCode,
                Phone,
                Fax,
                Email
            BEING
                SELECT
                    UsersInvoicesData.main.employees.EmployeeId,
                    UsersInvoicesData.main.employees.LastName,
                    UsersInvoicesData.main.employees.FirstName,
                    UsersInvoicesData.main.employees.Title,
                    UsersInvoicesData.main.employees.ReportsTo,
                    UsersInvoicesData.main.employees.BirthDate,
                    UsersInvoicesData.main.employees.HireDate,
                    UsersInvoicesData.main.employees.Address,
                    UsersInvoicesData.main.employees.City,
                    UsersInvoicesData.main.employees.State,
                    UsersInvoicesData.main.employees.Country,
                    UsersInvoicesData.main.employees.PostalCode,
                    UsersInvoicesData.main.employees.Phone,
                    UsersInvoicesData.main.employees.Fax,
                    UsersInvoicesData.main.employees.Email
                FROM UsersInvoicesData.main.employees
        WITH TABLEAU Customers
            WITH ATTRIBUTES
                CustomerId,
                FirstName,
                LastName,
                Company,
                Address,
                City,
                State,
                Country,
                PostalCode,
                Phone,
                Fax,
                Email,
                SupportRepId                    
            BEING
                SELECT
                    UsersInvoicesData.main.customers.CustomerId,
                    UsersInvoicesData.main.customers.FirstName,
                    UsersInvoicesData.main.customers.LastName,
                    UsersInvoicesData.main.customers.Company,
                    UsersInvoicesData.main.customers.Address,
                    UsersInvoicesData.main.customers.City,
                    UsersInvoicesData.main.customers.State,
                    UsersInvoicesData.main.customers.Country,
                    UsersInvoicesData.main.customers.PostalCode,
                    UsersInvoicesData.main.customers.Phone,
                    UsersInvoicesData.main.customers.Fax,
                    UsersInvoicesData.main.customers.Email,
                    UsersInvoicesData.main.customers.SupportRepId                
                FROM UsersInvoicesData.main.customers
        WITH TABLEAU Invoices
            WITH ATTRIBUTES
                InvoiceId,
                CustomerId,
                InvoiceDate,
                BillingAddress,
                BillingCity,
                BillingState,
                BillingCountry,
                BillingPostalCode,
                Total
            BEING
                SELECT
                    UsersInvoicesData.main.invoices.InvoiceId,
                    UsersInvoicesData.main.invoices.CustomerId,
                    UsersInvoicesData.main.invoices.InvoiceDate,
                    UsersInvoicesData.main.invoices.BillingAddress,
                    UsersInvoicesData.main.invoices.BillingCity,
                    UsersInvoicesData.main.invoices.BillingState,
                    UsersInvoicesData.main.invoices.BillingCountry,
                    UsersInvoicesData.main.invoices.BillingPostalCode,
                    UsersInvoicesData.main.invoices.Total
                FROM UsersInvoicesData.main.invoices
        WITH TABLEAU InvoiceItems
            WITH ATTRIBUTES
                InvoiceLineId,
                InvoiceId,
                TrackId,
                UnitPrice,
                Quantity
            BEING
                SELECT
                    UsersInvoicesData.main.invoice_items.InvoiceLineId,
                    UsersInvoicesData.main.invoice_items.InvoiceId,
                    UsersInvoicesData.main.invoice_items.TrackId,
                    UsersInvoicesData.main.invoice_items.UnitPrice,
                    UsersInvoicesData.main.invoice_items.Quantity
                FROM UsersInvoicesData.main.invoice_items
