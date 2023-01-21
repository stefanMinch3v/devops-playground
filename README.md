# Devops-playground
## The project is built as a part of the telerik course - https://www.telerikacademy.com/upskill/devops.

![GitHub Workflow Status (with branch)](https://img.shields.io/github/actions/workflow/status/stefanMinch3v/devops-playground/sonar-cloud.yml?branch=main&label=SAST)
![GitHub Workflow Status (with branch)](https://img.shields.io/github/actions/workflow/status/stefanMinch3v/devops-playground/build-pipeline.yml?branch=main)

The project covers the following topics:
- Source control
- Branching strategies
- Building pipelines
- GitOps (ArgoCD)
- Security
- Docker
- Kubernetes
- Public cloud (GCP)
- Database changes (EF)
- Phases of SDLC
- Continuous integration
- Continuous deployment (half way through)

Application tech stack:

- ASP[.]NET Core 3.1 
- Entity Framework
- MSSQL
- jQuery
- Bootstrap

### Local installation

with kubernetes
```sh
cd src\.k8s
kubectl apply -f .\.environment
kubectl apply -f .\databases
kubectl apply -f .\clients
```

with docker-compose
```sh
cd src
docker-compose up
```

Verify the deployment by navigating to your server address in
your preferred browser.

```sh
127.0.0.1:5010
```

## High level overview diagram
![High level overview diagram](https://i.imgur.com/oLP2s1O.jpg)

## Flow chart diagram
![High level overview diagram](https://i.imgur.com/z1PddEh.jpg)
