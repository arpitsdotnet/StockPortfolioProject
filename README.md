# 📈 Stock Portfolio Management System (.NET Core)

## 📌 Project Overview
This project is a **Stock Portfolio Management System** built on **.NET Core**. It focuses on managing stock symbols, tracking daily stock prices, and generating dashboards and reports. The application integrates with the **Alpha Vantage API** to fetch real-time and historical stock market data.

This project is created for **learning and portfolio purposes** and is not associated with any company’s internal system.

---

## 🎯 Key Features

### 1️⃣ Symbol Management
- Add, update, and delete stock symbols (e.g., AAPL, MSFT, RELIANCE)
- Maintain exchange and company details
- Enable or disable symbols

### 2️⃣ Daily Stock Price Management
- Fetch daily stock prices using Alpha Vantage API and Save into DB
- Store Open, High, Low, Close, and Volume data
- Support for scheduled or automated daily data sync

### 3️⃣ Dashboard
- Market overview with key metrics
- Price trends for selected stocks
- Day-wise gain and loss summary
- Visual representation using charts and cards

### 4️⃣ Reports
- Daily stock price reports
- Historical reports based on date range
- Symbol-wise performance reports
- Export options (CSV / Excel – future scope)

---

## 🛠 Technology Stack

- **Backend:** ASP.NET Core (.NET 6 / .NET 7)
- **Frontend:** Razor Pages / MVC
- **UI Framework:** Bootstrap, jQuery
- **Database:** SQL Server
- **ORM / Data Access:** Entity Framework Core / Dapper
- **External API:** Alpha Vantage API
- **Logging:** ILogger
- **Architecture:** Layered Architecture (Controller → Service → Repository)

---

## 🔌 External API Integration

### Alpha Vantage API
- Official Website: https://www.alphavantage.co
- Provides free and paid plans
- APIs used:
  - Daily Time Series
  - Symbol Search

> ⚠️ Note: Free plan has API rate limits which are handled in the application logic.

---

## ⚙️ Configuration

### appsettings.json
```json
{
  "AlphaVantage": {
    "ApiKey": "YOUR_API_KEY",
    "BaseUrl": "https://www.alphavantage.co/query"
  },
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_DATABASE_CONNECTION_STRING"
  }
}
```

---

## 🧱 High-Level Project Structure

```
StockPortfolio
│
├── Controllers / Pages
├── Services
│   └── AlphaVantageService.cs
├── Repositories
├── Models
├── Data
├── Views
├── wwwroot
└── appsettings.json
```

---

## 🚀 Future Enhancements
- User authentication and role management
- Portfolio buy/sell tracking
- Real-time stock price updates
- Advanced charts (Candlestick charts, indicators)
- Export reports to PDF and Excel

---

## 👨‍💻 Author
**Arpit**  
Senior .NET Backend Developer

---

## 📄 License
This project is developed for **educational and portfolio use only**.

