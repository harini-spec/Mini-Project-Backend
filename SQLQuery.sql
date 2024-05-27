use dbBusBookingAppln 
select * from Drivers 
select * from DriversDetails
select * from Users 
delete from Users where Id = 5
select * from Buses 
select * from Seats 
select * from Routes 
delete from Routes where Id in (2,3,4)
select * from RouteDetails 
delete from RouteDetails where RouteId in (2, 3, 4)
select * from Schedules
delete from Schedules where Id in (7)
select * from tickets 
delete from tickets where Id = 4
delete from TicketDetails where TicketId = 1
select * from TicketDetails