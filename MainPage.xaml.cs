using Microsoft.Maui.Devices.Sensors;
using System.Globalization;
using Microsoft.Maui.Media;
using System.Threading;

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
            BestSeller = "ốc hương sốt trứng muối",
            DescriptionVI = "Ốc Oanh là quán nổi tiếng tại phố ẩm thực Vĩnh Khánh. Món nổi bật là ốc rang muối.",
            DescriptionEN = "Oc Oanh is the most powerful name on Vinh Khanh Street..."
        },

        new Shop
        {
            Name = "Ốc Vũ",
            Latitude = 10.761394,
            Longitude = 106.702695,
            BestSeller = "Ốc tỏi nướng",
            DescriptionVI = "Ốc Vũ là quán ốc lâu đời tại quận 4.",
            DescriptionEN = "Oc Vu is located right at the beginning...",
            DescriptionZH = "Oc Vu 是一家著名的海鲜餐厅。"
        },

        new Shop
        {
            Name = "Ốc Nhi 20k",
            Latitude = 10.761283,
            Longitude = 106.705973,
            BestSeller = "Sò điệp nướng mỡ hành",
            DescriptionVI = "Ốc Nhi 20k nổi tiếng với món Sò điệp nướng mỡ hành.",
            DescriptionEN = "Oc Nhi 20k is a prime representative...",
            Popular = true
        },

        new Shop
        {
            Name = "Ốc su 20k",
            Latitude = 10.760324,
            Longitude = 106.707330,
            BestSeller = "Sò nướng",
            DescriptionVI = "Bé Ốc là quán hải sản bình dân đông khách.",
            DescriptionEN = "Oc Su 20k is also an ideal stop...",
            Popular = true
        }
    };

    Shop? currentShop = null;
    string currentLanguage = "vi";

    // 🔥 dùng để STOP giọng
    CancellationTokenSource? cts;

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
            infoLabel.Text = "Không có quyền GPS";
        }
    }

    async Task CheckLocation()
    {
        try
        {
            popularList.IsVisible = false;

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

                shop.Distance = distance * 1000;

                if (distance < 0.05)
                {
                    if (currentShop != shop)
                    {
                        currentShop = shop;
                        ShowShopInfo(shop);
                    }
                    return;
                }
            }

            // ngoài khu vực → hiện quán hot
            var popularShops = shops
                .Where(s => s.Popular)
                .OrderBy(s => s.Distance)
                .ToList();

            popularList.ItemsSource = popularShops;
            popularList.IsVisible = true;

            currentShop = null;
            replayButton.IsVisible = false;
            stopButton.IsVisible = false;

            infoLabel.Text = "Bạn đang ở ngoài khu phố ẩm thực";
        }
        catch (Exception ex)
        {
            infoLabel.Text = ex.Message;
        }
    }

    async void ShowShopInfo(Shop shop)
    {
        string text = GetText(shop);

        infoLabel.Text = text;

        replayButton.IsVisible = false;
        stopButton.IsVisible = true;

        await SpeakText(text);

        replayButton.IsVisible = true;
        stopButton.IsVisible = false;
    }

    string GetText(Shop shop)
    {
        if (currentLanguage == "vi")
            return $"{shop.Name}\n{shop.DescriptionVI}\nMón bán chạy: {shop.BestSeller}";

        if (currentLanguage == "en")
            return $"{shop.Name}\n{shop.DescriptionEN}\nBest seller: {shop.BestSeller}";

        return $"{shop.Name}\n{(shop.DescriptionZH ?? shop.DescriptionEN)}\n招牌菜: {shop.BestSeller}";
    }

    async Task SpeakText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        cts?.Cancel(); // stop cái cũ
        cts = new CancellationTokenSource();

        await TextToSpeech.Default.SpeakAsync(text, cancelToken: cts.Token);
    }

    void OnStopClicked(object sender, EventArgs e)
    {
        cts?.Cancel();

        stopButton.IsVisible = false;
        replayButton.IsVisible = true;
    }

    async void OnReplayClicked(object sender, EventArgs e)
    {
        if (currentShop != null)
        {
            await SpeakText(GetText(currentShop));
        }
    }

    async void OnChooseLanguage(object sender, EventArgs e)
    {
        string action = await DisplayActionSheet(
            "Chọn ngôn ngữ",
            "Hủy",
            null,
            "Tiếng Việt",
            "English",
            "中文");

        if (action == "Tiếng Việt")
            currentLanguage = "vi";
        else if (action == "English")
            currentLanguage = "en";
        else if (action == "中文")
            currentLanguage = "zh";

        if (currentShop != null)
        {
            ShowShopInfo(currentShop);
        }
    }
}