SETTING
    PROPAGATE UPDATE SETS
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE InvoicingData VERSION "1.0" #Mediated data about user invoices#
    WITH SCHEMA Main
        WITH TABLEAU Users #Data about users#
            WITH ATTRIBUTES
                UserId,
                UserFirstName,
                UserLastName,
                UserEmail
            BEING
                SELECT
                    UsersInvoicesData.main.customers.CustomerId,
                    UsersInvoicesData.main.customers.FirstName,
                    UsersInvoicesData.main.customers.LastName,
                    UsersInvoicesData.main.customers.Email
                FROM UsersInvoicesData.main.customers
        WITH TABLEAU UserInvoices #Data about invoices for users#
            WITH ATTRIBUTES
                InvoiceId,
                UserId,
                UserEmail,
                InvoiceDate,
                InvoiceTotal
            BEING
                SELECT
                    UsersInvoicesData.main.invoices.InvoiceId,
                    UsersInvoicesData.main.customers.CustomerId,
                    UsersInvoicesData.main.customers.Email,
                    UsersInvoicesData.main.invoices.InvoiceDate,
                    UsersInvoicesData.main.invoices.Total
                FROM UsersInvoicesData.main.invoices
                JOIN UsersInvoicesData.main.customers
                    ON UsersInvoicesData.main.invoices.CustomerId == UsersInvoicesData.main.customers.CustomerId
        WITH TABLEAU UserListenedTracks #Data about tracks listend by users provided by billing#
            WITH ATTRIBUTES
                InvoiceItemId,
                UserEmail,
                TrackName,
                Quantity
            BEING
                SELECT
                   UsersInvoicesData.main.invoice_items.InvoiceLineId,
                   UsersInvoicesData.main.customers.Email,
                   TracksData.main.tracks.Name,
                   UsersInvoicesData.main.invoice_items.Quantity
                FROM UsersInvoicesData.main.invoice_items
                JOIN UsersInvoicesData.main.invoices
                    ON UsersInvoicesData.main.invoice_items.InvoiceId == UsersInvoicesData.main.invoices.InvoiceId
                JOIN UsersInvoicesData.main.customers
                    ON UsersInvoicesData.main.invoices.CustomerId == UsersInvoicesData.main.customers.CustomerId
                JOIN TracksData.main.tracks
                    ON UsersInvoicesData.main.invoice_items.TrackId == TracksData.main.tracks.TrackId


