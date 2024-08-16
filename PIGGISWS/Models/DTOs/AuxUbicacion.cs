namespace PIGGISWS.Models.Auxiliares;

public class AuxUbicacion
{
    public List<DataItem> Data { get; set; }
}

public class DataItem
{
    public Provincia? Provincia { get; set; }
    public List<CantonItem>? Cantones { get; set; }
}

public class CantonItem
{
    public Canton? Canton { get; set; }
    public List<Ubicacion>? Ubicaciones { get; set; }
}
