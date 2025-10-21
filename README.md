# LaAlliance

## Food Ordering System

LaAlliance is a comprehensive food ordering system built with C# ASP.NET MVC. It enables restaurants to manage their menus and orders efficiently, while providing customers with a seamless experience for browsing, ordering, and tracking food deliveries.

---

### Features

- **Menu Browsing:** Customers can view detailed menus, including images and descriptions.
- **Order Placement:** Simple and secure process for placing food orders.
- **Order Tracking:** Real-time updates on order status from preparation to delivery.
- **User Authentication:** Secure login and registration for both customers and administrators.
- **Order History:** Customers can view their previous orders and reorder easily.
- **Admin Dashboard:** Restaurant staff can manage menus, view orders, and update order statuses.
- **Image & File Storage:** Menu images and other files are securely stored using Azure Blob Storage.

---

### Technology Stack

- **Backend:** C# with ASP.NET MVC framework
- **Frontend:** Razor Views for dynamic server-side rendering
- **Database:** MySQL for storing user, menu, and order data
- **File Storage:** Azure Blob Storage for images and documents

---

### Getting Started

#### Prerequisites

- Visual Studio 2019 or later
- .NET Framework (as specified in the project)
- MySQL Server
- Azure account for Blob Storage

#### Setup Instructions

1. **Clone the repository:**
   ```
   git clone https://github.com/yourusername/LaAlliance.git
   ```

2. **Open the solution in Visual Studio.**

3. **Configure the database connection:**
   - Update the MySQL connection string in `appsettings.json` or `Web.config`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "server=YOUR_SERVER;database=YOUR_DB;user=YOUR_USER;password=YOUR_PASSWORD;"
     }
     ```

4. **Configure Azure Blob Storage:**
   - Add your Azure Blob Storage credentials to `appsettings.json`:
     ```json
     "AzureBlobStorage": {
       "ConnectionString": "YOUR_AZURE_BLOB_CONNECTION_STRING",
       "ContainerName": "YOUR_CONTAINER_NAME"
     }
     ```

5. **Restore NuGet packages** and build the solution.

6. **Apply database migrations**.

7. **Run the application** using IIS Express or your preferred method in Visual Studio.

---

### Contributing

We welcome contributions! To contribute:

1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Commit your changes and push to your fork.
4. Submit a pull request describing your changes.

---

### License

License under MIT License

---
