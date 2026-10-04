namespace BE
{
    public interface IComponentePerfil
    {
        int Id { get; set; }
        string Nombre { get; set; }
        string ObtenerDescripcion(); // Texto para el nodo del TreeView
    }
}
