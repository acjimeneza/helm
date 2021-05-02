# helm

## Step 0 - Crete a basic microservice

Build the container image

```
cd Step0/ms-chart
docker build -f .docker/Dockerfile -t yourhubusername/ms-chart:a1 .
docker login <registry> -u <User> -p $(registryPassword)
docker push yourhubusername/ms-chart:a1
```

Test image

```
docker run -p 5000:5000 -d yourhubusername/ms-chart:a1
```

Open an explorer and enter to Swagger

```
http://localhost:5000/swagger
```

## Step 1 - Creating a basic chart

Check that helm is installed

```
helm version
```

Create a folder and then create a base for the chart

```
mkdir siigo_chart
cd siigo_chart

helm create ms-chart
```

Delete all files created inside the Template folder, and then create two files **Deployment.yaml** and **service.yaml**

### Deployment.yaml

Add this code to **Deployment.yaml** file

```
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: name-deploy
  labels:
    app: name-app
spec:
  selector:
    matchLabels:
      app: name-app
  replicas: 1
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 0
  template:
    metadata:
      labels:
        app: name-app
    spec:
      containers:
        - name: name-app
          image: acjimeneza/ms-chart:a1
          imagePullPolicy: Always
          ports:
            - containerPort: 5000
          resources:
            requests:
              memory: "64Mi"
              cpu: "50m"
            limits:
              memory: "256Mi"
              cpu: "500m"
```

### service.yaml

Add this code to **service.yaml** file

```
apiVersion: v1
kind: Service
metadata:
  name: name-service
  labels:
    app: name-app
spec:
  type: LoadBalancer
  selector:
    app: name-app
  ports:
    - protocol: TCP
      name: http
      port: 8000
      targetPort: 5000

```

### Testing the chart

In the folder `Step1/siigo_chart` run the code `helm template ms-chart ms-chart`. This will show an output in the console previewing the generated chart.

After checking the chart, you need to install it running this command `helm install ms-chart ms-chart`

### Checking installation

1. Chart release status: `helm ls` -> List all the releases in the default namespace and its status.
1. Deployment: `kubectl get deploy`
1. Service: `kubectl get service`

## Step 2 - Adding declarative variables

### Change labels and names using the name of the release

Reeplace all the names with the tag {{Release.name}}, for example:

```
apiVersion: v1
kind: Service
metadata:
  name: {{.Release.Name}}
  labels:
    app: {{.Release.Name}}
```

Then check the chart and update the old one running the nex command `helm upgrade ms-chart ms-chart`

### Change limits of the resouces of the release

After test the changes, lets do a conditional declaration in helm, so add the next lines in the `values.yaml` file

```
resources:
  limits:
    cpu: 100m
    memory: 128Mi
  requests:
    cpu: 100m
    memory: 128Mi
```

And then add this lines in the `deployment.yaml` file.

```
resources:
  {{- if .Values.resources }}
  {{- toYaml .Values.resources | nindent 12}}
  {{- else }}
  requests:
    memory: "64Mi"
    cpu: "50m"
  limits:
    memory: "128Mi"
    cpu: "100"
  {{- end}}
```

### Adding ConfigMap (env)

Create a file named `configmap.yaml` and add this code:

```
{{- if .Values.environmentVar}}
apiVersion: v1
kind: ConfigMap
metadata:
  name: {{ .Release.Name }}-conf
data:
  {{- range $key,$value := .Values.environmentVar}}
  {{ $key }} : {{ $value | quote }}
  {{- end}}
{{- end}}
```

then add the next lines to the file `deployment.yaml` to add the connection between the configmap and the container:

```
{{- if .Values.environmentVar }}
envFrom:
- configMapRef:
    name: {{ .Release.Name }}-conf
    optional: true
{{- end}}
```
