# Running `mslearn-dotnet-cloudnative` Project

## Prerequisites
- Docker installed
- Docker Hub account
- k3d installed or alternatively Minikube 
- kubectl installed

## Steps

### 1. Clone the Repository
```bash
git clone https://github.com/MicrosoftDocs/mslearn-dotnet-cloudnative.git
cd mslearn-dotnet-cloudnative
```
### 2. Run the following command to build the containers
```bash
dotnet publish /p:PublishProfile=DefaultContainer
```

### 3. Run the following command to run the app and attach the containers
```bash
docker compose up
``` 

### 4. test the api
go to docker desktop and expand dotnet-kubernetes
select the port where the backend-1 is running, as it will open in browser
append "/api/product" to the url
view the api response for a list of products

### 5. test the store app
in docker desktop, select the port where the frontend-1 is running, as it will open in browser
select products in the left menu
view the product list with pictures and remaining data

### 6. Close the web site, return to the TERMINAL tab
press CTRL + C. Docker compose halts the containers.

### 7. Sign in to Docker Hub
docker login
(Use the same username and password from when you created your Docker account)

### 8. Upload the images to Docker Hub
retag or rename the Docker images you created under your Docker username:
```bash
docker tag store josefernandoferreiragomes/storeimage
docker tag products josefernandoferreiragomes/productservice
```
upload, or push, the Docker images to Docker Hub:
```bash
docker push josefernandoferreiragomes/storeimage
docker push josefernandoferreiragomes/productservice
```
#### Make sure your are on the right repo:
```bash
git remote show origin
```
### 9. Configure and start kubernetes
### In windows machine

install kubectl
```bash
choco install kubernetes-cli -y
```

enable WSL2
```bash
wsl --set-default-version 2
```

install k3d, using chocolatery and then verify installation
```bash
choco install k3d -y

k3d --version
```

### create k3d cluster
```bash
k3d cluster create devcluster --config k3d.yml
```

#### Alternatively, use minikube to orchestrate the containers
make sure to have installed minikube (kubectl)
```bash
minikube start

minikube start `
  --ports=32000:32000,32001:32001 `
  --extra-config=apiserver.advertise-address=0.0.0.0 `
  --extra-config=apiserver.bind-address=0.0.0.0 `
  --extra-config=apiserver.secure-port=6443 `
  --wait=60s
```

### 10. Deploy and run the backend microservice
make sure deployment file backend-deploy.yml for the backend is created, with yml from the training module

```bash
kubectl apply -f backend-deploy.yml
```

get clusters
```bash
kubectl config get-contexts
```
get pods
```bash
kubectl get pods
```

get minikube ip (minikube only)
```
minikube ip
```

get services
```bash
kubectl get services --all-namespaces
```

check status (minikube only)
```bash
minikube status
```

### 11. Test the api in k3d, in the browser, concatenating localhost with the 
http://localhost:32001/api/product

#### alternatively, open tunnel and test the api (minikube only)
```bash
minikube service productsbackend
```
the url opened in browser will be something like http://127.0.0.1:64969
just append /api/product and obtain a json list of products (http://127.0.0.1:64969/api/product)

### 12. Create a deployment file and run the frontend service
Make sure deployment file frontend-deploy.yml for the frontend is created, with yml from the training module
Duplicate the terminal/open new terminal
```bash
kubectl apply -f frontend-deploy.yml
```

### 12. test the frontend, in the browser
```bash
kubectl get services
```
the url should be something like:
http://localhost:32000/

#### alternatively, open tunnel and test the site (minikube only)
```bash
minikube service storefrontend
```
The url opened in browser will be something like http://127.0.0.1:65077
Make sure the product list loads correctly

### 13. Scale a container instance in Kubernetes
Your microservice might come under heavy load during certain times of the day. Kubernetes makes it easy to scale your microservice by adding more instances for you.
Duplicate the terminal/open new terminal
```bash
kubectl scale --replicas=5 deployment/productsbackend
```
Verify the instances are created
```bash
kubectl get pods
```

### 14. To scale the instance back down, run the following command
```bash
kubectl scale --replicas=1 deployment/productsbackend
```

## References
https://learn.microsoft.com/en-us/training/modules/dotnet-deploy-microservices-kubernetes/4-exercise-deploy-to-kubernetes

# Resiliency approaches

#### Retry
#### Circuit Breaker
### References
https://learn.microsoft.com/en-us/training/modules/microservices-resiliency-aspnet-core/2-application-infrastructure-resiliency

### Modify the app to add code-based resiliency handling policies in a microservice. 

#### Add resiliency NuGet package:
```bash
dotnet add package Microsoft.Extensions.Http.Resilience
```
#### Add resiliency to the http client
```csharp
//(...)
.AddStandardResilienceHandler()
// further options added in Program.cs
```

#### List publish profiles
```bash
minikube profile list
```

#### Rebuild the app
```bash
dotnet publish /p:PublishProfile=DefaultContainer
```

#### Start only on docker, for simplified log access
```bash
docker compose up
```
#### Access the product list on store and make sure it works.

#### Stop products service container, and try to access the products list again. And then review the store container logs:
```bash
eshoplite-frontend-1  | warn: Polly[3]
eshoplite-frontend-1  |       Execution attempt. Source: 'ProductService-standard//Standard-Retry', Operation Key: '', Result: 'Name or service not known (backend:8080)', Handled: 'True', Attempt: '2', Execution Time: '27.2703'
```
it means the retries have ocurred

### In case you want to do the test in minikube:
### Re-publish the version to docker hub, and start minikube service and store services.

#### Access the container inside minikube (in case needed)
```bash
kubectl exec --stdin --tty storefrontend-78fc97b957-lmsls -- /bin/bash
```
#### Access the container logs inside minikube
```bash
kubectl logs storefrontend-78fc97b957-lmsls
```
### Reconfigure Kubernetes deployment to implement an infrastructure-based resiliency