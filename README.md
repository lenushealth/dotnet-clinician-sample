# dotnet-clinician-sample
A sample clinician app that demonstrates requesting agency for another users and displaying their data samples

# Requirements

- .NET core 3.1 SDK
- Visual Studio 2019 or Visual Studio Code
- Access to an instance of the Lenus Health platform
- Access to register a new client application for use with this sample

# Creating a client application

Visit https://portal.dhi-dse.scot, register and/or login to your account and create a new client application.  The client application will be required to use the following configuration:

- RedirectUri: `https://localhost:5001/signin-oidc`
- Grant Type: `authorization code`
- Basic Scopes: `openid`, `profile`, `email`, `agency_api`
- Correlation Scopes: `read.blood_pressure`
- Body Quantity Scopes: `read.height`, `read.body_mass`
- Fitness Quantity Scopes: `read.step_count`
- Vitals Quantity Scopes: `read.heart_rate`, `read.blood_pressure.blood_pressure_diastolic`, `read.blood_pressure.blood_pressure_systolic`

Set a known `<client-secret>` value

# Deploy to Azure

Deploying an instance directly to azure is the quickest way to see the sample in action, this will of course require that you have an active Azure subscription into which the resources can be created.  Use the button below to start the deployment

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

You will need to know:

- The absolute url of a health platform identity instance
- The absolute url of the data store api associated with the health platform identity instance
- The `client-id` created above
- The `client-secret` set for the client application
- The `redirect-uri`, this will be in the form `https://<site-name>.azurewebsites.net/signin-oidc`.  See the value you use for the **Site Name** parameter during deployment

# Running Locally

## Updating the sample configuration files

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

## Building and Running

Open the solution either in Visual Studio 2017 or using Visual Studio Code.
