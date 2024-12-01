# Design Diagram

```mermaid
---
title: Mediator Design Pattern
---
classDiagram
    direction BT
    ConcreteMediator ..> Mediator: implements
    ConcreteMediator o-- Colleague1: has
    ConcreteMediator o-- Colleague2: has
    Colleague1 --> Colleague: extends
    Colleague2 --> Colleague: extends
    class Mediator {
        <<interface>>
        + Notify(sender : Colleague, event : String)
    }
    class Colleague {
        <<abstract>>
        - mediator : Mediator
        + SetMediator(mediator : Mediator)
    }
    class ConcreteMediator {
        - colleague1 : Colleague1
        - colleague2 : Colleague2
        + Notify(sender : Colleague, event : String)
    }
    class Colleague1 {
        + Operation1()
    }
    class Colleague2 {
        + Operation2()
    }
```
