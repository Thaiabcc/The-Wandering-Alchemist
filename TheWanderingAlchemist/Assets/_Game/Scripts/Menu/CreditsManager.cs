using UnityEngine;
using TMPro; 

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI creditsTextComponent;

    void Start()
    {
        creditsTextComponent.text = @"<size=120%>THE WANDERING ALCHEMIST</size>

<size=60%>A Game By</size>
Dam Duc Thai

- - - - - - -

<size=70%>Game Design & Development</size>
Dam Duc Thai

<size=70%>Programming & System Engineering</size>
Dam Duc Thai

<size=70%>Worldbuilding & Quest Design</size>
Dam Duc Thai

<size=70%>Gameplay Systems & Balancing</size>
Dam Duc Thai

<size=70%>UI / UX Design</size>
Dam Duc Thai

- - - - - - -

<size=70%>Gameplay Review & Suggestions</size>
Ha Duc Tuan

<size=70%>Bug Testing & Quality Check</size>
Do Duy Khanh

<size=70%>Playtesting & Feedback</size>
Ha Duc Tuan
Do Duy Khanh

<size=70%>Quest Flow & Progression Review</size>
Ha Duc Tuan

<size=70%>Combat Balance Testing</size>
Do Duy Khanh

<size=70%>World Exploration Feedback</size>
Ha Duc Tuan

<size=70%>UI Testing & Accessibility Check</size>
Do Duy Khanh

<size=70%>Additional Creative Support</size>
Ha Duc Tuan
Do Duy Khanh

- - - - - - -

<size=70%>Visuals & Pixel Art Assets</size>
Admurin — UI: 02 Wood Theme
Anokolisa — Pixel Crawler - Free Pack
Brullov — Fire_fx_v1.0
Cainos — Pixel art top down -basic
Ddant1100 — TTrpg legacy_Potion#1
Eder Muniz — Pixel art infinite runner pack
Etahoshi — Fantasy minimal pixel art gui
HumblePixel — 16x16 Dungeon TileSet.v5
Kenmi — Cute Fantasy RPG
Pimen — Fire spell effect 02
Pipoya — FreeRPGTileSet 16x16
Schneider — Chests and coins
Xiao su zao shui — Pixel Art Assets
Bonsaiheldin — World Assets
Craftpix.net — Additional Graphics

<size=70%>Music & Sound Effects</size>
[Composer / Music Source]
Freesound.org

<size=70%>Tools & Software</size>
Unity Engine
Aseprite
Visual Studio

- - - - - - -

<size=70%>Special Thanks</size>
To those who found magic in the simplest herbs and stones

Unity Forums & Stack Overflow

And to the long nights of debugging,
testing, and trying again

- - - - - - -



“The cart must keep moving,
and every road hides another mystery...”


<size=110%>THANK YOU FOR PLAYING</size>



<size=40%>(Click anywhere to return)</size>";
    }
}