# DotNetCore-Authentication-AzureAD

Azure Active Directory + Swagger authentication + ASP.NET Core 3.1

# How settings Azure AD

Settings start with <a href="https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal#register-an-application-with-azure-ad-and-create-a-service-principal" target="_blank">`Register an application with Azure AD`</a>.

After register the application. 

Select **API permissions** use the *User.Read* (added by default) permission.

<p align="center">
  <img src="https://image.prntscr.com/image/2448lpl1S7OGzzpERgJzqQ.png"/>
</p>

After select **Authentication** click the button *Add a platform*.

<p align="center">
  <img src="https://image.prntscr.com/image/zmZP-zYNSUGuJwqGvPsqeQ.png"/>
</p>

Next **Configure platforms** select *Web*.

<p align="center">
  <img src="https://image.prntscr.com/image/5fIQWufmQ-2TvF-hdxEuEw.png"/>
</p>

Finally **Configure Web** fill in the first text box *https://localhost:44315/swagger/oauth2-redirect.html*. 

Section *Implicit grant* for web app, select *ID tokens* and *Access tokens*.

<p align="center">
  <img src="https://image.prntscr.com/image/tv7HBXAcSKuA1VwyRgQHhA.png"/>
</p>

For more details visit website <a href="https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-access-web-apis#configure-platform-settings-for-your-application" target="_blank">`Configure platform settings for your application`</a>.

We have completed the Azure AD settings.

# Settings appsettings.json

In the appsettings.json find section **AzureAd**

```
"AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "https://login.microsoftonline.com/{tenantId}/oauth2/authorize",
    "TenantId": "{tenantId}",
    "ClientId": "{clientId}",
    "CallbackPath": "/"
}
```
Replace the *{tenantId}* to your application with azure portal.

<p align="center">
  <img src="https://image.prntscr.com/image/xJCTLe2lR4uzhTUN7YWQQg.png"/>
</p>

Replace the *{clientId}* to your application with azure portal.

<p align="center">
  <img src="https://image.prntscr.com/image/9pH7eTvdSS6vQkbffFwMfQ.png"/>
</p>

For more details visit website <a href="https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal#get-tenant-and-app-id-values-for-signing-in" target="_blank">`Get tenant and app ID values for signing in`</a>.

We have finished setting up appsettings.json.
