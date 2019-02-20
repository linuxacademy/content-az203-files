$spname = "LaAz203Principal1"
$webappspname = "LaAz203SampleWebAppPrincipal"

$sp1 = az ad sp create-for-rbac --name $spname | ConvertFrom-Json
$sp1

az ad sp show --id $sp1.appId
az ad sp list --display-name $spname

az role assignment list --assignee $sp1.appId

az role assignment delete --assignee $sp1.appId --role Contributor
az role assignment create --assignee $sp1.appId --role Reader

az role assignment list --assignee $sp1.appId

az role definition list --output json --query '[].{"roleName":roleName, "description":description}'
az role definition list --custom-role-only false --output json --query '[].{"roleName":roleName, "description":description, "roleType":roleType}'
az role definition list --name "Contributor"
az role definition list --name "Contributor" --output json --query '[].{"actions":permissions[0].actions, "notActions":permissions[0].notActions}'

# make this SP able to publish to a web site
$sampleweb = az webapp show `
 --name laaz203samplewebapp `
 -g laaz203samplewebapp  | ConvertFrom-Json
$sampleweb.id

az role assignment create `
 --role "Website Contributor" `
 --assignee $sp1.appId `
 --scope $sampleweb.id


az sql server list


 # Note: PS Equivalent: New-AzureRmRoleAssignment
 
$webappsp = az ad sp create-for-rbac --name $webappspname | ConvertFrom-Json
$webappsp

 # pseudo code to assign
az webapp identity assign `
 -g laaz203samplewebapp `
 -n laaz203samplewebapp 

az sql server ad-admin create `
 --resource-group <resource-group-name> `
 --server-name <server_name> `
 --display-name <admin_user> `
 --object-id <principalid_from_last_step>

az webapp config connection-string set `
 --resource-group <resource-group-name> `
 --name <app name> `
 --settings MyDbConnection='Server=tcp:<server_name>.database.windows.net,1433;Database=<db_name>;' `
 --connection-string-type SQLAzure

az ad sp delete --id $webappsp.appId

