using Chispa.Core.Abstractions;

namespace Chispa.Core.Registry;

public sealed class ToolRegistry
{
    private readonly Dictionary<string, ITool> _tools =
        new(StringComparer.Ordinal);

    public IReadOnlyCollection<ITool> All => _tools.Values;

    public int Count => _tools.Count;

    public void Register(ITool tool)
    {
        ArgumentNullException.ThrowIfNull(tool);

        string name = tool.Definition.Name;

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "A tool must have a name.",
                nameof(tool));
        }

        if (!_tools.TryAdd(name, tool))
        {
            throw new InvalidOperationException(
                $"A tool named '{name}' is already registered.");
        }
    }

    public bool TryGet(string name, out ITool? tool)
    {
        return _tools.TryGetValue(name, out tool);
    }
}