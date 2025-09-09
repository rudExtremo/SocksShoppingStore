using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SocksShoppingStore.Tests.TestDoubles
{
    public class TestUrlHelper : IUrlHelper
    {
        public ActionContext ActionContext { get; }
        public TestUrlHelper(ActionContext ctx) { ActionContext = ctx; }
        public string? Action(UrlActionContext actionContext) => "/";
        public string? Content(string? contentPath) => contentPath;
        public bool IsLocalUrl(string? url) => !string.IsNullOrWhiteSpace(url) && url.StartsWith("/");
        public string? Link(string? routeName, object? values) => "/";
        public string? RouteUrl(UrlRouteContext routeContext) => "/";
        public string? Action(string? action, string? controller, object? values, string? protocol, string? host, string? fragment) => "/";
    }
}
