# Introduction to gRPC using C#

In gRPC, a client application can directly call a method on a server application on a different machine as if it were a local object, making it easier to create distributed applications and services. gRPC uses [HTTP/2](https://developers.google.com/web/fundamentals/performance/http2) as its underlying transport protocol and is based around the idea of defining a service, specifying the methods that can be called remotely with their parameters and return types. On the server side, the server implements this interface and runs a gRPC server to handle client calls. On the client side, the client has a stub (or referred to as just 'client') that provides the same methods as the server.

## This project 
Contains two applications - 
- Server - A gRPC server application that demonstrates the below types of RPCs - 
    - [Unary RPC](https://grpc.io/docs/what-is-grpc/core-concepts/#unary-rpc)
    - [Server streaming RPC](https://grpc.io/docs/what-is-grpc/core-concepts/#server-streaming-rpc)
    - [Client streaming RPC](https://grpc.io/docs/what-is-grpc/core-concepts/#client-streaming-rpc)
    - [Bidirectional RPC](https://grpc.io/docs/what-is-grpc/core-concepts/#bidirectional-streaming-rpc)

- Client - A client console application to call any of the RPC endpoints on the server.

## Protocol Buffers and .NET tooling
gRPC uses [Protocol Buffers](https://developers.google.com/protocol-buffers/docs/overview) for serializing structured data, a contract-first approach to API development. Services and messages are defined in *.proto files. 

You can use the protocol buffer compiler called [```protoc```](https://grpc.io/docs/protoc-installation/) to generate data access classes as well as client and server code in your preferred language. ```protoc``` supports many programming languages including C# ([Supported languages](https://grpc.io/docs/languages/)).
But dotnet comes with all the necessary tooling that makes it even easier to generate C# code.

.NET types for services, clients, and messages are automatically generated by including *.proto files in a project.
- Add a package reference to [Grpc.Tools](https://www.nuget.org/packages/Grpc.Tools/)
- Add *.proto files to the ```<Protobuf>``` item group in the *.csproj file. 
    For example to generate client code, in client's ```.csproj``` file - 
    ```
    <ItemGroup>
        <Protobuf Include="Protos\grpc-demo.proto" GrpcServices="Client" />
    </ItemGroup>
    ```

    To generate server code, in server's ```.csproj``` file - 
    ```
    <ItemGroup>
        <Protobuf Include="Protos\grpc-demo.proto" GrpcServices="Server" />
    </ItemGroup>
    ```
gRPC services can be hosted on ASP.NET Core. Services have full integration with ASP.NET Core features such as logging, dependency injection (DI), authentication, and authorization.

## How to run it?
This project requires [dotnet](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to be installed in order to run it.
Run the server in a terminal window - 
```
dotnet run --project ./server
```

Run the client in another terminal window - 
```
dotnet run --project ./client
```

<img src=".img/gRPC_Demo.gif" height="600" />

## References
- [Introduction to HTTP/2](https://developers.google.com/web/fundamentals/performance/http2)
- [Introduction to gRPC](https://grpc.io/docs/what-is-grpc/introduction/)
- [Core concepts, architecture and lifecycle](https://grpc.io/docs/what-is-grpc/core-concepts/)
- [Introduction to gRPC on .NET](https://docs.microsoft.com/en-us/aspnet/core/grpc/?view=aspnetcore-6.0)