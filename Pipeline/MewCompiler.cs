using System.Reflection;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using Mew;
using Mew.Serialization;

namespace MewDocs.Pipeline;

/// <summary>
/// Drives the Mew compiler library while the site renders: parses a program against the
/// embedded standard library, serializes every intermediate stage, emits an assembly to
/// memory, and can disassemble and decompile what came out.
/// </summary>
public sealed class MewCompiler
{
    private static readonly Assembly Self = typeof(MewCompiler).Assembly;

    private readonly Lock _prepareLock = new();
    private List<SyntaxTree>? _standardLibrary;
    private Dictionary<string, byte[]>? _references;

    /// <summary>
    /// Parses the standard library once and makes sure the compiler can find the reference
    /// assemblies it needs. The compiler builds its type index lazily from the
    /// trusted-platform-assembly list, so that list has to be right before the first
    /// compilation touches it. The runtime's own list already covers the framework; only
    /// Mew.Runtime is appended when it is missing.
    /// </summary>
    public void Prepare()
    {
        if (_standardLibrary != null)
        {
            return;
        }

        lock (_prepareLock)
        {
            if (_standardLibrary != null)
            {
                return;
            }

            var references = new Dictionary<string, byte[]>(StringComparer.Ordinal);
            foreach (var name in Self.GetManifestResourceNames().Where(n => n.StartsWith("refs/", StringComparison.Ordinal)))
            {
                references[Path.GetFileNameWithoutExtension(name)] = ReadResource(name);
            }

            EnsureRuntimeIsTrusted(references["Mew.Runtime"]);

            _references = references;
            _standardLibrary = Self.GetManifestResourceNames()
                .Where(n => n.StartsWith("stdlib/", StringComparison.Ordinal))
                .OrderBy(n => n, StringComparer.Ordinal)
                .Select(n => SyntaxTree.Parse(
                    ReadText(n),
                    entryPoint: false,
                    path: StandardLibrary.SourceRoot + "/" + n["stdlib/".Length..].Replace(Path.DirectorySeparatorChar, '/'),
                    standardLibrary: true))
                .ToList();
        }
    }

    /// <summary>
    /// The trusted-platform-assembly list only has Mew.Runtime in it when the host's
    /// dependency manifest lists it. If it does not, the embedded copy is written to a temp
    /// folder and appended, which the runtime ignores but the compiler's type index reads.
    /// </summary>
    private static void EnsureRuntimeIsTrusted(byte[] runtime)
    {
        var trusted = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? "";
        var listed = trusted
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Any(p => string.Equals(Path.GetFileNameWithoutExtension(p), "Mew.Runtime", StringComparison.OrdinalIgnoreCase));
        if (listed)
        {
            return;
        }

        var folder = Path.Combine(Path.GetTempPath(), "mew-docs", "refs");
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, "Mew.Runtime.dll");
        File.WriteAllBytes(path, runtime);
        AppContext.SetData("TRUSTED_PLATFORM_ASSEMBLIES", trusted.Length == 0 ? path : trusted + Path.PathSeparator + path);
    }

    public CompileResult Compile(string source)
    {
        Prepare();

        var main = SyntaxTree.Parse(source, entryPoint: true, path: "main.mew");
        var compilation = Compilation.Create("main")
            .AddSyntaxTrees(_standardLibrary!)
            .AddSyntaxTree(main)
            .WithStandardLibraryReference(StandardLibrary.Identity);

        var diagnostics = compilation.GetDiagnostics();
        var syntax = SyntaxSerializer.Serialize(main, showSource: false);
        var hir = HirSerializer.Serialize(compilation.ToHir());
        var mir = MirSerializer.Serialize(compilation.ToMir());

        if (diagnostics.HasErrors)
        {
            return new CompileResult(diagnostics, syntax, hir, mir, null, null);
        }

        var lir = LirSerializer.Serialize(compilation.ToLir());
        var emitted = Compiler.Emit(compilation);
        return emitted.Success
            ? new CompileResult(emitted.Diagnostics, syntax, hir, mir, lir, emitted.Assembly)
            : new CompileResult(emitted.Diagnostics, syntax, hir, mir, lir, null);
    }

    public string Disassemble(byte[] assembly)
    {
        var file = new PEFile("main", new MemoryStream(assembly), PEStreamOptions.PrefetchEntireImage);
        var output = new PlainTextOutput();
        new ReflectionDisassembler(output, CancellationToken.None).WriteModuleContents(file);
        return output.ToString();
    }

    public string Decompile(byte[] assembly)
    {
        var file = new PEFile("main", new MemoryStream(assembly), PEStreamOptions.PrefetchEntireImage);
        // The newest C# the decompiler knows, with its simplifications on: dead code and
        // stores dropped and temporaries inlined, so the result reads like source rather
        // than a transcript of the IL.
        var settings = new DecompilerSettings(LanguageVersion.Latest)
        {
            RemoveDeadCode = true,
            RemoveDeadStores = true,
            AggressiveInlining = true,
            ShowXmlDocumentation = false,
            // The casts that pick an overload stay, but without the ? annotations noise.
            NullableReferenceTypes = false,
        };
        var decompiler = new CSharpDecompiler(file, new MemoryResolver(_references!), settings);

        // Only the program's own types: the whole-module view also prints assembly attributes
        // and the empty <Module> class, which say nothing about the Mew that was compiled.
        var metadata = file.Metadata;
        var types = metadata.TypeDefinitions.Where(handle =>
        {
            var type = metadata.GetTypeDefinition(handle);
            return !type.GetDeclaringType().IsNil ? false : metadata.GetString(type.Name) != "<Module>";
        });
        return decompiler.DecompileTypesAsString(types).Trim();
    }

    private static byte[] ReadResource(string name)
    {
        using var stream = Self.GetManifestResourceStream(name)!;
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }

    private static string ReadText(string name)
    {
        using var reader = new StreamReader(Self.GetManifestResourceStream(name)!);
        return reader.ReadToEnd();
    }

    /// <summary>Serves the decompiler the embedded reference assemblies instead of a disk.</summary>
    private sealed class MemoryResolver(Dictionary<string, byte[]> references) : IAssemblyResolver
    {
        private readonly Dictionary<string, MetadataFile> _opened = new(StringComparer.Ordinal);

        public MetadataFile? Resolve(IAssemblyReference reference)
        {
            if (_opened.TryGetValue(reference.Name, out var opened))
            {
                return opened;
            }

            if (!references.TryGetValue(reference.Name, out var bytes))
            {
                return null;
            }

            return _opened[reference.Name] = new PEFile(reference.Name, new MemoryStream(bytes), PEStreamOptions.PrefetchEntireImage);
        }

        public MetadataFile? ResolveModule(MetadataFile mainModule, string moduleName) => null;

        public Task<MetadataFile?> ResolveAsync(IAssemblyReference reference) => Task.FromResult(Resolve(reference));

        public Task<MetadataFile?> ResolveModuleAsync(MetadataFile mainModule, string moduleName) => Task.FromResult<MetadataFile?>(null);

        public IDisposable? BeginSnapshot() => null;
    }
}

public sealed record CompileResult(
    Diagnostics Diagnostics,
    string Syntax,
    string Hir,
    string Mir,
    string? Lir,
    byte[]? Assembly)
{
    public bool HasErrors => Diagnostics.HasErrors;

    /// <summary>The diagnostics as one line each, for hosts that cannot see the Mew types.</summary>
    public string DiagnosticsText =>
        string.Join("\n", Diagnostics.Select(d => $"{d.Severity.ToString().ToLowerInvariant()} {d.Code}: {d.Message}"));
}
