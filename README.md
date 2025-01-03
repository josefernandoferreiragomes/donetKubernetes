## Running `mslearn-dotnet-cloudnative` Project locally
### ASP.NET Web Api
### Blazor web site

## Prerequisites
- Docker installed
- Docker Hub account
- k3d installed or alternatively Minikube 
- kubectl installed
- VS Code

### experimental
Azure pipeline files
 
## Steps

### to run the app, from the existing repo, after all software is installed, and the docker images are published to docker hub
```bash
k3d cluster create devcluster --config k3d.yml
```
```bash
kubectl apply -f backend-deploy.yml
```
or if already created
```bash
k3d cluster start devcluster
```

test backend
http://localhost:32001/api/product

```bash
kubectl apply -f frontend-deploy.yml
```

test frontend
http://localhost:32000/

Re deploy the containers
https://medium.com/@haroldfinch01/how-do-i-force-kubernetes-to-re-pull-an-image-cf2b8c4854bc
```bash
kubectl rollout restart deployment <deployment-name>
```

### To run the app, starting with local software installation

### 1. Clone the Repository
```bash
git clone https://github.com/MicrosoftDocs/mslearn-dotnet-cloudnative.git
cd mslearn-dotnet-cloudnative
```
### 2. Run the following command to build the containers (instead of using a dockerfile), just for testing purposes
```bash
dotnet publish /p:PublishProfile=DefaultContainer
or
docker-compose build
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

Install chocolatey, in powershell
https://docs.chocolatey.org/en-us/choco/setup/?form=MG0AV3
Paste, and enter:
```bash
Set-ExecutionPolicy Bypass -Scope Process -Force
```
then paste:
```bash
$installer = "https://community.chocolatey.org/install.ps1"
iex ((New-Object System.Net.WebClient).DownloadString($installer))
```
then, enter

Install kubectl
```bash
choco install kubernetes-cli -y
```

enable WSL2
```bash
wsl --set-default-version 2
```

install k3d, using chocolatey and then verify installation
```bash
choco install k3d -y
```

verify k3d version
```bash
k3d --version
```

### create k3d cluster
```bash
k3d cluster create devcluster --config k3d.yml
```

### In case you need to delete and create again, delete k3d cluster
```bash
k3d cluster delete devcluster
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

### 11. Test the api in k3d, in the browser, concatenating localhost with the url
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

### Add OpenTelemetry
https://learn.microsoft.com/en-us/training/modules/implement-observability-cloud-native-app-with-opentelemetry/4-exercise-add-observability-cloud-native-app

Add a Diagnostics project to the solution

Add OpenTelemetry packages

```bash
cd Diagnostics

dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.EventCounters --prerelease
dotnet add package OpenTelemetry.Instrumentation.Runtime
dotnet add package OpenTelemetry.Instrumentation.SqlClient --prerelease
dotnet add package OpenTelemetry.Instrumentation.Http
```

Add the code to use OpenTelemetry

Add Diagnostics project reference in Products

In program.cs add the following code
```csharp
builder.Services.AddObservability("Products", builder.Configuration);
```

### Update images
Tutorial
    https://docs.docker.com/get-started/docker-concepts/building-images/build-tag-and-publish-an-image/

Created dockerfiles and updated its directories

Commands to build
    ```bash    
    docker build -t josefernandoferreiragomes/productservice -f ./products/Dockerfile .
    docker build -t josefernandoferreiragomes/storeimage -f ./store/Dockerfile .
    ```

Commands to tag
```bash
docker tag josefernandoferreiragomes/productservice:latest josefernandoferreiragomes/productservice:latest
docker tag josefernandoferreiragomes/storeimage:latest josefernandoferreiragomes/storeimage:latest
```

Commands to push
```bash
docker push josefernandoferreiragomes/productservice:latest
docker push josefernandoferreiragomes/storeimage:latest
```

local pods update
```bash
kubectl set image deployment/productsbackend productsbackend=josefernandoferreiragomes/productservice:latest
kubectl set image deployment/storefrontend storefrontend=josefernandoferreiragomes/storeimage:latest
```

verify the deployment was updated
```bash
kubectl get deployment productsbackend -o yaml
kubectl get deployment storefrontend -o yaml
```

you should see in the output the image field under the containers section to confirm it has the new image tag josefernandoferreiragomes/storeimage:latest

to delete the old pods
```bash
kubectl delete pods -l app=productsbackend
kubectl delete pods -l app=storefrontend
```

### Open app

    http://localhost:32000

### verify the telemetry logs
```bash
kubectl get pods
kubectl logs <pod-name>

kubectl logs storefrontend-9cbdb4547-sn6wk -c storefrontend
```

Output to file
```bash
kubectl logs productsbackend-659d569fb8-m76h4 > productsbackendlogsexample.txt
```

