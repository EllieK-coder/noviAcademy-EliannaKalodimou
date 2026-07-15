using Autofac;
using NoviCode.Commands;
using NoviCode.Commands.Players;
using NoviCode.Decorators;

namespace NoviCode;

public class ApplicationModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // ---- Raw handlers (business logic only) ----
        builder.RegisterType<CreateWalletHandler>().As<ICommandHandler<CreateWalletCommand, Wallet?>>();
        builder.RegisterType<DepositHandler>().As<ICommandHandler<DepositCommand, Wallet?>>();
        builder.RegisterType<ApplyFundsHandler>().As<ICommandHandler<ApplyFundsCommand, Wallet?>>();
        builder.RegisterType<SetWalletBlockedHandler>().As<ICommandHandler<SetWalletBlockedCommand, Wallet?>>();
        builder.RegisterType<CreatePlayerHandler>().As<ICommandHandler<CreatePlayerCommand, Player?>>();

        builder.RegisterType<GetWalletByIdHandler>().As<IQueryHandler<GetWalletByIdQuery, Wallet?>>();
        builder.RegisterType<GetWalletsByPlayerHandler>().As<IQueryHandler<GetWalletsByPlayerQuery, IReadOnlyList<Wallet>>>();
        builder.RegisterType<GetAllWalletsHandler>().As<IQueryHandler<GetAllWalletsQuery, IReadOnlyList<Wallet>>>();
        builder.RegisterType<GetPlayerByIdHandler>().As<IQueryHandler<GetPlayerByIdQuery, Player?>>();
        builder.RegisterType<GetAllPlayersHandler>().As<IQueryHandler<GetAllPlayersQuery, IReadOnlyList<Player>>>();

        // ---- Query decorators: caching (inner) then logging (outer) ----
        builder.RegisterGenericDecorator(typeof(CachingQueryHandlerDecorator<,>), typeof(IQueryHandler<,>));
        builder.RegisterGenericDecorator(typeof(LoggingQueryHandlerDecorator<,>), typeof(IQueryHandler<,>));

        // ---- Command decorators: write-through (inner) then logging (outer) ----
        builder.RegisterGenericDecorator(typeof(WalletCacheWriteThroughDecorator<>), typeof(ICommandHandler<,>));
        builder.RegisterGenericDecorator(typeof(PlayerCacheWriteThroughDecorator<>), typeof(ICommandHandler<,>));
        builder.RegisterGenericDecorator(typeof(LoggingCommandHandlerDecorator<,>), typeof(ICommandHandler<,>));

        // ---- Dispatcher ----
        builder.RegisterType<Dispatcher>().As<IDispatcher>();
    }
}

using MediatR;
using NoviCode.Decorators;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoviCode
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Keep application registrations here. MediatR is registered through IServiceCollection in Program.cs.
        }
    }
}
