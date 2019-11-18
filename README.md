# FFTHH: Actor Model using Akka.NET

## Lesson 0 - Create your first Actor (15m)

Learn how to setup an ASP.NET Core web app that runs an Akka.NET actor system.

## Requirements

[.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2)

[Visual Studio Code](https://code.visualstudio.com/Download)

An HTTP test tool your comfortable using (e.g. curl, Postman).



**Setup**

Clone or fork this git repository.

`https://github.com/Cobster/Axian.ActorModel.git`



This solution consists of 4 projects.

- **Axian.ActorModel**: In this main project you'll be working with Akka.NET
- **Axian.ActorModel.Tests**: This contains some predefined tests to verify things are running smoothly.
- **Axian.ActorModel.Website**: A simple ASP.NET Core Web API for some I/O.

**Install the Akka.NET nuget packages**

1. Open a shell and navigate to the solution directory.
2. Run the following commands to add the Akka libraries.

```
dotnet add src/Axian.ActorModel package Akka
dotnet add src/Axian.ActorModel.Tests package Akka.TestKit
```

**Create an ActorSystem**

Before we can create an Actor, we must first create an ActorSystem in which the Actor will live. 
An ActorSystem is a heavy-weight object, so we'll set it up as a singleton. To do this we will 
use some predefined code to provide a thin abstraction over the ActorSystem to provide a service
boundary. More on this later...

Create the actor system with a customizable name and configuration.

```c#
/* Axian.ActorModel/AxSystem.cs */

// TODO: Create an ActorSystem
ActorSystem actorSystem = ActorSystem.Create(name, config);
return new AxSystem(actorSystem);
```


Register the system as a singleton.

```c#
/* Axian.ActorModel.Website/Extensions/IServiceCollectionExtensions.cs */

// TODO: Register the ActorSystem as a singleton
services.AddSingleton(_ => AxSystem.Create(name, config));
```

Integrate the ActorSystem with ASP.NET Core.

```c#
/* Axian.ActorModel.Website/Startup.cs */

// TODO: Wire up the ActorSystem with ASP.NET Core
services.AddAxSystem("axian-ffthh-akka", "akka.conf");
```

---

<details>
<summary><strong>What's the conf file?</strong></summary>

Its a configuration file for the ActorSystem.

**What's the conf file format?**

HOCON

**What is HOCON?**

Human-Optimized Config Object Notation

**But I like JSON.**

Thats fine, JSON is HOCON. Use the provided akka.json file instead.

```c#
// TO DO: Wire up the ActorSystem with ASP.NET Core
services.AddAxSystem("axian-ffthh-akka", "akka.json");
```

**Neat.**

HOCON is not JSON, go [learn more about HOCON](https://github.com/lightbend/config/blob/master/HOCON.md#hocon-human-optimized-config-object-notation).

</details>

---

Next we need to wire up the ActorSystem lifecycle with the web app.

Since the ActorSystem is registered as a singleton, all we need to do is retrieve it from
the service collection to create the instance and thus startup the actor system.

```c#
/* Axian.ActorModel.Website/Extensions/IApplicationLifetimeExtensions.cs */

// TODO: Create an ActorSystem on start up
lifetime.ApplicationStarted.Register(() => container.GetService<AxSystem>());
```

We also need to handle shutdown,

```c#
/* Axian.ActorModel.Website/Extensions/IApplicationLifetimeExtensions.cs */

// TODO: Stop the ActorSystem when stopping
lifetime.ApplicationStopping.Register(() => container.GetService<AxSystem>().Terminate());
```

Let's get this system fired-up!

Set **Axian.ActorModel.Website** as the StartUp project, and fire it up in the debugger. 
This should also open `http://localhost:29426/api/sys` in a browser. If everything is working
you should see a response similiar to:

```json
{
  "name":"axian-ffthh-akka",
  "startTime":"2019-11-19T08:39:46.793+00:00",
  "uptime":"00:00:00.8529191"
}
```

<details>
<summary>
<strong>Why port 29426?</strong>
</summary>

Any valid port will work, but 29426 has significance. Can you figure it out why?
</details>

Okay, now that the ActorSystem is running, it's time to create our first actor. 
Create a class named `UntypedGreeter`, inherit from `Akka.Actor.UntypedActor`, and
override the `OnReceive` method.

```c#
/* Axian.ActorModel/UntypedGreeter.cs */

public class UntypedGreeter : Akka.Actor.UntypedActor
{
    protected override void OnReceive(object message) 
    {

    }
}
```

Whenever the Greeter receives a message, we want to respond to the sender with simple message.
Let's write a test for that.

```c#
/* Axian.ActorModel.Tests/GreeterSpecs.cs */

[Fact]
public void UntypedGreeter_should_always_respond_hello()
{
    // Arrange
    IActorRef greeter = Sys.ActorOf(Props.Create<UntypedGreeter>());

    // Act
    greeter.Tell("hi!");

    // Assert
    string response = ExpectMsg<string>();
    Assert.Equal("hello", response);
}
```

Then let's make it pass by overriding the `OnReceive` method and responding back to the sender.

```c#
/* Axian.ActorModel/UntypedGreeter.cs */

// TODO: Override OnReceive
protected override void OnReceive(object message) 
{
    Sender.Tell("hello");
}

```

Run the tests
```
dotnet test
```

<details>
<summary>
<strong>What is Sys.ActorOf?</strong>
</summary>

`Sys` in this test is the ActorSystem. `ActorOf` instructs the ActorSystem to create an instance  of the actor and return a reference to the new actor.  It's also responsible for assigning the  actor an address and setting up the actor's mailbox.

</details>

<details>
<summary>
<strong>What is Props.Create?</strong>
</summary>

An `Akka.Actor.Props` is essentially a recipe for an actor. The ActorSystem uses this to create instances whenever they're needed. 

Props are immutable and should be serializable. Thus they can easily be sent to other Actors in messages. This enables some very interesting creation design patterns.

</details>

<details>
<summary>
<strong>Why an IActorRef instead of the Greeter?</strong>
</summary>

Sorry, Akka does not let you talk directly to an Actor. The `IActorRef` is essentially the address to the Actor. And you use the `IActorRef` to either `Tell` or `Ask` the Actor something. Either way the message will land in the Actor's mailbox, and sometime later the actor will decide what to do with the message. 

This is important, because an Actor could throw an exception and be recreated by the ActorSystem. The new Actor instance will have the same address as the failed instance and thus the IActorRef can continue to be used for sending messages.

Or it's also possible that the IActorRef represents multiple Actors, like a [broadcast group or an elastic round robin pool](https://getakka.net/articles/actors/routers.html).

This also enables location transparency. Using the `Akka.Remote` or `Akka.Cluster` libraries makes it possible for an Actor to live on a different physical hosts. The IActorRef makes sending remote messages no different than sending local messages (but requires messages to be serializable).

An `IActorRef` is immutable and serializable. Making it easy to include in them in messages. 
This allows Actors to share addresses, and enables some interesting communication patterns.
</details>

**Grats! You've created your first actor.**


Now let's do the same thing using a ReceiveActor.

Create a new class named **ReceiveGreeter**.

```c#
/* Axian.ActorModel/ReceiveGreeter.cs */

public class ReceiveGreeter : Akka.Actor.ReceiveActor
{

}
```

Let's write another test.

```c#
/* Axian.ActorModel.Tests/GreeterSpecs.cs */

[Fact]
public void ReceiveGreeter_should_always_respond_hello()
{
    // Arrange
    IActorRef greeter = Sys.ActorOf(Props.Create<ReceiveGreeter>());

    // Act
    greeter.Tell("hi!");

    // Assert
    string response = ExpectMsg<string>();
    Assert.Equal("hello", response);
}
```

Then let's make it pass by defining a constructor which calls the `ReceiveAny` method, providing a lambda that responds to the sender.

```c#
/* Axian.ActorModel/ReceiveGreeter.cs */

// TODO: Override OnReceive
public ReceiveGreeter()
{
    ReceiveAny(msg => {
      Sender.Tell("hello");
    });
}

```

Run them tests.
```
dotnet test
```

Here are some other things to try out to continue exploring.

- custom responses based on the message value
- custom responses based on the message type
- send multiple responses
- store some state (e.g. message count)
- throw an exception
- don't handle the message


---


## Lesson 1: HttpRequest capture app (60m)

Let's create HTTP request capture app, a simplistic clone of [postb.in](https://postb.in).

**Requirements**

  - Users should be able to create a bin.
  - Each bin should have a endpoint for capturing HTTP requests.
  - Captures any HTTP request sent to the the bin endpoint.
    - Any HTTP verb
    - Captures verb, address, query string, headers, and body.
  - Stores a configurable number of the most recent requests
  - Lists all the stored requests
  - Bins expires after a configurable amount of time.

### Akka.NET topics covered

- Parent/Child Actors
- Scheduler
- Configuration


### API Design

**Create a bin**

`POST /bin`

**Store an HTTP request in a bin**

`VERB /bin/:binId`

**List bins**

`GET /bin`

**List HTTP requests stored in a bin**

`GET /logs/:binId`


### Actor Model Design

[Insert Design Image]


### Setup

Copy the lesson 1 tests from the finished branch.

















**AxSystem**

- Provides a name for the actor system if not provided.
- Provides a configuration for the actor system if not provided.

- Exposes service boundaries within the actor system to external callers.
- Plays nice with dependency injection.





When you create an child actor the parent actor, just like irl you are responsible for your child.


