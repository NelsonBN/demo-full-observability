﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Notifications.Infrastructure.Grpc;

public static class Setup
{
    public static IServiceCollection AddGrpcServer(this IServiceCollection services)
    {
        services.AddGrpc(options =>
            options.EnableDetailedErrors = true);

        return services;
    }

    public static IApplicationBuilder UseGrpcServer(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints => endpoints.MapGrpcService<GrpcService>());

        return app;
    }
}
