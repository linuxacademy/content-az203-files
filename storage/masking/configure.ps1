$rgName="sql"
$location="southcentralus"
$serverName="laaz200sqlserver"
$adminLogin="ServerAdmin"
$adminPassword="Change!Me!Please"
$dbName="masking"
$startIP="0.0.0.0"
$endIP="0.0.0.0"

az group create -n $rgName -l $location
az sql server create -g $rgName -l $location -n $serverName `
  --admin-user $adminLogin --admin-password $adminPassword

az sql server firewall-rule create `
  --resource-group $rgName --server $serverName -n AllowMyIp `
  --start-ip-address $startIP `
  --end-ip-address $endIP

az sql db create `
  -g $rgName `
  -s $serverName `
  -n $dbName `
  --service-objective S0 

CREATE TABLE laaz200dm.dbo.Users (
  UserId int NOT NULL,
  AccountCode varchar(50) NOT NULL,
  Pin varchar(10) NOT NULL,
  Name varchar(50) NOT NULL
) GO;
  
INSERT INTO Users VALUES(1, '123-45-6789', 'ABCD', 'Mike')
SELECT * FROM Users;

New-AzureRmSqlDatabaseDataMaskingRule `
  -ServerName $serverName `
  -DatabaseName $dbName `
  -ResourceGroupName $rgName `
  -SchemaName "dbo" `
  -MaskingFunction Text `
  -TableName "Users" `
  -ColumnName "AccountCode" `
  -SuffixSize 4 `
  -ReplacementString "xxxx"

Remove-AzureRmSqlDatabaseDataMaskingRule `
  -ServerName $serverName `
  -DatabaseName $dbName `
  -ResourceGroupName $rgName `
  -SchemaName "dbo" `
  -TableName "Users" `
  -ColumnName "AccountCode" 

USE masking;
GO
CREATE USER user1 WITHOUT LOGIN;
GRANT SELECT ON OBJECT::dbo.Users TO user1;  
GO

USE masking;
EXECUTE AS USER = 'user1';
SELECT * FROM Users;
REVERT;
  