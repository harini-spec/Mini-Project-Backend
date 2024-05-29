# Bus Booking System

To develop a bus ticket booking system, allowing users to book tickets, manage bookings, and leave feedback. The system supports various entities such as users, drivers, buses, routes, schedules, tickets, payments, refunds, and feedback, with appropriate relationships and functionalities.

---

## Models
[] - Key

**User** : [User ID], Name, Age, Phone, Email, Role(Customer, Admin)

**User Details** : [User ID], Password Hash, Password encrypted, Status (for soft delete)

**Rewards** : [User ID], Reward points

**Driver** : [Driver ID], Name, Age, Email, Phone, Experience years

**Driver Details** : [Driver ID], Password Hash, Password encrypted, Status (Initially not active)

**Bus** : [Bus Number] (Number Plate), Total no.of seats

**Seats** : [Seat ID], Bus Number, Seat Number, Seat Type, Price of seat  

**Route** : [Route ID], Source, Destination 

**Route Details** : [Route ID, Stop number], From, To

**Schedule** : [Scehdule ID], Date of Departure, Date of Arrival, Bus Number, Route ID, Driver ID

**Tickets** : [Ticket ID], User ID, Schedule ID, GST, Discount, Total cost, Final amount, Status(Not Booked / Booked / Ride Over / Cancelled (all the tickets are cancelled)), Date and Time of adding 

**Ticket Details** : [Ticket ID, Seat ID], Seat price, Passenger Name, Age, Gender, Status (Not Booked / Booked / Cancelled)

**Payment** : [Payment Transaction ID], Ticket ID, Payment method, Payment date, Payment status (Success / Fail), Amt Paid

**Refund** : [Refund Transaction ID], Ticket ID, Refund Amount, Refund Date, Refund Status (Success / Fail)

**Feedback** : [Ticket ID], Feedback Date, Feedback Message, Rating



---

## Endpoints

In the bus ticket booking system, endpoints serve as the gateway for clients to interact with the application's functionalities. Each endpoint represents a specific action or operation that clients can perform, such as booking a ticket, retrieving route information, or processing payments.

## Admin:
`Post/Register Admin`
- Enter Admin details, account is created and active by default

`Post/Login Admin`
- Enter mail and password, Generates JWT Token

`Post/Register Driver` 
- Enter driver details, account inactive initially

`Put/Activate Driver`
- Activates the driver account 
    
`Post/Add Bus` 
- Fill Bus Object and it's Nav prop SeatsInBus
    
`Post/Add Route` 
- Fill Route Object and it's Nav prop RouteDetails
    
`Post/Add Schedule` 
- Fill Schedule Object with Bus Id, Route Id, Driver Id 
- Check if bus already scheduled for a diff ride 
- Check if driver already schedule for a diff ride 

`Get/AllSchedules`
- All sschedules are returned
    
`Put/UpdateRideStatus` 
- Ticket status = Ride Over for a given schedule ID
    
`Get/GetAllFeedbacksOfARide`  
- Feedbacks for a particular schedule 

## Driver
`Put/Change password` 
- If UserDetail status = "Active", get password and update 

`Get/AllSchedules`
- All schedule info of driver

# Customer 
`Post/Register Customer` 
- Fill User Object and encrypt password - populate UserDetail Object, Role = Customer 
- Status = Active 

`Post/Login Customer`
- Enter mail and password, Generates JWT Token

`Put/Deactivate Account`
- Temporarily delete account - Status = "Active"
- Don't delete if active tickets present 

`Put/Reactivate Account`
- Enter email and password
- Activate account if status = "Inactive"

`Get/AllSchedules`
- All sschedules are returned

`Get/BusesScheduledOnGivenDate` 
- Buses Scheduled on a particular day are returned 
- Input: 
    - Date of Departure, Source, Destination
        - Convert Source and Destination to Route ID 
- Output:
    - List of schedule DTO objects 
    
`Get/All Tickets of Customer`
- All tickets added, booked and cancelled by user are shown 

`Add/Add Ticket`
- Select seats in a particular schedule 
- Get Passenger details
- Calculate Total cost of seats from seats table 
- Add gst and deduct discount by calculation discount from reward pts 
- Date and Time of adding 
- These seats will be reserved for the user for 1 hr from the time of adding 

`Delete/Remove Ticket`
- Remove a ticket

`Delete/Remove Ticket Item`
- Remove a seat from the ticket
- If all seats are removed, ticket added will be removed too

`Get/Get all available Seats`
- Input:
    - Schedule Id
- CheckTicketAvailability:
    - Get all tickets
    - Check if Schedule Id in Ticket and Seat ID in Ticket Details 
    - If Ticket Status = Booked 
        - Check if Ticket Details Status of that particular seat = Booked 
        - If booked, not available, if not, available
    - If Ticket Status = Cancelled 
        - Go ahead to booking, available
    - If Ticket Status = Not Booked 
        - Check if its been 1 hr since them adding the ticket 
        - If so, release ticket 
            - Release Ticket : Delete Ticket Details first, then ticket 
            - send available
        - If not, not available 
- Output:
    - All available seats in a schedule

`Post/Add Feedback` 
- Fill Feedback Object only if Ticket Status = Ride Over 

`Post/Book Ticket`
- Input:
    - TicketId, Payment method
- Make Payment:
    - Check if its been less than 1hr of adding 
    - Calculate discount %:
        - Reward system 
        - Reduce user points if any present and if discount provided 
    - Update status = Succes 
    - Add 10 reward points to user for new every seat booked

- Reward points system:
    - 1 seat booking  = 10 Reward points 
    - 100 reward points = 10% off 
    - Can only use 10% off on every ticket total 

`Delete/Cancel Ticket` 
- Input: 
    - Ticket Id
- Logic:
    - Check if its on the day of the ride - No cancellation 
    - Refund - half of the total amount (w/o gst and discount) is refunded 
    - Check if payment done - get Payemnt Id by ticket Id and status = success
    - Deduct reward points provided to user while booking 
    - 100 pts while booking - loss
    - Ticket status = Cancelled 
    - Fill refund object 
Output:
    - Refund object DTO

`Delete/Cancel Seats`
- Input:
    - TicketID, seat ID
- Logic:
    - Check if its on the day of the ride - No refund 
    - Refund amount of seat in ticket details - half is refunded
    - Change ticket details status = cancelled 
    - Check if all ticket details status = "Cancelled" 
    - Deduct reward points from user, Ticket status = cancelled 
    - Fill refund object 
- Output:
    - Refund object DTO

