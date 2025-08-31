using System;
using System.Collections.Generic;
using System.Text;

namespace HeuristicAlgorithm.WPF.Utilities;
class ComboOption<T> {
    public string Name { get; set; }
    public T Value { get; set; }

    public ComboOption(string name, T value) {
        Name = name;
        Value = value;
    }
    public override string ToString() => Name;
}
