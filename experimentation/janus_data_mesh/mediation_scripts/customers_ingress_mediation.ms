SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE CustomersIngressData VERSION "1.0"
    WITH SCHEMA Main
        WITH TABLEAU Customers
            WITH ATTRIBUTES
                CustomerId,
                UserName,
                Email,
                CountryName
            BEING
                SELECT
                    CustomersData.main.customers.customer_id,
                    CustomersData.main.customers.username,
                    CustomersData.main.customers.email,
                    CountriesData.main.countries.name
                FROM CustomersData.main.customers
                JOIN CountriesData.main.countries
                    ON CustomersData.main.customers.country_id = CountriesData.main.countries.country_id