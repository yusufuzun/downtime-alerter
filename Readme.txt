
Setup

1- Set ConnectionStrings.DefaultConnection with an connection string
2- Apply EF Core Migration:
Add-Migration initial
Update-Database
3- Modify SmtpSettings for email service
------
Some Info About Dependencies

- Database Management -> EF Core
- Logging -> Serilog MsSql
- Alerting Jobs -> Hangfire Sql
- Live Notification -> SignalR
- InApp Messaging -> MediatR
- User Management -> Aspnet Identity
- Unit Testing -> xUnit