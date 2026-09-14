# 🏥 DocFind - Backend API

![.NET](https://img.shields.io/badge/dotnet-%5D.NET%209-blue.svg)
![EF Core](https://img.shields.io/badge/Entity_Framework-Core-green.svg)
![SQLite](https://img.shields.io/badge/Database-SQLite-orange.svg)

DocFind is the high-performance backend engine for a healthcare booking 
platform. Built with **.NET 9**, it manages doctor profiles, patient 
records, and appointment scheduling using an optimized SQLite database.

## 🛠 Tech Stack
*   **Framework:** .NET 9 (WebAPI)
*   **ORM:** Entity Framework Core
*   **Database:** SQLite (Configuration handled internally via `Program.cs`)
*   **Auth:** JWT Bearer Token Authentication
*   **Testing:** xUnit/NUnit (Integrated via Docker containers)

## 🚀 Getting Started

### Prerequisites
* [.NET 9 SDK](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/) (Required for running the test suite)
* [SQLite](https://www.sqlite.org/) (Standard driver included in EF Core)

### Installation & Setup

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/yourusername/docfind-backend.git
    cd docfind-backend
    ```

2.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```

3.  **Configure JWT Secrets:**
    The application requires a secret key for JWT authentication. 
    Instead of manual editing, use the .NET secrets tool:

```bash
    # This will store the secret locally in your user profile's secret folder
    dotnet user-secrets set "JwtSettings:Secret" "Your_Super_Long_Secure_Key_Here"
```

4.  **Update Database:**
    Since the connection string is managed within `Program.cs`, simply run:
    
```bash
    dotnet ef database update
```

5.  **Run the API:**
    ```bash
    dotnet run --configuration Development
    ```
    
    The scalar API will be available at: 
	`https://localhost:7173/scalar`

## 🧪 Testing
I use a Docker-based approach to ensure tests run in an isolated 
environment with consistent parameters. To run the test suite, use the 
provided bash script:

```bash
# This script handles the Docker container lifecycle and passes the 
# correct environment variables
chmod +x ./scripts/run_tests.sh  # (Adjust path if your script is elsewhere)
./scripts/run_test.sh
```

## 📂 Project Structure
The solution is organized into functional modules for clarity:

*   **`endpoints/`**: Contains the API controller definitions and route handling.
*   **`migrations/`**: EF Core database migration files.
*   **`models/`**: Data models, DTOs, and entity definitions.
*   **`properties/`**: Configuration properties and internal helper objects.
*   **`Program.cs`**: Main entry point, DI container setup, and inline DB configuration.
*   **`tests/`**: The project containing the test suite logic.

## 🛣 API Overview (Examples)
| Method | Endpoint                           | Description                         |
| :----- | :--------------------------------- | :---------------------------------- |
| `GET`  | `/api/get/doctors`                 | Retrieve a list of all doctors      |
| `POST` | `/api/create/appointment`          | Schedule a new booking              |
| `GET`  | `/api/schedule/get/{id}/workhours` | Get work hours of a specific doctor |

## 🤝 Contributing
1. Fork the project.
2. Create your feature branch (`git checkout -b feature/new-feature`).
3. Commit your changes.
4. Push to the branch and open a Pull Request.

## 📄 License
Distributed under the MIT License.

---
*Built with ❤️ for DocFind.*