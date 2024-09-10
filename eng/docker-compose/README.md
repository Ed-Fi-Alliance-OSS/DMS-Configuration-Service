# Keycloak developer setup

The purpose of this document is to provide the basic steps for configuring Keycloak locally using docker-compose.

### 1.	A yaml file "keycloak.yml" (eng\docker-compose\keycloak.yml) was created that contains the configuration needed to lift the container using the command:
`docker compose -p keycloak_demo -f keycloak.yml up -d`

 ![alt text](image.png)

### 2.	Once either of the two previous commands is executed, you can validate that keycloak is up and running in Docker Desktop.
  ![alt text](image-1.png)

### 3.	Now, you can enter the URL: http://localhost:8081/


### 4.	On this page, you will be asked for your username (admin) and password (admin)
![alt text](image-2.png)

### 5.	Once authenticated, you will enter the settings.
![alt text](image-3.png)

### 6.	In the upper left of the scream, you must create a new realm, called “edfi”.
![alt text](image-4.png)

### 7.	In this configuration, we will only be asked for the name and click on Create
![alt text](image-5.png)

## 8.	The home screen will change this way
![alt text](image-6.png)

### 9.	We will proceed to the configuration of the client by clicking Clients, which is located in the left panel (Note: make sure you are in edit and not in Master).
![alt text](image-7.png)

## #10.	In General settings, we will assign a Client ID
![alt text](image-8.png)

### 11.	En Capability config, we will enable 
a.	Client authentication
b.	Authorization
c.	Authentication Flow, check-in Standard flow
![alt text](image-9.png)

### 12.	In Login settings, we will indicate the Root URL
![alt text](image-10.png)

### 13.	Click Save
 ![alt text](image-11.png)
