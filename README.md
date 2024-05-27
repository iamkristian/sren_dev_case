# Sr. Engineer Developer Case


## Setup

Start by installing dotnet

``` brew install dotnet ```

And docker if you haven't got it already.

``` brew install docker ```

## Start the system

Run a

``` docker-compose up -d ```

And you should be able to connect to [Orderlist Service](http://localhost:5219/swagger) and [Briefing Service](http://localhost:5054/swagger)

### Testing the system

In the project root, you can fire

``` dotnet test ```

To run the test cases. It will fire up the applications and run the tests against an in memory database.


## OrderListService

Build the container with

``` docker build -t orderlistservice:latest src/OrderListService ```

## BriefingService

Build the container with

``` docker build -t briefingservice:latest src/BriefingService ```

## Outcome
- [ ] Diagram C4 (mermaid)
- [x] Github project
- [x] Deck of slides presenting the solution

