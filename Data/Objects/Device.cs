namespace ALViN.Data.Objects;

public class Device
{
    public int Id {get;set;}
    public string? Name {get; set;} 
    public Guid LastUuid {get; set;}
    public DateTime LastDetected {get; set;}
    public Device(){

    }
}