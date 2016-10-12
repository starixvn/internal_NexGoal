namespace NexGoal.Site
{
    using BundleTransformer.Core.Orderers;
    using BundleTransformer.Core.Transformers;
    using System.Web.Optimization;

    public static class BundleEntensions
    {
        public static void AddAndInclude(this BundleCollection bundles, string bundleVirtualPath, params string[] virtualPaths)
        {
            if (bundles != null)
            {
                bundles.Add(new Bundle(bundleVirtualPath)
                {
                    Transforms = { new CssTransformer(), new CssMinify() },
                    Orderer = new NullOrderer()
                }.Include(virtualPaths));
            }
        }
    }
}