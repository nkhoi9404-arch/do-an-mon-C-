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
            Latitude = 10.7601,
            Longitude = 106.7001,
            BestSeller = "Ốc rang muối",
            DescriptionVI = "Ốc Oanh là quán nổi tiếng tại phố ẩm thực Vĩnh Khánh. Món nổi bật là ốc rang muối.",
            DescriptionEN = "Oc Oanh is a famous seafood restaurant in Vinh Khanh street. Best seller is salt roasted snails."
        },

        new Shop
        {
            Name = "Ốc Vũ",
            Latitude = 10.7602,
            Longitude = 106.7002,
            BestSeller = "Ốc tỏi nướng",
            DescriptionVI = "Ốc Vũ là quán ốc lâu đời tại quận 4.",
            DescriptionEN = "Oc Vu is a well-known seafood restaurant in district 4."
        },

        new Shop
        {
            Name = "Thảo Ốc",
            Latitude = 10.7603,
            Longitude = 106.7003,
            BestSeller = "Nghêu hấp",
            DescriptionVI = "Thảo Ốc phục vụ nhiều loại hải sản tươi ngon.",
            DescriptionEN = "Thao Oc serves many fresh seafood dishes."
        },

        new Shop
        {
            Name = "Ốc Sau Nổ",
            Latitude = 10.7604,
            Longitude = 106.7004,
            BestSeller = "Ốc sốt trứng muối",
            DescriptionVI = "Ốc Sau Nổ nổi tiếng với món ốc sốt trứng muối.",
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
        }
        else
        {
            infoLabel.Text = "Không có quyền GPS";
        }
    }

    async Task CheckLocation()
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.Best));
            await DisplayAlert("GPS", $"{location.Latitude} , {location.Longitude}", "OK");
            if (location == null)
                return;

            foreach (var shop in shops)
            {
                var shopLocation = new Location(shop.Latitude, shop.Longitude);

                double distance = Location.CalculateDistance(
                    location,
                    shopLocation,
                    DistanceUnits.Kilometers);

                if (distance < 1) // 50m
                {
                    ShowShopInfo(shop);
                    return;
                }
            }

            infoLabel.Text = "Bạn đang ở ngoài khu phố ẩm thực.";
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