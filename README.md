# SAP-AdresToLatLong

SAP B1 utility to transform SAP addresses into Latitude and Longitude coordinates using the Google Geocoding API.

## Features
- Fetches addresses from SAP Business One or a direct SQL database.
- Converts addresses to coordinates via Google Geocoding API.
- Supports two run modes: **SQL** (Direct Database) and **SAP** (DI API).
- Dockerized for easy deployment.

## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/products/docker-desktop/) 
- Google Geocoding API Key
- Access to SAP Business One database (via SQL server, SAP DI API or SAP HANA)
- **SAP Business One DI API (SAPbobsCOM.dll)**: If using `SAP` mode, you must generate a COM Interop assembly for the SAP DI API.

### SAP DI API Interop Note
When using the SAP DI API (`RUN_MODE=SAP`), the application relies on the `SAPbobsCOM` library. You need to ensure the Interop assembly is correctly referenced or generated,
The SAP DI API requires a COM Interop assembly to be available in the build path. Please do ensure to denote the version number of the DI API in the interop name. 
If your SAPBobsCOM dll is named `SAPBobsCOM100.dll`, you should name the Interop assembly as `Interop.SAPbobsCOM100.dll`.


To set this up:
1. Locate the `SAPbobsCOM.dll` in your SAP Business One installation folder.
2. Add it as a COM reference in your project or generate the Interop assembly using `tlbimp.exe`.
3. Ensure the `Interop.SAPbobsCOM.dll` is available in your build path.

## Getting Started

### 1. Configuration
The application requires environment variables to function. **You must create a `.env` file** in the root directory by copying the provided template:

```bash
cp .env.example .env
```

Edit the `.env` file and fill in your credentials and API keys. **Note:** Ensure that `.env` is ignored by version control to protect your secrets.

### 2. Environment Variables

| Variable | Description |
| :--- | :--- |
| `GOOGLE_GEOCODING_API_KEY` | Your Google Maps Geocoding API Key. |
| `RUN_MODE` | `SQL` to use direct database queries or `SAP` to use SAP DI API. |
| `SAP_DATABASE_URL` | URL/Server address for SAP database. |
| `SAP_DATABASE_USERNAME` | Username for SAP database. |
| `SAP_DATABASE_PASSWORD` | Password for SAP database. |
| `SAP_DI_API_SQL_VERSION` | SQL Version for DI API (e.g., `2016`, `2017`, `2019`, `2022`). |
| `SQL_DATABASE_URL` | Connection string or URL for direct SQL access. |
| `SQL_DATABASE_USERNAME` | Username for direct SQL access. |
| `SQL_DATABASE_PASSWORD` | Password for direct SQL access. |

### 3. Running the Application

#### Using Docker (Recommended)
Build and run the container using Docker Compose:
```bash
docker-compose up --build
```

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
