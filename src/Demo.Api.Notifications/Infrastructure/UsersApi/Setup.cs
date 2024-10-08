﻿using System;
using Api.Notifications.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Notifications.Infrastructure.UsersApi;

public static class Setup
{
    public static IServiceCollection AddUsersApi(this IServiceCollection services)
    {
        services
            .AddHttpClient<IUsersService, UsersService>((sp, client) =>
            {
                var url = sp.GetRequiredService<IConfiguration>()["UsersApi"]!;
                client.BaseAddress = new Uri(url);
            });

        return services;
    }
}
