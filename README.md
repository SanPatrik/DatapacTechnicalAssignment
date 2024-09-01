# Datapac Technical Assignment

## Overview
This project is a simple library management system built with ASP\.NET Core\. It includes functionalities for managing books and loans, with automatic due date settings and reminder triggers\.

## Features
\- **Book Management**: CRUD operations for books\.
\- **Loan Management**: Borrow and return books\.
\- **Automatic Due Date**: The return date for loans is automatically set to the next day to trigger a cron job for reminders\.
\- **In\-Memory Database**: Uses Entity Framework Core with an in\-memory database for easy testing and development\.
\- **Unit Tests**: Includes simple unit tests that can be extended\.
\- **Cron Jobs**: Uses Quartz library for scheduling tasks\.

## Technologies Used
\- **ASP\.NET Core**: Web framework for building the application\.
\- **Entity Framework Core**: ORM for database operations\.
\- **Quartz\.NET**: Library for scheduling cron jobs\.
\- **XUnit**: Testing framework for unit tests\.

## Getting Started
### Prerequisites
\- \.NET SDK
\- IDE \(e\.g\., JetBrains Rider\)

### Installation
1\. Clone the repository:
\```sh
git clone https://github.com/SanPatrik/DatapacTechnicalAssignment.git
\```
2\. Navigate to the project directory:
\```sh
cd DatapacTechnicalAssignment
\```

### Running the Application
1\. Open the project in your IDE\.
2\. Run the application\.

### Running Tests
1\. Open the project in your IDE\.
2\. Run the application\.
3\. Run the unit tests\.

## Project Structure
\- `Controllers`: Contains the API controllers for books and loans\.
\- `Models`: Contains the data models for the application\.
\- `Dtos`: Contains the Data Transfer Objects\.
\- `UnitTests`: Contains the unit tests for the application\.
\- `Jobs`: Contains the reminder job for users\.

## Unit Tests
The project includes simple unit tests for the main functionalities\. These tests can be extended to cover more scenarios\.

## Cron Jobs
The project uses Quartz\.NET for scheduling tasks\. The return date for loans is automatically set to the next day to trigger a cron job for sending reminders\.

## In\-Memory Database
The project uses an in\-memory database with Entity Framework Core for easy testing and development\. This allows you to run the application and tests without setting up a real database\.

## License
This project is licensed under the MIT License\.
