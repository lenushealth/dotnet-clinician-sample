# dotnet-clinician-sample
A sample clinician app that demonstrates requesting agency for another users and displaying their data samples

# Requirements

- .NET core 2.0 SDK
- Visual Studio 2017 or Visual Studio Code
- Access to an instance of the Lenus Health platform
- Access to register a new client application for use with this sample

# Creating a client application

Visit https://portal.dhi-dse.scot, register and/or login to your account and create a new client application.  The client application will be required to use the following configuration:

- RedirectUri: `http://localhost:5000/signin-oidc`
- Grant Type: `authorization code`
- Basic Scopes: `openid profile email agency_api`
- Correlation Scopes: `read.blood_pressure`
- Vitals Quantity Scopes: `read.blood_pressure.blood_pressure_diastolic read.blood_pressure.blood_pressure_systolic read.body_mass`

Set a known `client secret` value

# Updating the sample configuration files

Included in the sample is an `appsettings.development.json` file, within this file you will need to update the following configuration with values obtained with creating your client application:

```json
  "OpenIdConnect" : 
  {
     "Authority" :  "<uri-to-identity-service>",
     "ClientId" :  "<client-id>",
     "ClientSecret" : "<client-secret>"
  } 
```

The `Authority` uri will be that of the `IdentityServer` website, see the [environments](https://github.com/lenushealth/docs/blob/master/environment.md) documentation.

Then update the following section of the same configuration file:

```json
  "HealthDataClient" : 
  {
    "BaseUri" :  "<uri-to-data-service>"
  }
```

Where the `BaseUri` for the data service can also be obtained from the [environments](https://github.com/lenushealth/docs/blob/master/environment.md) documentation.

Finally, update the Agency configuration as above:

```json
  "Agency" : {
     "AgencyRequestUri" :  "<uri-to-identity-service>/agency" ,
     "AgencyApiBaseUri" :  "<uri-to-identity-service>/api/agency" 
  }
```

# Building and Running

Open the solution either in Visual Studio 2017 or using Visual Studio Code

You can also deploy an instance of this sample directly to azure using:

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)
