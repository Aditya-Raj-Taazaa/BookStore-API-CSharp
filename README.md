# BookStore 📚 API

                                                 Request
                                                    /\
                                                    ||
                                                    \/
                                                Controller
                                                    /\
                                                    ||
                                                    \/
                                                 Services
                                                    /\
                                                    ||
                                                    \/
                                                Repository
                                                    /\
                                                    ||
                                                    \/
                                        Data Storage (PSQL, SQL, Redis)


## Components :

### Controllers
    |
    |- Mediator between External Request/Client Layer and Service Layer
    |
    |- Consists of API paths

### Services
    |
    |- Mediator between Controllers and Repository layer
    |
    |- Consists of Business Logic

### Repositories 
    |
    |- Connects Services and Storage Layer to enhance Abstraction.
    |
    |- Consists of Concrete Implementation.

### Interfaces
    |
    |- Enforces Abstraction and Encapsulation with Loose Coupling between Components.

### DTOs (Data Transfer Objects)
    |
    |- Abstraction and Behaviour Enforcing for Domains and its Methods.

### Middlewares
    |
    |- Logs HTTP method with Color Grading based on Specific type of Request, path, and timestamp 

## Data Storage Layer :

### PSQL 
* Uses EFCore (ORM) for performing transactions with PSQL Database.

### Domains

          |- Id 
          |- AuthorId <----------------------------------|   
    BOOK--|- Title                                       |
          |- Price                                       |
          |- Author (Fetch using Navigation Property)    |
                                                         |
            |- Id <--------------------------------------|
    AUTHOR--|- Name                                      
            |- Bio                                       

