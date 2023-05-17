SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE CustomersData VERSION "1.0"
    WITH SCHEMA Stable
        WITH TABLEAU Customers
            WITH ATTRIBUTES
                CustomerId,
                UserName,
                Email,
                CountryName
            BEING
                SELECT
                    CustomersDomainData.main.Main_Customers.CustomerId,
                    CustomersDomainData.main.Main_Customers.UserName,
                    CustomersDomainData.main.Main_Customers.Email,
                    CustomersDomainData.main.Main_Customers.CountryName
                FROM CustomersDomainData.main.Main_Customers
    WITH SCHEMA Fresh
        WITH TABLEAU Customers
            WITH ATTRIBUTES
                CustomerId,
                UserName,
                Email,
                CountryName
            BEING
                SELECT
                    CustomersIngressData.Main.Customers.CustomerId,
                    CustomersIngressData.Main.Customers.UserName,
                    CustomersIngressData.Main.Customers.Email,
                    CustomersIngressData.Main.Customers.CountryName
                FROM CustomersIngressData.Main.Customers
