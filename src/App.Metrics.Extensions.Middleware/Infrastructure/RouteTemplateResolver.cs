﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System.Linq;
using System.Threading.Tasks;

namespace App.Metrics.Extensions.Middleware.Infrastructure
{
    public class DefaultRouteTemplateResolver : IRouteNameResolver
    {
        public Task<string> ResolveMatchingTemplateRoute(RouteData routeData)
        {
            var templateRoute = routeData.Routers
                    .FirstOrDefault(r => r.GetType().Name == "Route")
                as Route;

            if (templateRoute != null)
            {
                var controller = routeData.Values.FirstOrDefault(v => v.Key == "controller");
                var action = routeData.Values.FirstOrDefault(v => v.Key == "action");

                var result = templateRoute.ToTemplateString(controller.Value as string, action.Value as string).ToLower();

                return Task.FromResult(result);
            }

            var attributeRouteHandler = routeData.Routers
                    .FirstOrDefault(r => r.GetType().Name == "MvcAttributeRouteHandler")
                as MvcAttributeRouteHandler;

            if (attributeRouteHandler != null)
            {
                var actionDescriptor = attributeRouteHandler.Actions.FirstOrDefault();
                var result = actionDescriptor?.AttributeRouteInfo?.Template.ToLower() ?? string.Empty;

                return Task.FromResult(result);
            }

            return Task.FromResult(string.Empty);
        }
    }
}