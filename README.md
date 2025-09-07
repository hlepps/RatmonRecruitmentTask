# Ratmon Recruitment task

System that allocates data from various devices, stores them and displays in secured web application.
Uses C#, Blazor, Radzen, PostgreSQL and RabbitMQ


## Features

- Simulation of measurement devices
- Guaranteed reliable transmission of data packets using RabbitMQ
- Secure https connection
- Authorized access via login and password
- Web-based device configuration management
- Clear visualization of received data

## How to run

1. Open .sln Solution file in Visual Studio
2. Switch Startup Item to "*docker-compose"*
3. In order to enable  https, generate and trust certificate using:
```dotnet dev-certs https -ep ./dev_https/devcert.pfx -p zaq1@WSX --trust```
4. Run *Docker Compose*
5. Visit */Account/supersecretregister* to create account
