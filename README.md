# Car rental app

## Description

This WinUI app simulates a car rental experience, allowing users to register, log in, and select dates for car reservations. Users can also view and manage their reservations within the app.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Screenshots or Demo](#screenshots-or-demo)
- [Security](#security)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Database Configuration](#database-configuration)
- [Importing the Database](#importing-the-database)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)
## Features

- User registration and login.
- Date selection for car reservations.
- Reservation management (view, change dates, cancel).
- Database management (as an administrator).
- MySQL database integration.

## Technologies Used

- WinUI
- Windows App SDK
- MySQL
- C#
- Visual Studio

## Screenshots or Demo


## Security

- User passwords are securely hashed before being stored in the database to ensure data confidentiality and user account safety.

## Getting Started

To get started with this project, follow these steps:

1. Clone the repository:

```bash
git clone https://github.com/konstantinos-prasinos/car-rental-app.git
```

2. Install any necessary dependencies.

3. Set up the MySQL database and configure the app to connect to it.

4. Build and run the app.

## Usage

1. Register and log in to your account.
2. Select dates for car reservations.
3. View and manage your reservations.

## Database Configuration

You have the flexibility to host the MySQL database locally on `localhost` and port `3306`, or modify the database connection settings in the code to match your specific configuration. To do this:

1. If you want to host the database locally:
   - Ensure you have MySQL installed and running on your machine.
   - Use the default `localhost` and `3306` settings in the code.

2. If you have a remote database or different database configuration:
   - Open the app's source code in Visual Studio.
   - Locate the database connection settings (usually found in a configuration file or in the code where the database connection is established).
   - Modify the database connection string to match your specific database server, username, password, and port.

Remember to keep your database connection credentials secure and do not share them in public repositories.

## Importing the Database

If you want to recreate the database using the provided SQL dump file, follow these steps:

1. Ensure you have MySQL installed and running on your machine.

2. Open a command prompt or terminal.

3. Navigate to the root directory of the project.

4. Run the following command to import the SQL dump file into your MySQL database:

   ```bash
   mysql -u [username] -p [database_name] < database_backup/database_dump.sql
   ```

## Contributing

Contributions are welcome!

## License

This project is licensed under the [MIT License](LICENSE.txt).

## Contact

If you have any questions or need assistance, you can reach me at [konstantinos.prasinos@gmail.com](mailto:konstantinos.prasinos@gmail.com).
