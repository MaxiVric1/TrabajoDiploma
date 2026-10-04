using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TpIngSoft.Traduccion
{
    public class DataGridTraducible : IControlTraducible
    {
        public string NombreControl => _nombreControl;
        private readonly DataGridView _dataGridView;
        private readonly string _nombreControl;
        private readonly List<(string ColumnName, string Key)> _columnas = new List<(string, string)>();

        public DataGridTraducible(DataGridView dataGridView, string nombreControl)
        {
            _dataGridView = dataGridView ?? throw new ArgumentNullException(nameof(dataGridView));
            _nombreControl = nombreControl;
        }

        public DataGridTraducible ConColumna(string columnName, string key)
        {
            _columnas.Add((columnName, key));
            return this;
        }

        public void Traducir(Dictionary<string, string> traducciones)
        {
            foreach (var col in _columnas)
            {
                if (_dataGridView.Columns.Contains(col.ColumnName))
                {
                    var gridCol = _dataGridView.Columns[col.ColumnName];
                    if (traducciones.TryGetValue(col.Key, out string texto) && !string.IsNullOrWhiteSpace(texto))
                    {
                        gridCol.HeaderText = texto;
                    }
                    else
                    {
                        gridCol.HeaderText = $"<{col.Key}>";
                    }
                }
            }
        }
    }
}
