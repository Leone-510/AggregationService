**📝 API Aggregation Service Documentation**

**🚀 1. Setup & Configuration Instructions**
**Prerequisites**
- .NET 8.0 SDK or higher
- IDE (Visual Studio 2022, VS Code, or JetBrains Rider)

**Configuration (appsettings.json)**
The application configuration manages environment urls for external dependencies. Ensure your appsettings.json contains the correct host addresses:

{
  "ExternalApis": {
    "ProductsApiUrl": "https://fakestoreapi.com",
    "WeatherApiUrl": "https://api.open-meteo.com",
    "CountriesApiUrl": "https://restcountries.com"
  }
}

**Installation Steps**
1. Clone or extract the project files to your local machine.
2. Open your terminal in the directory containing the .sln file.
3. Restore the dependencies and compile the solution: dotnet build
4. Run the API application: dotnet run --project Aggregation.WebAPI
5. Open your browser and navigate to https://localhost:7120/swagger/index.html to access the interactive Swagger UI panel.


**🛣️ 2. API Endpoints Reference**

**Endpoint A: Unified Aggregated Data**

Consolidates real-time weather details, commercial product catalogs, and countries information data concurrently in parallel using specialized in-memory caching and resilience handling.

**URL:** 
/api/aggregated

**Method:**
GET

**Query Parameters:**
- lat (double, Optional): Latitude coordinates. Defaults to 37.98 (Athens).
- lon (double, Optional): Longitude coordinates. Defaults to 23.72 (Athens).
- category (string, Optional): Filters products by classification (e.g., electronics).
- sortBy (string, Optional): Sort order criterion. Options: price, title.
- sortOrder (string, Optional): Sort direction. Options: asc (default), desc.

**📥 Sample Request String**

GET https://localhost:7120/api/aggregated

GET https://localhost:7120/api/aggregated?lat=37.98&lon=23.72&category=electronics&sortBy=price&sortOrder=desc

**📤 Sample Output Payload (200 OK) - json**

{
"countries": [
  {
    "name": {
      "common": "Anguilla",
      "official": "Anguilla",
      "nativeName": {
        "eng": {
          "official": "Anguilla",
          "common": "Anguilla"
          }
        }
      },
      "currencies": {
        "XCD": {
          "name": "Eastern Caribbean dollar",
          "symbol": "$"
          }
            },
            "capital": ["The Valley"]
        },
        {
            "name": {
                "common": "Guatemala",
                "official": "Republic of Guatemala",
                "nativeName": {
                    "spa": {
                        "official": "República de Guatemala",
                        "common": "Guatemala"
                    }
                }
            },
            "currencies": {
                "GTQ": {
                    "name": "Guatemalan quetzal",
                    "symbol": "Q"
                }
            },
            "capital": [
                "Guatemala City"
            ]
        }
  ],
  "products": [
        {
            "id": 1,
            "title": "Fjallraven - Foldsack No. 1 Backpack, Fits 15 Laptops",
            "price": 109.95,
            "description": "Your perfect pack for everyday use and walks in the forest. Stash your laptop (up to 15 inches) in the padded sleeve, your everyday",
            "category": "men's clothing",
            "image": "https://fakestoreapi.com/img/81fPKd-2AYL._AC_SL1500_t.png",
            "rating": {
                "rate": 3.9,
                "count": 120
            }
        },
        {
            "id": 2,
            "title": "Mens Casual Premium Slim Fit T-Shirts ",
            "price": 22.3,
            "description": "Slim-fitting style, contrast raglan long sleeve, three-button henley placket, light weight & soft fabric for breathable and comfortable wearing. And Solid stitched shirts with round neck made for durability and a great fit for casual fashion wear and diehard baseball fans. The Henley style round neckline includes a three-button placket.",
            "category": "men's clothing",
            "image": "https://fakestoreapi.com/img/71-3HjGNDUL._AC_SY879._SX._UX._SY._UY_t.png",
            "rating": {
                "rate": 4.1,
                "count": 259
            }
        }
  ],
  "weatherForecast": {
        "latitude": 38,
        "longitude": 29.75,
        "generationtime_ms": 0.029325485229492188,
        "utc_offset_seconds": 0,
        "timezone": "GMT",
        "timezone_abbreviation": "GMT",
        "elevation": 1136,
        "current_units": {
            "time": "iso8601",
            "interval": "seconds",
            "temperature_2m": "°C"
        },
        "current": {
            "time": "2026-05-28T00:30",
            "interval": 900,
            "temperature_2m": 14.1
        }
    },
    "systemWarnings": []
}

**Endpoint B: Performance Monitoring Statistics**

Retrieves tracking information measuring total request traffic rates and average latency responses bucketed into specific execution speeds.

**URL:** /api/statistics

**Method:** GET

**📥 Sample Request String**

GET https://localhost:7120/api/statistics

**📤 Sample Output Payload (200 OK) - json**

{
  "Weather_Api": {
    "totalRequests": 6,
    "averageResponseTimeMs": 1515.17,
    "fastRequestsCount": 4,
    "averageRequestsCount": 1,
    "slowRequestsCount": 1
  },
  "Countries_Api": {
    "totalRequests": 6,
    "averageResponseTimeMs": 174.33,
    "fastRequestsCount": 5,
    "averageRequestsCount": 0,
    "slowRequestsCount": 1
  },
  "Products_Api": {
    "totalRequests": 6,
    "averageResponseTimeMs": 60.17,
    "fastRequestsCount": 5,
    "averageRequestsCount": 0,
    "slowRequestsCount": 1
  }
}


**🛠️ 3. Built-in Engineering Features**

**Parallelism Optimization:** Parallel operations run concurrently via Task.WhenAll mapping processes, bringing the combined API network response down to the execution time of the single slowest dependency.

**In-Memory Caching:** Utilizes localized IMemoryCache intervals protecting strict resource connection constraints (AbsoluteExpiration for real-time weather metrics, SlidingExpiration for static catalog products).

**Resilience Handlers:** Built-in fault handling powered by Microsoft.Extensions.Http.Resilience implementing layered 3-attempt exponential backoff parameters.

**Fault Isolation:** High architecture survival structures. Individual endpoint exceptions are caught gracefully, logging internal faults while supplying warning diagnostics to consumers without dropping healthy payload fragments.


**🧪 4. Execution of Test Suites**
To validate reliability metrics, execute the xUnit test runner locally from the command line:

dotnet test
