using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contrib.Avro.Codegen;

internal sealed class ActionDisposable(Action action) : IDisposable
{
    public void Dispose()
    {
        action();
    }
}

internal class IndentedStringBuilder(int indentationSize = 4, int initialIndentationLevel = 0)
{
    private readonly StringBuilder _stringBuilder = new();
    private int _indentationLevel = initialIndentationLevel;

    private bool _isAtLineStart = true;

    private Queue<IDisposable> _blocks = new();

    public static IndentedStringBuilder New(int indentationSize = 4, int initialIndentationLevel = 0) =>
        new(indentationSize, initialIndentationLevel);

    public IndentedStringBuilder AppendLine(string value)
    {
        AppendValue(value).AppendLine();
        _isAtLineStart = true;
        return this;
    }

    public IndentedStringBuilder Append(string value)
    {
        AppendValue(value);
        _isAtLineStart = value.EndsWith("\n");
        return this;
    }

    public IndentedStringBuilder AppendLine()
    {
        _stringBuilder.AppendLine();
        _isAtLineStart = true;
        return this;
    }

    public IndentedStringBuilder AppendMany<T>(IEnumerable<T> values, Func<T, string> toLine) =>
        values.Aggregate(this, (builder, value) => builder.AppendLine(toLine(value)));

    private StringBuilder AppendValue(string value)
    {
        if (string.IsNullOrEmpty(value)) return _stringBuilder;
        if (_isAtLineStart) _stringBuilder.Append(' ', _indentationLevel * indentationSize);

        var lines = value.Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            if (i == 0)
            {
                _stringBuilder.Append(lines[i]);
            }
            else
            {
                _stringBuilder
                    .AppendLine()
                    .Append(' ', _indentationLevel * indentationSize)
                    .Append(lines[i]);
            }
        }

        return _stringBuilder;
    }

    public IDisposable IndentBlock(string? blockStart = null, string? blockEnd = null)
    {
        if (blockStart is not null) AppendLine(blockStart);
        IncreaseIndentation();
        return new ActionDisposable(() =>
        {
            DecreaseIndentation();
            if (blockEnd is not null) AppendLine(blockEnd);
        });
    }

    public IndentedStringBuilder StartBlock(string? blockStart = null, string? blockEnd = null)
    {
        if (blockStart is not null) AppendLine(blockStart);
        IncreaseIndentation();
        _blocks.Enqueue(new ActionDisposable(() =>
        {
            DecreaseIndentation();
            if (blockEnd is not null) AppendLine(blockEnd);
        }));
        return this;
    }

    public IndentedStringBuilder EndBlock()
    {
        if (_blocks.Count > 0) _blocks.Dequeue().Dispose();
        return this;
    }

    public IndentedStringBuilder EndAllBlocks()
    {
        while (_blocks.Count > 0) _blocks.Dequeue().Dispose();
        return this;
    }

    public IndentedStringBuilder IncreaseIndentation()
    {
        _indentationLevel++;
        return this;
    }

    public IndentedStringBuilder DecreaseIndentation()
    {
        if (_indentationLevel > 0) _indentationLevel--;
        return this;
    }

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }
}
