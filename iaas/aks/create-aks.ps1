$rg = "aksdemos"
$cluster = "akscluster"

az group create -n $rg `
 -l westus

az aks create -g $rg `
 -n $cluster `
 --node-count 1 `
 --generate-ssh-keys `
 --enable-addons monitoring

az aks get-credentials `
 -g $rg `
 -n $cluster

kubectl get nodes

kubectl apply -f azure-vote.yaml

kubectl get service azure-vote-front `
 --watch
