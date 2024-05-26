# Sr. Engineer Developer Case


## Setup

Start by installing dotnet

``` brew install dotnet ```

And docker if you haven't got it already.

``` brew install docker ```

## OrderListService

Build the container with

``` docker build -t orderlistservice:latest src/OrderListService ```

## BriefingService

Build the container with

``` docker build -t briefingservice:latest src/BriefingService ```

## Start the system

Run a

``` docker-compose up -d ```

And you should be able to connect on << LOCALHOST:PORT >>

## Outcome
- [ ] Diagram C4 (mermaid)
- [x] Github project
- [ ] Deck of slides presenting the solution

## Breakdown of the problem

OrderListMeta
     |
BriefingMeta
AssetMeta


Cached ContentDistribution


Integration layer - REST/Events - up to me




