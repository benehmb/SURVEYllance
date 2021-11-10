using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace SURVEYllance.Resources
{
    //TODO: Add documentation
    public static class EndpointExtensions
    {
        /// <summary>
        /// Redirect all requests to the given path to the given endpoint.
        /// accepts routes with parameters, if they have EXACTLY the same naming and order in <paramref name="from"/> and <paramref name="to"/>
        /// Example:
        /// <code>endpoints.Redirect("/test/{JoinId?}", "/test.html?id={JoinId?}");</code>
        /// If you call 'example.com/test/5', it will be redirected to 'example.com/test.html?id=5'
        /// </summary>
        /// <param name="endpoints">Route-Builder to apply route on</param>
        /// <param name="from">Route which will be redirected from</param>
        /// <param name="to">Route which will be redirected to</param>
        /// <returns></returns>
        public static IEndpointRouteBuilder Redirect(
            this IEndpointRouteBuilder endpoints,
            string from, string to)
        {
            return Redirect(endpoints,
                new Redirective(from, to));
        }

        public static IEndpointRouteBuilder RedirectPermanent(
            this IEndpointRouteBuilder endpoints,
            string from, string to)
        {
            return Redirect(endpoints,
                new Redirective(from, to, true));
        }
        
        public static IEndpointRouteBuilder Redirect(
            this IEndpointRouteBuilder endpoints,
            params Redirective[] paths
        )
        {
            foreach (var (from, to, permanent) in paths)
            {
                endpoints.MapGet(from, async http =>
                {
                    string toReplaced = to;
                    Regex rx = new Regex(@"\{(.*?)\}", RegexOptions.Compiled);
                    MatchCollection matchesFrom = rx.Matches(from);
                    MatchCollection matchesTo = rx.Matches(to);
                    
                    // Check if both matches are the same
                    bool same = matchesFrom.Count == matchesTo.Count;
                    for (var i = 0; i < matchesFrom.Count && same; i++)
                    {
                        if (!matchesFrom[i].Value.Equals(matchesTo[i].Value))
                            same=false;
                    }
                    
                    // If they are the same, we can replace the parameters
                    if (same)
                    {
                        for (var i = 0; i < matchesTo.Count; i++)
                        {
                            // Replace every occurrence of routing parameters from the from-route with the corresponding values from the Route-Values
                            string matchWithoutAdditions = matchesTo[i].Value;
                            matchWithoutAdditions = matchWithoutAdditions.Replace("{", "").Replace("}", "");
                            matchWithoutAdditions = matchWithoutAdditions.Replace("?", "");
                            toReplaced = toReplaced.Replace(
                                matchesTo[i].Value,
                                http.Request.RouteValues[matchWithoutAdditions] as string);
                        }
                    }
                    http.Response.Redirect(toReplaced, permanent);
                });
            }

            return endpoints;
        }
    }
    
    public record Redirective(string From, string To, bool Permanent = false);
}