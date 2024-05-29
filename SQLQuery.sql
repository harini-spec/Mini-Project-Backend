use dbBusBookingAppln 
use master 
select * from Drivers 
select * from Users 
select * from DriversDetails
select * from Users 
select * from UserDetails
delete from Users where Id = 5
select * from Buses 
select * from Seats 
select * from Routes 
select * from Rewards
delete from Routes where Id in (2,3,4)
select * from RouteDetails 
delete from RouteDetails where RouteId in (2, 3, 4)
select * from Schedules
delete from Schedules where Id in (7)
select * from tickets 
select * from Payments
delete from tickets where Id = 4
delete from TicketDetails where TicketId = 1
select * from TicketDetails
select * from Feedbacks