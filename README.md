# Library Management System

A web-based library management system built with ASP.NET Core MVC that allows administrators to manage books, authors, categories, members, and borrowings. Users can view their borrowed books and extend due dates.

## Features

### Admin Features
- **Book Management**: Create, read, update, and delete books
- **Author Management**: Manage author information
- **Category Management**: Organize books into categories
- **Member Management**: Manage library members
- **Borrowing Management**: Create, return, and delete borrowings
- **User Management**: Add and manage system users (Admin/User roles)
- **Reports**: View library statistics including most borrowed books, popular authors, overdue borrowings, and books by category

### User Features
- **Authentication**: Login and registration
- **My Account**: View personal information and borrowed books
- **Extend Due Date**: Extend borrowing period by 14 days

## Technologies Used

- **ASP.NET Core MVC** - Web framework
- **Entity Framework Core** - ORM for database operations
- **SQLite** - Database
- **Bootstrap 5** - UI framework
- **Bcrypt** - Password hashing

## Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd lab10
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

The application will automatically create and seed the SQLite database on first run.

## Default Credentials

### Admin Account
- **Email**: admin@mail.com
- **Password**: admin123

### Test User Accounts
- **Email**: john.smith@email.com
- **Password**: password123

- **Email**: jane.doe@email.com
- **Password**: password123

- **Email**: bob.johnson@email.com
- **Password**: password123

## Project Structure

```
lab10/
├── Controllers/          # MVC controllers
│   ├── AccountController.cs      # Authentication and user management
│   ├── AuthorsController.cs      # Author CRUD operations
│   ├── BooksController.cs        # Book CRUD operations
│   ├── BorrowingsController.cs   # Borrowing management
│   ├── CategoriesController.cs   # Category CRUD operations
│   ├── HomeController.cs         # Home and reports
│   └── MembersController.cs      # Member CRUD operations
├── Models/               # Data models
│   ├── Author.cs
│   ├── Book.cs
│   ├── Borrowing.cs
│   ├── Category.cs
│   ├── Login.cs
│   ├── Member.cs
│   └── ViewModels/
├── Views/                # Razor views
│   ├── Account/
│   ├── Authors/
│   ├── Books/
│   ├── Borrowings/
│   ├── Categories/
│   ├── Home/
│   ├── Members/
│   └── Shared/
├── Services/             # Business logic services
│   └── DbSeeder.cs       # Database seeding
├── Configuration/        # Configuration classes
├── Middleware/           # Custom middleware
├── Program.cs            # Application entry point
└── AppDbContext.cs       # Database context
```

## Database Seeding

The application automatically seeds the database with:
- 1 admin user
- 3 regular users
- 5 authors
- 5 categories
- 10 books
- 3 members
- 2 sample borrowings

To reset the database, delete the `database.db` file and restart the application.

## Borrowing Rules

- Default loan period: 28 days
- Users can extend due date by 14 days
- Admins can manage all borrowings
- Users can only view and extend their own borrowings

## Development

The project uses session-based authentication. Session timeout is set to 30 minutes.

## License

This project is for educational purposes.
