Welcome the solution for my Knab assessment

Here I will give some more detail regarding the solution.

General:
-The solution is a React with .netCoreWebApi template. So when starting the solution both the api as well as the UI will start up.

API:
Auth endpoint
-This endpoint simulates a login request in order to validate the credentials as well as to retrieve a valid jwt token which will be used for crypto data requests

Cryptodata endpoints
-These endpoints are secured with jwt validation
-Use admin as both username and password for validation  (lowercase)

getCryptoProperties
-This retrieves a list of all the available cryptocurrencies from the database
-It is mostlyused in the UI but it can be used to view all the available properties in the database

cryptoCurrency
-This endpoint retrieves the exhange rate values for the provided currency


UI:
-The UI simulates the same flow as the swagger UI.
-It is not the prettiest UI, and not my best work, but it simply serves as a small tool to view the data retrieval flow
-For the login simulation use admin as both username as password (lowercase)


