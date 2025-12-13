# Introduction
## Câmara de Regulação do Mercado de Medicamentos (CMED) 💊

CMED is the brazillian Public agency responsible for economical regulation of the pharmaceutical market. The names, EAN codes and the max prices for most of the drugs available in Brazil are updated monthly to a .xlsx file made public [here](https://www.gov.br/anvisa/pt-br/assuntos/medicamentos/cmed/precos)



# The Project 👨🏻‍💻
This project provides an easy way to automatically keep updated with the latest drug prices, names & EAN code by scrapping the site periocally, looking for changes.

## Endpoints 🔗

The updated file is downloadable from the /conformity endpoint.

Check if your file is updated, by sending the /conformity/is-updated endpoint with your file creation date (for example 12/12/2025 21:16:56 -03:00) as a url parameter.

There is a Swagger UI enpoint /swagger that makes it easy to undertand the API.

# Build and Test 🏗️
This project uses .net10, sou you need to have the sdk installed. Then you can run:

```bash
dotnet build -c Release
```

```bash
dotnet run -c Release
```

## Configuration ⚙️
The configurable parameters for this API are in the `appsettings.json` and `appsettings.Development.json` files.

### ConformitySiteUrl
The site url from where the .xlsx is obtained.

### ConformityOutputDirectoryName
The directory name where the processed .csv file must be stored.

### ConformityFileName
The name of the file where the .csv content will be stored.

### SleepTimeInMilliseconds
This parameter controls the frequency of the queries in the CMED site.
The file is updated monthly, so there is no need to query more than a couple times a day, ***adjust accordingly***.

# Disclaimer ‼️
This project has no commercial intentions.
