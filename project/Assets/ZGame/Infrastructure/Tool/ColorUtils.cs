using UnityEngine;

public class ColorUtils
{
    public static Color HSVToRGB(float h, float s, float v)
    {
        float r = 0;
        float g = 0;
        float b = 0;
        float i;
        float f;
        float p;
        float q;
        float t;
        i = Mathf.Floor(h * 6);
        f = h * 6 - i;
        p = v * (1 - s);
        q = v * (1 - f * s);
        t = v * (1 - (1 - f) * s);
        switch ((int)i % 6)
        {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            case 5: r = v; g = p; b = q; break;
        }
        Color color = new Color(r, g, b);
        return color;
    }
    public static Vector3 RGBToHSV(Color rgb)
    {
        float R = rgb.r;
        if (R < 0)
        {
            R = 0;
        }
        else if (R > 1)
        {
            R = 1;
        }
        float G = rgb.g;
        if (G < 0)
        {
            G = 0;
        }
        else if (G > 1)
        {
            G = 1;
        }
        float B = rgb.b;
        if (B < 0)
        {
            B = 0;
        }
        else if (B > 1)
        {
            B = 1;
        }
        float H;
        float S;
        float V;
        var min = Mathf.Min(R, G, B);
        var max = Mathf.Max(R, G, B);
        float achromatic;
        float delta;
        Vector3 hsv;

        if (R == G && G == B)
        {
            achromatic = 1;
        }
        else
        {
            achromatic = 0;
        }
        V = max;
        delta = max - min;
        if (max != 0)
        {
            S = delta / max;
        }
        else
        {
            S = 0;
            V = 0;
            H = 0;
            hsv.x = H;
            hsv.y = S;
            hsv.z = V;
            return hsv;
        }
        if (achromatic == 1)
        {
            H = 0;
        }
        else if (R == max)
        {
            H = 0 + (G - B) / delta;
        }
        else if (G == max)
        {
            H = 2 + (B - R) / delta;
        }
        else
        {
            H = 4 + (R - G) / delta;
        }
        hsv.x = H;
        hsv.y = S;
        hsv.z = V;
        return hsv;
    }
}