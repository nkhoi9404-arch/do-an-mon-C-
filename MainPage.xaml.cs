using Microsoft.Maui.Devices.Sensors;
using System.Globalization;
using Microsoft.Maui.Media;

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
            DescriptionEN = "Oc Oanh is the most powerful name on Vinh Khanh Street, famous for its massive space spanning both sides of the road that is always packed with tourists and locals from late afternoon until midnight." +
            "The restaurant is loved for its premium, large, and firm snails combined with a bold, salty-sweet-spicy seasoning style typical of Saigon street food, such as the signature salted egg yolk sauce or snow-salted crab claws." +
            "Oc Oanh remains the top choice for those seeking a high-quality ( worth every penny ) culinary experience that has been recommended by the Michelin Guide."
        },

        new Shop
        {
            Name = "Ốc Vũ",
            Latitude = 10.761394,
            Longitude = 106.702695,
            BestSeller = "Ốc tỏi nướng",
            DescriptionVI = "Ốc Vũ là quán ốc lâu đời tại quận 4.",
            DescriptionEN = "Oc Vu is located right at the beginning of Vinh Khanh Street near Hoang Dieu, serving as a national favorite destination for young people and drinking groups thanks to its perfect balance of food quality and reasonable prices." +
            "The biggest attraction is the fresh seafood display right at the storefront where customers can pick their favorites, along with a diverse menu of dozens of types of snails, shrimp, crabs, and flower crabs served incredibly fast by an energetic staff." +
            "Signature dishes like stir-fried morning glory with razor clams or grilled scallops with scallion oil always maintain their heat and consistent flavor, offering an authentic and dynamic District 4 sidewalk dining experience at a more accessible cost than neighboring large restaurants."
        },

        new Shop
        {
            Name = "Ốc Đào 2",
            Latitude = 10.761173,
            Longitude = 106.704948,
            BestSeller = "răng mực sào bơ tỏi",
            DescriptionVI = "Ốc Đào 2 phục vụ nhiều loại hải sản tươi ngon.",
            DescriptionEN = "Oc Dao 2 brings a more refined and meticulous vibe to the night food street, being a branch of the legendary Oc Dao brand in District 1 that has long established its position among gourmet diners and international tourists." +
            "Unlike the heavier seasoning of street stalls, the snails here are meticulously cleaned of sand and seasoned with a subtle, less oily style that highlights the natural sweetness of the seafood through (legendary) dishes like stir-fried squid teeth with garlic butter or creamy coconut stir-fried mud creepers." +
            "While the space is not overly large, it scores points for its cleanliness, politeness, and professional service, making it very suitable for family gatherings or those wanting to enjoy Michelin-selected snail dishes in a comfortable atmosphere.\r\n"
        },

        new Shop
        {
            Name = "Ốc Nhi 20k",
            Latitude = 10.761283,
            Longitude = 106.705973,
            BestSeller = "Sò điệp nướng mỡ hành",
            DescriptionVI = "Ốc Nhi 20k nổi tiếng với món Sò điệp nướng mỡ hành.",
            DescriptionEN = "Oc Nhi 20k is a prime representative of the flat-price snail model extremely popular on Vinh Khanh sidewalks, attracting a large number of students and diners who want to eat well without worrying about their wallets." +
            "With super cheap prices ranging from only 20,000 VND to 30,000 VND per plate, the restaurant serves smaller portions so that customers can comfortably order 5-7 different dishes at once—from salt-toasted cana snails to blood cockles stir-fried with garlic—while remaining very economical." +
            "Although the snails are smaller in size compared to the major restaurants, the cooking style at Oc Nhi is still very appetizing and rich in flavor, creating a cozy and exciting budget sidewalk dining experience in the heart of the city at night.",
            Popular = true
        },

        new Shop
        {
            Name = "Ốc su 20k",
            Latitude = 10.760324,
            Longitude = 106.707330,
            BestSeller = "Sò nướng",
            DescriptionVI = "Bé Ốc là quán hải sản bình dân đông khách.",
            DescriptionEN = "Oc Su 20k is also an ideal stop in the low-cost snail segment, featuring quick service and a menu that flexibly changes based on the fresh seafood imported daily." +
            "The restaurant focuses on basic snail dishes but prepares them very skillfully; in particular, the ginger fish sauce and various tamarind or garlic sauces always have a smooth consistency and a unique flavor that keeps customers coming back. " +
            "Sitting at Oc Su, you can clearly feel the bustling rhythm of the Vinh Khanh night market through simple plastic tables and chairs lined up along the sidewalk, where with just a little pocket change, you can enjoy a full-flavored snail feast with friends in a breezy open space.",
            Popular = true
        }
    };


    public MainPage()
    {
        InitializeComponent();

    }
    Shop? currentShop = null;
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

            var popularShops = shops
                .Where(s => s.Popular)
                .OrderBy(s => s.Distance)
                .ToList();

            popularList.ItemsSource = popularShops;
            popularList.IsVisible = true;

            currentShop = null;
            replayButton.IsVisible = false;
            infoLabel.Text = "Ban dang o ngoai khu pho am thuc";
        }
        catch (Exception ex)
        {
            infoLabel.Text = ex.Message;
        }
    }

    async void ShowShopInfo(Shop shop)
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        string text;

        if (lang == "vi")
        {
            text = $"{shop.Name}\n{shop.DescriptionEN}\n Mon ban chay: {shop.BestSeller}";
            infoLabel.Text = text;
        }
        else
        {
            text = $"{shop.Name}\n{shop.DescriptionEN}\n Best seller: {shop.BestSeller}";
            infoLabel.Text = text;
        }

        replayButton.IsVisible = false;

        await SpeakText(text);

        replayButton.IsVisible = true;
    }
    async Task SpeakText(string text)
    {
        await TextToSpeech.Default.SpeakAsync(text);
    }

    async void OnReplayClicked(object sender, EventArgs e)
    {
        if (currentShop != null)
        {
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            string text;

            if (lang == "vi")
            {
                text = $"{currentShop.Name}. {currentShop.DescriptionVI}. Mon ban chay: {currentShop.BestSeller}";
            }
            else
            {
                text = $"{currentShop.Name}. {currentShop.DescriptionEN}. Best seller: {currentShop.BestSeller}";
            }

            await SpeakText(text);
        }
    }
}