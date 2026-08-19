# Dijital Kredi Yönetim Sistemi


- **.NET 8 SDK** ([İndir](https://dotnet.microsoft.com/download))
- **Node.js 18+** ([İndir](https://nodejs.org))
- **MySQL Server 8+** ([İndir](https://dev.mysql.com/downloads/mysql/))
- **Visual Studio 2022** veya **VS Code**

### 1. Backend Kurulumu

```bash
git clone https://github.com/AbdulsametEkinci/DigitalLoanSystem.git
cd DigitalLoanSystem

cd Infrastructure
dotnet ef database update

cd ../API
dotnet run
```

### 2. Frontend Kurulumu

```bash
cd digital-loan-frontend

npm install

npm run dev
```


---
