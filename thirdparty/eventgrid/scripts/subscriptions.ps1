
$egrgname = "eventgrid"
$subname = "login"

az eventgrid event-subscription delete
az eventgrid topic list

az eventgrid event-subscription create `
 -g rg1 `
 --topic-name topic1 `
 --name es1 `
 --endpoint https://contoso.azurewebsites.net/api/f1?code=code

$funcappname = ""
#$funcappdns = "$funcappname.azurewebsites.net"
$funcappdns = "44a40dfd.ngrok.io"
$loginFuncUrl = "https://$funcappdns/runtime/webhooks/eventgrid?functionName=LoginEventConsumer"
$logoutFuncurl = "https://$funcappdns/runtime/webhooks/eventgrid?functionName=LogoutEventConsumer"

az eventgrid event-subscription create `
 -g  $egrgname `
 --topic-name laaz203egcustomtopic `
 --name subscription1 `
 --endpoint $loginFuncUrl

 az eventgrid event-subscription create `
 -g  $egrgname `
 --topic-name laaz203egcustomtopic `
 --name subscription2 `
 --endpoint $logoutFuncUrl

az eventgrid topic key list `
 -n laaz203egcustomtopic `
 -g $egrgname

az eventgrid event-subscription delete `
 --topic-name laaz203egcustomtopic `
 -n subscription1 `
 -g $egrgname

az eventgrid event-subscription delete `
--topic-name laaz203egcustomtopic `
-n subscription2 `
 -g $egrgname


 az eventgrid event-subscription create `
 -g  $egrgname `
 --topic-name laaz203egcustomtopic `
 --subject-begins-with Login `
 --name subscription1 `
 --endpoint $loginFuncUrl


 az eventgrid event-subscription create `
 -g  $egrgname `
 --topic-name laaz203egcustomtopic `
 --subject-begins-with Logout `
 --name subscription2 `
 --endpoint $logoutFuncUrl

