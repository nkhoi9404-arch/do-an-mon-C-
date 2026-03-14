using Microsoft.Maui.Devices.Sensors;
using System.Globalization;

namespace streetfood;

public partial class MainPage : ContentPage
{
    List<Shop> shops = new List<Shop>()
    {
        new Shop
        {
            Name = "Ốc Oanh",
            Latitude = 10.760725,
            Longitude = 106.703296,
            BestSeller = "Ốc rang muối",
            DescriptionVI = "Ốc Oanh là quán nổi tiếng tại phố ẩm thực Vĩnh Khánh. Món nổi bật là ốc rang muối.",
            DescriptionEN = "Oc Oanh is a famous seafood restaurant in Vinh Khanh street."
        },

        new Shop
        {
            Name = "Ốc Vũ",
            Latitude = 10.761394,
            Longitude = 106.702695,
            BestSeller = "Ốc tỏi nướng",
            DescriptionVI = "Ốc Vũ là quán ốc lâu đời tại quận 4.",
            DescriptionEN = "Oc Vu is a well-known seafood restaurant in district 4."
        },

        new Shop
        {
            Name = "Ốc Đào 2",
            Latitude = 10.761173,
            Longitude = 106.704948,
            BestSeller = "răng mực sào bơ tỏi",
            DescriptionVI = "Ốc Đào 2 phục vụ nhiều loại hải sản tươi ngon.",
            DescriptionEN = "Ốc Đào 2  serves many fresh seafood dishes."
        },

        new Shop
        {
            Name = "Ốc Nhi 20k",
            Latitude = 10.761283,
            Longitude = 106.705973,
            BestSeller = "Sò điệp nướng mỡ hành",
            DescriptionVI = "Ốc Sau Nổ nổi tiếng với món Sò điệp nướng mỡ hành.",
            DescriptionEN = "Oc Sau No is famous for salted egg sauce snails."
        },

        new Shop
        {
            Name = "Bé Ốc",
            Latitude = 10.7605,
            Longitude = 106.7005,
            BestSeller = "Sò nướng",
            DescriptionVI = "Bé Ốc là quán hải sản bình dân đông khách.",
            DescriptionEN = "Be Oc is a popular local seafood stall."
        }
    };

    public MainPage()
    {
        InitializeComponent();

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        if (status == PermissionStatus.Granted)
        {
            await CheckLocation();

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                _ = CheckLocation();
                return true;
            });
        }
        else
        {
            infoLabel.Text = "Khong co quyen GPS";
        }
    }

    async Task CheckLocation()
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.Best));

            if (location == null)
                return;

            foreach (var shop in shops)
            {
                var shopLocation = new Location(shop.Latitude, shop.Longitude);

                double distance = Location.CalculateDistance(
                    location,
                    shopLocation,
                    DistanceUnits.Kilometers);

                if (distance < 0.5)
                {
                    ShowShopInfo(shop);
                    return;
                }
            }

            infoLabel.Text = "Ban dang o ngoai khu pho am thuc";
        }
        catch (Exception ex)
        {
            infoLabel.Text = ex.Message;
        }
    }

    void ShowShopInfo(Shop shop)
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        if (lang == "vi")
        {
            infoLabel.Text = $"{shop.Name}\n{shop.DescriptionVI}\nBest seller: {shop.BestSeller}";
        }
        else
        {
            infoLabel.Text = $"{shop.Name}\n{shop.DescriptionEN}\nBest seller: {shop.BestSeller}";
        }
    }
}
