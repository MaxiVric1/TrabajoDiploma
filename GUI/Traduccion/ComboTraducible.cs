using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TpIngSoft.Traduccion
{
    public class ComboItemTraducible
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Texto { get; set; }

        public override string ToString()
        {
            return Texto;
        }
    }

    public class ComboTraducible : IControlTraducible
    {
        public string NombreControl => _nombreControl;
        private readonly ComboBox _comboBox;
        private readonly string _nombreControl;
        private readonly List<(string Key, object Value, string Fallback)> _items = new List<(string, object, string)>();

        public ComboTraducible(ComboBox comboBox, string nombreControl)
        {
            _comboBox = comboBox ?? throw new ArgumentNullException(nameof(comboBox));
            _nombreControl = nombreControl;
        }

        public ComboTraducible ConItem(string key, object value, string fallback)
        {
            _items.Add((key, value, fallback));
            return this;
        }

        public void Traducir(Dictionary<string, string> traducciones)
        {
            int selectedIndex = _comboBox.SelectedIndex;
            _comboBox.Items.Clear();

            foreach (var item in _items)
            {
                string texto = $"<{item.Key}>";
                if (traducciones.TryGetValue(item.Key, out string traducido))
                {
                    texto = traducido;
                }
                
                _comboBox.Items.Add(new ComboItemTraducible
                {
                    Key = item.Key,
                    Value = item.Value,
                    Texto = texto
                });
            }

            if (selectedIndex >= 0 && selectedIndex < _comboBox.Items.Count)
            {
                _comboBox.SelectedIndex = selectedIndex;
            }
            else if (_comboBox.Items.Count > 0)
            {
                _comboBox.SelectedIndex = 0;
            }
        }
    }
}
