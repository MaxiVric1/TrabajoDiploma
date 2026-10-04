using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TpIngSoft.Traduccion
{
    public class MenuStripTraducible : IControlTraducible
    {
        public string NombreControl => _nombreControl;
        private readonly string _nombreControl;
        private readonly List<(ToolStripItem Item, string Key)> _items = new List<(ToolStripItem, string)>();

        public MenuStripTraducible(string nombreControl)
        {
            _nombreControl = nombreControl;
        }

        public MenuStripTraducible ConItem(ToolStripItem item, string key)
        {
            if (item != null)
            {
                _items.Add((item, key));
            }
            return this;
        }

        public void Traducir(Dictionary<string, string> traducciones)
        {
            foreach (var mapping in _items)
            {
                if (traducciones.TryGetValue(mapping.Key, out string texto) && !string.IsNullOrWhiteSpace(texto))
                {
                    mapping.Item.Text = texto;
                }
                else
                {
                    mapping.Item.Text = $"<{mapping.Key}>";
                }
            }
        }
    }
}
