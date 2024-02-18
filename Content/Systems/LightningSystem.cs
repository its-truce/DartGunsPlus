using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Systems;

public class LightningSystem : ModSystem
{
    // public static void MakeDust(Vector2 source, Vector2 dest, float scale, short dustId, float sway = 80f, float jagednessNumerator = 1f)
    // {
    //     List<Vector2> dustPoints = CreateBolt(source, dest, sway, jagednessNumerator);
    //
    //     for (int i = 1; i < dustPoints.Count; i++)
    //     {
    //         Vector2 start = dustPoints[i - 1];
    //         Vector2 end = dustPoints[i];
    //         float numDust = (end - start).Length() * 0.4f;
    //
    //         for (int j = 0; j < numDust; j++)
    //         {
    //             float lerp = j / numDust;
    //             Vector2 dustPosition = Vector2.Lerp(start, end, lerp);
    //
    //             Dust d = Dust.NewDustPerfect(dustPosition, dustId, Scale: scale);
    //             d.noGravity = true;
    //             d.velocity = Main.rand.NextVector2Circular(0.3f, 0.3f);
    //         }
    //     }
    // }

    public static List<Vector2> CreateBolt(Vector2 source, Vector2 dest, float sway = 80f, float jagednessNumerator = 1f)
    {
        var results = new List<Vector2>();
        Vector2 tangent = dest - source;
        Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
        float length = tangent.Length();

        List<float> positions = new()
        {
            0
        };

        for (int i = 0; i < length / 16f; i++) positions.Add(Main.rand.NextFloat());

        positions.Sort();

        float jaggedness = jagednessNumerator / sway;

        Vector2 prevPoint = source;
        float prevDisplacement = 0f;
        for (int i = 1; i < positions.Count; i++)
        {
            float pos = positions[i];

            // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
            float scale = length * jaggedness * (pos - positions[i - 1]);

            // defines an envelope. Points near the middle of the bolt can be further from the central line.
            float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

            float displacement = Main.rand.NextFloat(-sway, sway);
            displacement -= (displacement - prevDisplacement) * (1 - scale);
            displacement *= envelope;

            Vector2 point = source + pos * tangent + displacement * normal;
            results.Add(point);
            prevPoint = point;
            prevDisplacement = displacement;
        }

        results.Add(prevPoint);
        results.Add(dest);
        results.Insert(0, source);

        return results;
    }
}