Should see a log like 
```
(2025-01-02T16:37:04.9174192Z, 2025-01-02T17:04:20.9855429Z] http.request.method: PUT http.response.status_code: 200 http.route: /api/Stock/{id} network.protocol.version: 1.1 url.scheme: http Histogram
Value: Sum: 0.0926551 Count: 1 Min: 0.0926551 Max: 0.0926551 
```

```bash
kubectl logs -f <pod-name>
```

### Rolling update
Make a change to the site
then update the frontend-deploy.yml
```bash
spec:
  replicas: 3
  strategy: 
    type: RollingUpdate 
    rollingUpdate: 
      maxUnavailable: 1 
      maxSurge: 1
```

Apply deployment
```bash
kubectl apply -f frontend-deploy.yml
```

Ùpdate the image
```bash
kubectl set image deployment/storefrontend storefrontend=josefernandoferreiragomes/storeimage:latest

```

Monitor the update
```bash
kubectl rollout status deployment/storefrontend

kubectl get pods
```

### View telemetry data in 3rd party tools

https://learn.microsoft.com/en-us/training/modules/implement-observability-cloud-native-app-with-opentelemetry/5-view-telemetry-data

Use OpenTelemetry data in a cloud-native application

Adds two new services, Prometheus and Grafana. The Prometheus section configures a container to respond on port 9090. It maps the prometheus folder expecting a prometheus.yml file. The Grafana section configures a container to respond on port 3000. It maps three folders inside a grafana folder.

Configure Grafana

Update ASP.NET Core app to expose metrics for Prometheus

Optionally remove previous OpenTelemetry package, but may be maintained for demonstration and comparisson purposes

Add the OpenTelemetry.Exporter.Prometheus.AspNetCore package:
```bash
dotnet add package OpenTelemetry.Exporter.Prometheus.AspNetCore --prerelease
```

#### Add Prometheus exporter on Diagnostics

```csharp
.AddPrometheusExporter();
```

Test the new observability features

```bash
dotnet publish /p:PublishProfile=DefaultContainer
docker compose build
docker compose up
```

Open Prometheus (9090). If you're running locally in Visual Studio Code, open a browser and, on a new tab, go to the Prometheus app
http://localhost:9090

#### Add Grafana

Open in Browser for Grafana (3000). If you're running locally in Visual Studio Code, open a browser and, on a new tab, go to the Grafana app
http://localhost:3000.

credentials: u: admin, p: grafana

Import dashboard
https://github.com/dotnet/aspire/blob/main/src/Grafana/dashboards/aspnetcore.json

In the Prometheus data source dropdown, select Prometheus.

#### Extend the tracing capabilities of the app by adding Zipkin

Add a Zipkin container to your app and configure it to connect to the OpenTelemetry collector. 
Then you add the OpenTelemetry Zipkin exporter to your app.

Add zipkin dependency to site and api

Add zipkin package to Diagnostics project
```bash
dotnet add package OpenTelemetry.Exporter.Zipkin --prerelease
```

Add tracing providers to DiagnosticServiceCollectionExtensions.cs
```csharp
// add the tracing providers
.WithTracing(tracing =>
{
  tracing.SetResourceBuilder(resource)
              .AddAspNetCoreInstrumentation()
              .AddHttpClientInstrumentation()
              .AddSqlClientInstrumentation()
              .AddZipkinExporter(zipkin =>
              {
                var zipkinUrl = configuration["ZIPKIN_URL"] ?? "http://zipkin:9411";
                zipkin.Endpoint = new Uri($"{zipkinUrl}/api/v2/spans");
              });
});
```

Build and run the app

Open Zipkin (make some requests from store web site)
http://localhost:9411

Explore dependencies and traces (it takes a few minutes to show up)

### Troubleshoot api connectivity from within
```bash
docker exec -u root e0f14f07f267 apt update
docker exec -u root e0f14f07f267 apt install -y curl
docker exec -u root e0f14f07f267 apt install -y net-tools
docker exec -u root e0f14f07f267 netstat -tuln
docker exec -u root e0f14f07f267 curl -v http://localhost:8080/metrics
```

### Extend telemetry

https://learn.microsoft.com/en-us/training/modules/implement-observability-cloud-native-app-with-opentelemetry/7-exercise-extend-telemetry

Create a custom metric: 

Creates a new metric called eshoplite.products.stock_change. This metric tracks the amount of stock being changed through the product service

Add the metric to OpenTelemetry:

Add the metric to OpenTelemetry so that it can be exported to your observability tools

View the new metric in Prometheus:

    Open the eShopLite app in a browser at http://localhost:32000
    Go to the Products page and change the stock amount on several products
    Open Prometheus in a browser at http://localhost:9090

    In the search box, enter theeshoplite_products_stock_change_total metric and then select Execute.
    You should see it listed in a table.
    Select the Graph tab. You should see the stock amount change over time.

Docker build with no cache
```bash
docker compose build --no-cache
```

