
# Gatherly - Online Meeting Scheduler

## Project description

This project is an online meeting scheduling application called **Gatherly**, developed using C#, React and Next.js. The platform allows users to schedule and manage meetings online with features such as calendar integration, real-time chat, guest invitations, and a time-coordination module. The backend is built with a combination of **ASP .NET Core** for the application logic and **NHibernate** for object-relational mapping (ORM), while **JWT** and **Refresh Tokens** are used for secure authentication. 
The frontend is hosted on Vercel, while the backend and database are hosted on Azure.

## Access the live version here
https://gatherly-mocha.vercel.app/

## Technologies Used

- **Backend:** C# (ASP.NET Core)
- **Frontend:** React.js, Next.js
- **Database:** MSSQL (with T-SQL and NHibernate)
- **Real-time Communication:** SignalR
- **Authorization:** JWT, Refresh Tokens, SSO (Single Sign-On)
- **Hosting:** Frontend on Vercel, Backend and Database on Azure

## Functionalities

### Integrations
- **SignalR Real-time Chat Integration:** Integrated SignalR for real-time messaging during meetings.
- **JWT & Refresh Token Authentication:** Custom implementation of JWT tokens with refresh tokens for user authentication.
- **SSO (Single Sign-On) Integration:** Enabled single sign-on via email messages.
  
### Raw functionalities
- **Meeting Scheduling with Calendar:** Users can create and schedule meetings with integrated calendar views.
- **Guest Invitations:** Users can invite guests to meetings via email or direct links.
- **Time Coordination Module:** A feature that allows users to collaboratively decide on the best meeting time.
- **Password Recovery:** Implemented secure password reset functionality.
- **Real-time Notifications:** Notifications for upcoming meetings and guest actions.

## SOLID principles

This application was developed with an emphasis on the **SOLID** principles, ensuring a maintainable and extendable codebase:

- **Single Responsibility Principle (SRP):** Every class has a focused responsibility, keeping the codebase organized and easier to maintain.
- **Open/Closed Principle (OCP):** The system is designed to be extendable without modifying existing functionality, allowing for future features to be added without impacting the core logic.
- **Liskov Substitution Principle (LSP):** Interfaces and abstract classes are implemented to ensure that derived classes are used without altering their intended behavior.
- **Interface Segregation Principle (ISP):** Fine-grained interfaces are created to ensure classes are only aware of the methods they need to implement.
- **Dependency Inversion Principle (DIP):** High-level modules rely on abstractions, and dependency injection is used throughout the project.

## Architecture

The backend follows the **MVC** pattern, with the **Persistence** layer substituting the View component to handle data access. Data migrations are managed through **NHibernate**, with MSSQL as the underlying database.

## Improvements

To further improve the application, future developments will focus on:

- **Enhanced Testing Coverage:** Expanding unit and integration tests to improve code reliability.
- **Scalability Enhancements:** Optimizing the application for larger user bases with high concurrency requirements.

