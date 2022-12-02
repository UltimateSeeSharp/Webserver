#pragma warning disable 8714

using Autofac;
using Autofac.Core;
using Webserver.Extentions;

namespace Webserver;

internal static class Bootstrapper
{
    private static IContainer? _container;

    internal static T Resolve<T>() => _container!.Resolve<T>();

    public static async Task Start() => await Task.Run(async () =>
    {
        ContainerBuilder builder = new();

        builder.RegisterType<StreamService>();
        builder.RegisterType<HeaderService>();

        _container = builder.Build();
    });

    public static async Task Stop() => await _container!.DisposeAsync();
}