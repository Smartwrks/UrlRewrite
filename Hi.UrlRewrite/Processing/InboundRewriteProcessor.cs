using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing.Results;
using Sitecore.Data;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Hi.UrlRewrite.Processing
{
    public class InboundRewriteProcessor : HttpRequestProcessor
    {

        public override void Process(HttpRequestArgs args)
        {
            var db = Sitecore.Context.Database;

            try
            {
                //var context = HttpContext.Current;
                //if (context == null || db == null) return;

                //var httpContext = new HttpContextWrapper(context);
                //var requestUri = httpContext.Request.Url;

                //Log.Warn(this, db, "httpContext.Request.Url: {0}", httpContext.Request.Url);
                //Log.Warn(this, db, "httpContext.Request.Url.Host: {0}", httpContext.Request.Url.Host);
                //Log.Warn(this, db, "httpContext.Request.Url.Authority: {0}", httpContext.Request.Url.Authority);
                //Log.Warn(this, db, "HttpContext.Current.Request.Url: {0}", HttpContext.Current.Request.Url);

                if (args.HttpContext == null || db == null) return;

                //var httpContext = new HttpContextWrapper(args.Context);
                var httpContext = args.HttpContext;
                var requestUri = httpContext.Request.Url;

                if (requestUri == null || Configuration.IgnoreUrlPrefixes.Length > 0 && Configuration.IgnoreUrlPrefixes.Any(prefix => requestUri.PathAndQuery.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return;
                }
                
                var urlRewriter = new InboundRewriter(httpContext.Request.ServerVariables, httpContext.Request.Headers);
                var requestResult = ProcessUri(requestUri, db, urlRewriter);

                if (requestResult == null || !requestResult.MatchedAtLeastOneRule) return;

                httpContext.Items["urlrewrite:db"] = db.Name;
                httpContext.Items["urlrewrite:result"] = requestResult;

                var urlRewriterItem = Sitecore.Context.Database.GetItem(new ID(Constants.UrlRewriter_ItemId));

                Log.Warn(this, db, "urlRewritterItem {0}.", urlRewriterItem.Name);
                Log.Warn(this, db, "urlRewritterItem Uri ToString() {0}.", urlRewriterItem.Uri.ToString());
                Log.Warn(this, db, "urlRewritterItem {0}.", httpContext.User.Identity.Name);

                if (urlRewriterItem != null)
                {
                    Sitecore.Context.Item = urlRewriterItem;
                }
                else
                {
                    Log.Warn(this, db, "Unable to find UrlRewriter item {0}.", Constants.UrlRewriter_ItemId);
                }

                return;
            }
            catch (ThreadAbortException)
            {
                // swallow this exception because we may have called Response.End
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException) return;

                Log.Error(this, ex, db, "Exception occured");
            }
        }

        internal ProcessInboundRulesResult ProcessUri(Uri requestUri, Database db, InboundRewriter urlRewriter)
        {
            var inboundRules = GetInboundRules(db);

            if (inboundRules == null)
            {
                return null;
            }

            return urlRewriter.ProcessRequestUrl(requestUri, inboundRules);
        }

        private List<InboundRule> GetInboundRules(Database db)
        {
            var cache = RulesCacheManager.GetCache(db);
            var inboundRules = cache.GetInboundRules();

            if (inboundRules != null) return inboundRules;

            Log.Info(this, db, "Initializing Inbound Rules.");

            using (new SecurityDisabler())
            {
                var rulesEngine = new RulesEngine(db);
                inboundRules = rulesEngine.GetCachedInboundRules();
            }

            return inboundRules;
        }
    }
}
