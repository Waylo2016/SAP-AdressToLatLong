# SAP-AdresToLatLong

SAP B1 utility to transform SAP addresses into Latitude and Longitude coordinates using the Google Geocoding API. 
This program utilizes the SAP Service Layer, and the V2 architecture of the SAP B1 API.

## Features
- Fetches addresses from SAP Business One and processes them.
- Converts addresses to coordinates via Google Geocoding API.
- Uses PostgreSQL for data storage.
- Dockerized for easy deployment with automated secret management.

## Prerequisites
- [Docker](https://www.docker.com/products/docker-desktop/) 
- [OpenSSL](https://www.openssl.org/) (required by `deploy.sh` for secret generation)
- Google Geocoding API Key
- And quite obviously, a SAP Business One server that has a working Service Layer

## Getting Started

### 1. Deployment
The application uses a deployment script (`deploy.sh`) to handle secret generation and service orchestration. **New users must run this script after pulling the repository/image to initialize the environment.**

Run the deployment script:
```bash
chmod +x deploy.sh
./deploy.sh
```

### 2. How `deploy.sh` Works
The `deploy.sh` script is designed to automate the setup of sensitive information and ensure the application starts correctly.

- **Secret Initialization**: It creates a `secrets/` directory and populates it with required files if they don't exist.
- **Automated Generation**: It generates secure, random credentials for the internal PostgreSQL database (`postgres_user.txt`, `postgres_password.txt`).
- **Placeholder Creation**: For external services (SAP and Google), it creates files with `CHANGE_ME`. You must replace these values with your actual credentials.
- **Validation**: Before running `docker compose`, it verifies that no `CHANGE_ME` placeholders remain.
- **Service Launch**: Finally, it triggers `docker compose up -d --build`.

#### When to call it:
- **First time**: Mandatory to generate secrets and directory structure.
- **After Updates**: Recommended after pulling new code to ensure any new secret requirements are met.
- **Restarts**: Safe to call anytime; it won't overwrite existing random Postgres secrets, but it will check if you've filled in your API keys.

### 3. Secret Management
This project uses **Docker Secrets** for production-grade security. 

| Secret File             | Source        | Description                                      |
|:------------------------|:--------------|:-------------------------------------------------|
| `postgres_user.txt`     | Generated     | Randomly generated username for the internal DB. |
| `postgres_password.txt` | Generated     | Randomly generated password for the internal DB. |
| `sap_username.txt`      | User Provided | Your SAP Business One Service Layer username.    |
| `sap_password.txt`      | User Provided | Your SAP Business One Service Layer password.    |
| `geocoding_api.txt`     | User Provided | Your Google Geocoding API Key.                   |

> **Note:** The `secrets/` folder is ignored by Git. Never commit these files if you are helping development.

### 4. Configuration
The application is primarily configured via environment variables. For convenience, a `.env.example` file is provided, pre-loaded with default values for a standard setup (e.g., database names, ports, and internal file paths).

| Variable                 | Description                                                                                                                                                                  |
|:-------------------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `SAP_REST_CLIENT`        | **Required.** The SAP Service Layer base URL (e.g., `sap-server:50000`). As you can see, there's no need to append a http(s) to the URL, the program does this automatically |
| `SAP_SERVER`             | **Required.** The database used for SAP, known as the CompanyDB in SAP                                                                                                       |
| `POSTGRES_HOST`          | Hostname of the PostgreSQL service (default: `postgres` for Docker, `localhost` for local dev).                                                                              |
| `POSTGRES_PORT`          | Port of the PostgreSQL service (default: `5432`).                                                                                                                            |
| `POSTGRES_DB`            | Name of the database (default: `geocoder_db`).                                                                                                                               |
| `SAP_REST_USERNAME`      | Path to the secret file containing the SAP username.                                                                                                                         |
| `SAP_REST_PASSWORD`      | Path to the secret file containing the SAP password.                                                                                                                         |
| `GEOCODING_API`          | **Required.** API Key for the Google Maps Geocoding API                                                                                                                      |
| `POSTGRES_USER_FILE`     | Path to the secret file containing the DB username.                                                                                                                          |
| `POSTGRES_PASSWORD_FILE` | Path to the secret file containing the DB password.                                                                                                                          |

To use these, copy `.env.example` to `.env` and adjust as needed. Note that `deploy.sh` and `compose.yaml` prioritize Docker Secrets for sensitive credentials.


## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