In case you need to debug, add the following to the docker-compose.yml
```yml
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    ports:
      - "8080:80"
    volumes:
      - .:/app
```

Or, change Release with Debug in the Dockerfile

And then attatch the debugger -> Local -> container -> frontend

### Add compliance

https://learn.microsoft.com/en-us/training/modules/dotnet-compliance-cloud-native-applications/3-exercise-classify-sensitive-data-cloud-native-application

Add code to create two new taxonomies. Then annotate the Product and Order data types with the appropriate attributes.

In Data Entities project, add package
```bash
dotnet add package Microsoft.Extensions.Compliance.Redaction
```

Add new Compliance class

The new Code creates two taxonomies, EUII and EUPI. It also creates two attributes, EUIIDataAttribute and EUPDataAttribute. These attributes are used to annotate the data types.

Use these taxonomies and attributes to classify the data types in the eShopLite app.

https://learn.microsoft.com/en-us/training/modules/dotnet-compliance-cloud-native-applications/4-redact-sensitive-data-cloud-native-application

The .NET logging framework provides a simple way to redact data in log messages. The Microsoft.Extensions.Compliance.Abstractions package enhances logging to include a Redactor class that redacts data


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

#### Install linkerd on windows

Get latest linkerd version from release page on GitHub
https://github.com/linkerd/linkerd2/releases/


Add linkerd to path (WSL)
```bash
$env:PATH += ";C:\Program Files\linkerd2-main"
```

### Comment the c# code which was added previously to implement resiliency
```csharp
//using Microsoft.Extensions.Http.Resilience;
//(...)
.AddStandardResilienceHandler(options =>
{
    //default timeout was 30s, but we changed it to 260s
    options.Retry.MaxRetryAttempts = 7;
    options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
    {
        Timeout = TimeSpan.FromMinutes(5)
    };
})
```

### Remove the reference to resiliency package
```bash
dotnet remove package Microsoft.Extensions.Http.Resilience
```
Check whether linkerd is properly installed:
```bash
linkerd check --pre
```

Generates a Kubernetes manifest with the necessary control plane resources:
```bash
linkerd install --crds | kubectl apply -f -
```

The generated manifest is piped to kubectl apply, which installs those control plane resources in the Kubernetes cluster:
```bash
linkerd install --set proxyInit.runAsRoot=true | kubectl apply -f -
```

Add the linkerd.io/inject: enabled annotation to the backend-deploy.yml file under template metadata.
```bash
template:
    metadata:
      annotations:
        linkerd.io/inject: enabled
      labels: 
```

publish the updated image do to docker hub
```bash
docker push josefernandoferreiragomes/storeimage
```

update kubernetes
```bash
kubectl apply -f backend-deploy.yml,frontend-deploy.yml
```


In powershell, The linkerd manifest is configured as:

Any idempotent HTTP GET route matching the pattern /api/Product can be retried.
Retries can add no more than an extra 20 percent to the request load, plus another 10 "free" retries per second.
Run the following command to use the service profile in the Kubernetes cluster:

```bash
$yaml = @"
apiVersion: linkerd.io/v1alpha2
kind: ServiceProfile
metadata:
  name: backend
  namespace: default
spec:
  routes:
  - condition:
      method: GET
      pathRegex: /api/Product
    name: GET /v1/products 
    isRetryable: true
  retryBudget:
    retryRatio: 0.2
    minRetriesPerSecond: 10
    ttl: 120s  
"@

```

then, run:
```bash
$yaml | kubectl apply -f -
```

Linkerd has extensions to give you extra features. Install the viz extension and view the status of the app in Linkerd's dashboard.
In the terminal, run this command to install the extension:
```bash
linkerd viz install | kubectl apply -f -
```

View the dashboard with this command:
```bash
linkerd viz dashboard
```
IT will open the website
http://localhost:50750/namespaces

Check the pods

To test the resilience provided by linkerd:
Scale productsbackend to 0 replicas

Restart the product service pods ( 1 replica)

the app sould now display the products

### Uninstall linkerd

Remove Data Plane Proxies
```bash
kubectl get deploy -o yaml | linkerd uninject - | kubectl apply -f -
```

Remove extensions
```bash
linkerd viz uninstall | kubectl delete -f -
```

Remove Control Plane
```bash
linkerd uninstall | kubectl delete -f -
```

comment the yml configuration from both backend-deploy.yml and frontend-deploy.yml
```yml
metadata:
      #annotations:
      #  linkerd.io/inject: enabled
```
#### If linkerd does not install in WSL, it might be needed to install a linux distribution, like ubuntu, from microsoft store and configure a user

To navigate to windows user folders, type 
  cd /mnt/c/Users/<your-username>

Installing linkerd in linux:
```bash
curl.exe -sL https://run.linkerd.io/install
```

add linkerd to path (linux)
```bash
$env:PATH += ";$HOME\.linkerd2\bin"
```
