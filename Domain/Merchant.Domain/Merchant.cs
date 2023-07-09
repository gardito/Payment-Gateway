namespace Domain;

public class Merchant
{
    public int Id { get; }
    public string Name { get; }

    public Merchant(int id, string name)
    {
        Id = id;
        Name = name;
    }
}