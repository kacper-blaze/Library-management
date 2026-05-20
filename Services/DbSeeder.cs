using lab10.Models;
using Microsoft.EntityFrameworkCore;

namespace lab10.Services;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        // Ensure database is created
        await dbContext.Database.EnsureCreatedAsync();

        // Check if admin user exists
        var adminUser = await dbContext.Loginy.FirstOrDefaultAsync(l => l.Username == "admin@mail.com");
        if (adminUser == null)
        {
            var hashedPassword = BcryptHelper.ComputeBCryptHash("admin123");
            var apiToken = GenerateApiToken();

            adminUser = new Login
            {
                Username = "admin@mail.com",
                Password = hashedPassword,
                Role = "Admin",
                ApiToken = apiToken
            };
            dbContext.Loginy.Add(adminUser);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Admin user created: admin@mail.com / admin123");
        }

        // Seed categories if empty
        if (!await dbContext.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Fiction", Description = "Fictional literature" },
                new Category { Name = "Non-Fiction", Description = "Non-fictional works" },
                new Category { Name = "Science", Description = "Scientific publications" },
                new Category { Name = "History", Description = "Historical books" },
                new Category { Name = "Biography", Description = "Biographical works" }
            };
            dbContext.Categories.AddRange(categories);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Categories seeded");
        }

        // Seed authors if empty
        if (!await dbContext.Authors.AnyAsync())
        {
            var authors = new List<Author>
            {
                new Author { FirstName = "J.K.", LastName = "Rowling", Biography = "British author" },
                new Author { FirstName = "George", LastName = "Orwell", Biography = "English novelist" },
                new Author { FirstName = "Jane", LastName = "Austen", Biography = "English novelist" },
                new Author { FirstName = "Stephen", LastName = "King", Biography = "American author" },
                new Author { FirstName = "Agatha", LastName = "Christie", Biography = "English writer" }
            };
            dbContext.Authors.AddRange(authors);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Authors seeded");
        }

        // Seed books if empty
        if (!await dbContext.Books.AnyAsync())
        {
            var authors = await dbContext.Authors.ToListAsync();
            var categories = await dbContext.Categories.ToListAsync();
            
            var books = new List<Book>
            {
                new Book
                {
                    Title = "Harry Potter and the Philosopher's Stone",
                    ISBN = "978-0747532743",
                    PublicationYear = 1997,
                    Publisher = "Bloomsbury",
                    TotalCopies = 5,
                    AvailableCopies = 5,
                    AuthorId = authors[0].Id,
                    CategoryId = categories[0].Id
                },
                new Book
                {
                    Title = "1984",
                    ISBN = "978-0451524935",
                    PublicationYear = 1949,
                    Publisher = "Signet Classic",
                    TotalCopies = 3,
                    AvailableCopies = 3,
                    AuthorId = authors[1].Id,
                    CategoryId = categories[1].Id
                },
                new Book
                {
                    Title = "Pride and Prejudice",
                    ISBN = "978-0141439518",
                    PublicationYear = 1813,
                    Publisher = "Penguin Classics",
                    TotalCopies = 4,
                    AvailableCopies = 4,
                    AuthorId = authors[2].Id,
                    CategoryId = categories[0].Id
                },
                new Book
                {
                    Title = "The Shining",
                    ISBN = "978-0307743657",
                    PublicationYear = 1977,
                    Publisher = "Doubleday",
                    TotalCopies = 2,
                    AvailableCopies = 2,
                    AuthorId = authors[3].Id,
                    CategoryId = categories[0].Id
                },
                new Book
                {
                    Title = "Murder on the Orient Express",
                    ISBN = "978-0062073501",
                    PublicationYear = 1934,
                    Publisher = "HarperCollins",
                    TotalCopies = 3,
                    AvailableCopies = 3,
                    AuthorId = authors[4].Id,
                    CategoryId = categories[0].Id
                },
                new Book
                {
                    Title = "A Brief History of Time",
                    ISBN = "978-0553380163",
                    PublicationYear = 1988,
                    Publisher = "Bantam",
                    TotalCopies = 4,
                    AvailableCopies = 4,
                    AuthorId = authors[0].Id,
                    CategoryId = categories[2].Id
                },
                new Book
                {
                    Title = "The Diary of a Young Girl",
                    ISBN = "978-0141035139",
                    PublicationYear = 1947,
                    Publisher = "Penguin",
                    TotalCopies = 3,
                    AvailableCopies = 3,
                    AuthorId = authors[2].Id,
                    CategoryId = categories[3].Id
                },
                new Book
                {
                    Title = "Steve Jobs",
                    ISBN = "978-1451648539",
                    PublicationYear = 2011,
                    Publisher = "Simon & Schuster",
                    TotalCopies = 2,
                    AvailableCopies = 2,
                    AuthorId = authors[3].Id,
                    CategoryId = categories[4].Id
                },
                new Book
                {
                    Title = "Sapiens: A Brief History of Humankind",
                    ISBN = "978-0062316097",
                    PublicationYear = 2011,
                    Publisher = "Harper",
                    TotalCopies = 3,
                    AvailableCopies = 3,
                    AuthorId = authors[1].Id,
                    CategoryId = categories[1].Id
                },
                new Book
                {
                    Title = "The Catcher in the Rye",
                    ISBN = "978-0316769488",
                    PublicationYear = 1951,
                    Publisher = "Little, Brown and Company",
                    TotalCopies = 4,
                    AvailableCopies = 4,
                    AuthorId = authors[3].Id,
                    CategoryId = categories[0].Id
                }
            };
            dbContext.Books.AddRange(books);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Books seeded");
        }

        // Seed members if empty
        if (!await dbContext.Members.AnyAsync())
        {
            var members = new List<Member>
            {
                new Member
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@email.com",
                    PhoneNumber = "1234567890",
                    Address = "123 Main St, City",
                    MembershipDate = DateTime.Now.AddDays(-30)
                },
                new Member
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane.doe@email.com",
                    PhoneNumber = "0987654321",
                    Address = "456 Oak Ave, Town",
                    MembershipDate = DateTime.Now.AddDays(-15)
                },
                new Member
                {
                    FirstName = "Bob",
                    LastName = "Johnson",
                    Email = "bob.johnson@email.com",
                    PhoneNumber = "5551234567",
                    Address = "789 Pine Rd, Village",
                    MembershipDate = DateTime.Now.AddDays(-60)
                }
            };
            dbContext.Members.AddRange(members);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Members seeded");

            // Create login accounts for members
            var loginUsers = new List<Login>
            {
                new Login
                {
                    Username = "john.smith@email.com",
                    Password = BcryptHelper.ComputeBCryptHash("password123"),
                    Role = "User",
                    ApiToken = GenerateApiToken()
                },
                new Login
                {
                    Username = "jane.doe@email.com",
                    Password = BcryptHelper.ComputeBCryptHash("password123"),
                    Role = "User",
                    ApiToken = GenerateApiToken()
                },
                new Login
                {
                    Username = "bob.johnson@email.com",
                    Password = BcryptHelper.ComputeBCryptHash("password123"),
                    Role = "User",
                    ApiToken = GenerateApiToken()
                }
            };
            dbContext.Loginy.AddRange(loginUsers);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Login accounts for members created");
        }

        // Seed some borrowings if empty
        if (!await dbContext.Borrowings.AnyAsync())
        {
            var books = await dbContext.Books.ToListAsync();
            var members = await dbContext.Members.ToListAsync();
            
            var borrowings = new List<Borrowing>
            {
                new Borrowing 
                { 
                    BookId = books[0].Id,
                    MemberId = members[0].Id,
                    BorrowDate = DateTime.Now.AddDays(-10),
                    DueDate = DateTime.Now.AddDays(20),
                    ReturnDate = null
                },
                new Borrowing 
                { 
                    BookId = books[1].Id,
                    MemberId = members[1].Id,
                    BorrowDate = DateTime.Now.AddDays(-5),
                    DueDate = DateTime.Now.AddDays(25),
                    ReturnDate = null
                }
            };
            
            // Update available copies
            foreach (var borrowing in borrowings)
            {
                var book = await dbContext.Books.FindAsync(borrowing.BookId);
                if (book != null)
                {
                    book.AvailableCopies--;
                }
            }
            
            dbContext.Borrowings.AddRange(borrowings);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Borrowings seeded");
        }
    }

    private static string GenerateApiToken()
    {
        return Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
    }
}
