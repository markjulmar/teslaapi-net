using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeslaCli.Commands;

namespace TeslaCli
{
    public class OutputDefinition<TItem>
    {
        const OutputStyle AllOutputStyles = OutputStyle.Csv | OutputStyle.JSON | OutputStyle.Table;
        private readonly IReadOnlyList<TItem> items;
        private readonly IList<IOutputValue> values = new List<IOutputValue>();

        public OutputDefinition(IEnumerable<TItem> items)
        {
            this.items = items.ToList();
        }

        public OutputDefinition(TItem item)
        {
            this.items = new List<TItem>() { item };
        }

        public void Add<T>(string header, Func<TItem, T> value, string suffix = "", OutputStyle showInStyles = AllOutputStyles)
        {
            values.Add(new OutputValue<T>(header, value, suffix, showInStyles));
        }

        public void Add(string header, string value, string suffix = "", OutputStyle showInStyles = AllOutputStyles)
        {
            values.Add(new OutputValue<string>(header, _ => value, suffix, showInStyles));
        }

        public void Render(OutputStyle outputStyle)
        {
            StringBuilder sb = new();

            if (outputStyle == OutputStyle.Text || outputStyle == 0)
            {
                int maxHeaderLen = values.Max(v => v.Header.Length);
                for (int i = 0; i < items.Count; i++)
                {
                    if (i > 0)
                        sb.AppendLine();

                    TItem item = items[i];
                    foreach (var value in values)
                    {
                        string header = value.Header;
                        header = new string(' ', maxHeaderLen - header.Length) + header;
                        sb.AppendLine($"{header}: {value.GetValue(item)}{value.Suffix}");
                    }
                }
            }
            else if (outputStyle == OutputStyle.Csv)
            {
                sb.AppendLine(string.Join(',', values.Select(v => $"\"{v.Header}\"")));
                foreach (var item in items)
                    sb.AppendLine(string.Join(',', values.Select(v => v.GetValue(item))));
            }
            else if (outputStyle == OutputStyle.Table)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (i > 0)
                        sb.AppendLine();

                    // Get the max size of the column
                    TItem item = items[i];
                    int[] headerLen = new int[values.Count];
                    for (int x = 0; x < values.Count; x++)
                        headerLen[x] = Math.Max(values[x].Header.Length, items.Max(it => values[x].GetValue(it).Length));

                    for (int x = 0; x < values.Count; x++)
                    {
                        if (values[x].ShowInStyles.HasFlag(outputStyle))
                        {
                            string header = values[x].Header;
                            header += new string(' ', headerLen[x] - header.Length);
                            sb.Append(header).Append(' ');
                        }
                    }
                    sb.AppendLine();

                    for (int x = 0; x < values.Count; x++)
                    {
                        if (values[x].ShowInStyles.HasFlag(outputStyle))
                        {
                            string sep = new('-', headerLen[x]);
                            sb.Append(sep).Append(' ');
                        }
                    }
                    sb.AppendLine();

                    for (int x = 0; x < values.Count; x++)
                    {
                        if (values[x].ShowInStyles.HasFlag(outputStyle))
                        {
                            string value = values[x].GetValue(item) + values[x].Suffix;
                            value += new string(' ', headerLen[x] - value.Length);
                            sb.Append(value).Append(' ');
                        }
                    }
                }
            }
            else if (outputStyle == OutputStyle.JSON)
            {
                sb.AppendLine(Utilities.LastJsonResponse);
            }

            Console.WriteLine(sb.ToString());
        }

        interface IOutputValue
        {
            public string Header { get; }
            public string Suffix { get; }
            public string GetValue(TItem item);
            public OutputStyle ShowInStyles { get; }
        }

        class OutputValue<T> : IOutputValue
        {
            public string Header { get; private set; }
            public string Suffix { get; }
            public OutputStyle ShowInStyles { get; private set; }
            readonly Func<TItem, T> getValue;

            public OutputValue(string header, Func<TItem, T> value, string suffix, OutputStyle showInStyles)
            {
                Header = header;
                getValue = value;
                Suffix = suffix;
                ShowInStyles = showInStyles;
            }

            public string GetValue(TItem item)
            {
                object obj = getValue.Invoke(item);
                string value = (obj??"").ToString();
                if (typeof(T) == typeof(bool))
                    value = (value == "False") ? "No" : "Yes";

                return value;
            }
        }
    }
}
