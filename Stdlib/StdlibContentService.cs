namespace MewDocs.Stdlib;

using System.Collections.Immutable;
using MewDocs.Stdlib.Components;
using MewDocs.Stdlib.Model;
using Pennington.Content;
using Pennington.FrontMatter;
using Pennington.Pipeline;
using Pennington.Routing;

/// <summary>
/// Publishes one page per standard library namespace under <c>/stdlib/</c>, rendered by
/// <see cref="NamespacePage"/>. The pages join the "Standard Library" area's sidebar next to the
/// hand-written <c>Content/stdlib/index.md</c>, are indexed for search and llms.txt, and keep the
/// <c>stdlib.*</c> xref uids the language docs link to.
/// </summary>
public sealed class StdlibContentService : IContentService
{
    public const string AreaSlug = "stdlib";

    private readonly StdlibModel _model;

    public StdlibContentService(StdlibModel model)
    {
        _model = model;
    }

    public string DefaultSectionLabel => "Standard Library";

    public int SearchPriority => 5;

    public static ContentRoute RouteFor(StdlibNamespace ns) =>
        ContentRouteFactory.FromUrl(new UrlPath($"{StdlibHtmlRenderer.RoutePrefix}{ns.Slug}/"));

    public async IAsyncEnumerable<DiscoveredItem> DiscoverAsync()
    {
        var componentType = typeof(NamespacePage).FullName!;
        var order = 1;

        foreach (var ns in _model.Namespaces)
        {
            // Front matter makes the page a record like a Markdown page, so social cards,
            // structured data and search see a title and description.
            var metadata = new DocFrontMatter
            {
                Title = ns.Name,
                Description = StdlibHtmlRenderer.Describe(ns),
                Uid = ns.Uid,
                Order = order++,
                SectionLabel = DefaultSectionLabel,
            };

            yield return new DiscoveredItem(RouteFor(ns), new ContentSource(new RazorPageSource(componentType)))
            {
                Metadata = metadata,
            };
        }

        await Task.CompletedTask;
    }

    public Task<ImmutableList<ContentToCopy>> GetContentToCopyAsync() =>
        Task.FromResult(ImmutableList<ContentToCopy>.Empty);

    public Task<ImmutableList<ContentTocItem>> GetContentTocEntriesAsync()
    {
        var items = ImmutableList.CreateBuilder<ContentTocItem>();
        var order = 1;

        foreach (var ns in _model.Namespaces)
        {
            items.Add(new ContentTocItem(
                Title: ns.Name,
                Route: RouteFor(ns),
                Order: order++,
                HierarchyParts: [AreaSlug, ns.Slug],
                SectionLabel: DefaultSectionLabel,
                Locale: null)
            {
                Description = StdlibHtmlRenderer.Describe(ns),
            });
        }

        return Task.FromResult(items.ToImmutable());
    }

    public Task<ImmutableList<CrossReference>> GetCrossReferencesAsync() =>
        Task.FromResult(_model.Namespaces
            .Select(ns => new CrossReference(ns.Uid, ns.Name, RouteFor(ns)))
            .ToImmutableList());
}
