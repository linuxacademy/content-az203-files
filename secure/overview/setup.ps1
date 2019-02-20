$webappname = "laaz203samplewebapp"
$webapprgname = "laaz203samplewebapprg"
$dbrgname = "laaz203samplewebappdbrg"
$location = "westus"
$sqlservername = "laaz203samplewebappsql"
$dbadminusername = "laaz203samplewebapp"
$dbadminpassword = "LaAz!203"
$dbname = "laaz203samplewebappdb"
$webappplanname = "laaz203samplewebappplan"

az group create -n $dbrgname -l $location

az sql server create `
 -n $sqlservername `
 -g $dbrgname `
 -l $location `
 --admin-user $dbadminusername `
 --admin-password $dbadminpassword

az sql server firewall-rule create `
 -g $dbrgname `
 --server $sqlservername `
 --name AzureAccess `
 --start-ip-address 0.0.0.0 `
 --end-ip-address 0.0.0.0

$ipinfo = Invoke-RestMethod http://ipinfo.io/json 
$ipinfo.ip

az sql server firewall-rule create `
 -g $dbrgname `
 --server $sqlservername `
 --name AzureAccess `
 --start-ip-address $ipinfo.ip `
 --end-ip-address $ipinfo.ip

az sql db create `
 -g $dbrgname `
 --server $sqlservername `
 --name $dbname `
 --service-objective S0 `
 --sample-name AdventureWorksLT

az sql db show-connection-string `
 --name $dbname `
 --server $sqlservername `
 --client ado.net

# now web app stuff

# az appservice plan delete -n $webappplanname -g $webapprgname --yes

#az webapp deployment user set `
# --user-name laaz203deploy `
# --password LaAz!203

# build/publish the app and zip it up
Push-Location app
$publishFolder = "publish"
if (Test-path $publishFolder) { Remove-Item -Recurse -Force $publishFolder }
dotnet publish -c release -o $publishFolder

$destination = "publish.zip"
if (Test-path $destination) { Remove-item $destination }

Compress-Archive -Path $publishFolder/* -DestinationPath $destination -Force

Pop-Location

az group create -n $webapprgname -l $location

az appservice plan create `
 -n $webappplanname `
 -g $webapprgname `
 --sku S1 `
 --is-linux 

# create web app
az webapp create `
 -g $webapprgname `
 --plan $webappplanname `
 -n $webappname `
 --runtime "DOTNETCORE|2.2" 

$hostname = az webapp show `
 -g $webapprgname `
 -n $webappname `
 --query defaultHostName `
 -o tsv
$url = "https://" + $hostname
$url

Start-Process $url

az webapp deployment source config-zip `
 -g $webapprgname `
 -n $webappname `
 --src app/publish.zip 

#$localgit = "https://laaz203deploy@" + $webappname + ".scm.azurewebsites.net/laaz203samplewebapp.git"
#$localgit

#Compress-Archive -Path $publishFolder/* -DestinationPath app.zip -Force


#$localgit

#git init
#git add .
#git commit -m "first commit"
# git remote add azure $localgit



$connectionString = "Server=tcp:laaz203samplewebappsql.database.windows.net,1433;Database=laaz203samplewebappdb;User ID=laaz203samplewebapp;Password=LaAz!203;Encrypt=true;Connection Timeout=30;"

az webapp config connection-string set `
 -g $rgname `
 -n $webappname `
 --settings MyDbConnection=$connectionString `
 --connection-string-type SQLServer

az webapp config appsettings set `
 -n $webappname `
 -g $rgname `
 --settings ASPNETCORE_ENVIRONMENT="Production"

az group delete -n $webapprgname --yes
