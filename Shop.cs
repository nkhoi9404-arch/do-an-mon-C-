namespace streetfood;

public class Shop
{
    public string Name { get; set; } = "";

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string DescriptionVI { get; set; } = "";

    public string DescriptionEN { get; set; } = "";

    public string BestSeller { get; set; } = "";
    public double Distance { get; set; }

    public bool Popular { get; set; }
}