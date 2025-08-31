using System;
using System.Collections.Generic;
using System.Text;

namespace HeuristicAlgorithm.WPF.Utilities;
public class ComboOption<T>(string name, T value) {
    public string Name { get; set; } = name;
    public T Value { get; set; } = value;

    public override string ToString() => Name;
}
